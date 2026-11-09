using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar worker-ägda statusövergångar för matching runs.
/// </summary>
public sealed class MatchingRunExecutionService : IMatchingRunExecutionService
{
    private readonly Arb_IntDbContext _db;
    private readonly ICanonicalJsonService _canonicalJsonService;
    private readonly IValidationService _validationService;

    /// <summary>
    /// Skapar execution-servicen med aktuell databascontext.
    /// </summary>
    public MatchingRunExecutionService(Arb_IntDbContext db, ICanonicalJsonService canonicalJsonService, IValidationService validationService)
    {
        _db = db;
        _canonicalJsonService = canonicalJsonService;
        _validationService = validationService;
    }

    /// <summary>
    /// Försöker claima en queued matching run för en worker.
    /// </summary>
    public async Task<Result<MatchingRunResponse>> StartAsync(Guid matchingRunId, CancellationToken cancellationToken = default)
    {
        var run = await _db.Set<MatchingRun>().SingleOrDefaultAsync(x => x.Id == matchingRunId, cancellationToken);

        if (run is null)
        {
            return Result.Fail<MatchingRunResponse>(new NotFoundError(nameof(MatchingRun), matchingRunId));
        }

        if (run.Status != MatchingRunStatus.Queued)
        {
            return Result.Fail<MatchingRunResponse>(new ConflictError($"MatchingRun with status '{run.Status}' cannot be started."));
        }

        run.Status = MatchingRunStatus.Running;
        run.StartedAtUtc = DateTimeOffset.UtcNow;
        run.CompletedAtUtc = null;
        run.ErrorMessage = null;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<MatchingRunResponse>(new ConcurrencyError(nameof(MatchingRun), run.Id));
        }

        return Result.Ok(await ProjectRequiredAsync(run.Id, cancellationToken));
    }

    /// <summary>
    /// Markerar en running matching run som Failed och sparar en begränsad feltext.
    /// </summary>
    public async Task<Result<MatchingRunResponse>> FailAsync(Guid matchingRunId, string errorMessage, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        var run = await _db.Set<MatchingRun>().SingleOrDefaultAsync(x => x.Id == matchingRunId, cancellationToken);

        if (run is null)
        {
            return Result.Fail<MatchingRunResponse>(new NotFoundError(nameof(MatchingRun), matchingRunId));
        }

        if (run.Status != MatchingRunStatus.Running)
        {
            return Result.Fail<MatchingRunResponse>(new ConflictError($"MatchingRun with status '{run.Status}' cannot fail."));
        }

        run.Status = MatchingRunStatus.Failed;
        run.CompletedAtUtc = DateTimeOffset.UtcNow;
        run.ErrorMessage = errorMessage.Length <= 8000 ? errorMessage : errorMessage[..8000];

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<MatchingRunResponse>(new ConcurrencyError(nameof(MatchingRun), run.Id));
        }

        return Result.Ok(await ProjectRequiredAsync(run.Id, cancellationToken));
    }

    /// <summary>
    /// Kontrollerar att run-input och samtliga candidate-snapshots är giltiga innan completion.
    /// </summary>
    public async Task<Result<MatchingRunCompletionReadinessResponse>> GetCompletionReadinessAsync(
        Guid matchingRunId,
        CancellationToken cancellationToken = default)
    {
        var run = await _db.Set<MatchingRun>()
            .AsNoTracking()
            .Include(x => x.Candidates)
            .SingleOrDefaultAsync(
                x => x.Id == matchingRunId,
                cancellationToken);

        if (run is null)
        {
            return Result.Fail<MatchingRunCompletionReadinessResponse>(
                new NotFoundError(nameof(MatchingRun), matchingRunId));
        }

        var issues = new List<MatchingReadinessIssueResponse>();

        if (run.Status != MatchingRunStatus.Running)
        {
            AddIssue(
                issues,
                "RunNotRunning",
                $"MatchingRun has status '{run.Status}'.",
                run.Id);
        }

        if (string.IsNullOrWhiteSpace(run.InputSnapshotJson)
            || string.IsNullOrWhiteSpace(run.InputSnapshotHash))
        {
            AddIssue(
                issues,
                "RunInputSnapshotMissing",
                "MatchingRun is missing its immutable input snapshot.",
                run.Id);
        }
        else
        {
            var computedInputHash = _canonicalJsonService.ComputeHash(
                run.InputSnapshotJson);

            if (!string.Equals(
                    computedInputHash,
                    run.InputSnapshotHash,
                    StringComparison.OrdinalIgnoreCase))
            {
                AddIssue(
                    issues,
                    "RunInputSnapshotInvalid",
                    "MatchingRun InputSnapshotHash does not match InputSnapshotJson.",
                    run.Id);
            }
        }

        var finalizedCandidates = 0;

        foreach (var candidate in run.Candidates)
        {
            if (candidate.Outcome == SupplySolutionOutcome.Incomplete)
            {
                AddIssue(
                    issues,
                    "CandidateOutcomeIncomplete",
                    $"Candidate '{candidate.CandidateNumber}' still has outcome Incomplete.",
                    candidate.Id);
            }

            if (string.IsNullOrWhiteSpace(candidate.SnapshotJson)
                || string.IsNullOrWhiteSpace(candidate.SnapshotHash))
            {
                AddIssue(
                    issues,
                    "CandidateNotFinalized",
                    $"Candidate '{candidate.CandidateNumber}' has not been finalized.",
                    candidate.Id);

                continue;
            }

            var computedCandidateHash = _canonicalJsonService.ComputeHash(
                candidate.SnapshotJson);

            if (!string.Equals(
                    computedCandidateHash,
                    candidate.SnapshotHash,
                    StringComparison.OrdinalIgnoreCase))
            {
                AddIssue(
                    issues,
                    "CandidateSnapshotInvalid",
                    $"Candidate '{candidate.CandidateNumber}' has an invalid snapshot hash.",
                    candidate.Id);

                continue;
            }

            finalizedCandidates++;
        }

        return Result.Ok(
            new MatchingRunCompletionReadinessResponse
            {
                MatchingRunId = run.Id,
                CandidateCount = run.Candidates.Count,
                FinalizedCandidateCount = finalizedCandidates,
                IsReady = issues.Count == 0,
                Issues = issues
            });
    }

    /// <summary>
    /// Slutför runnen först när input och samtliga genererade candidates är immutable och verifierbara.
    /// </summary>
    public async Task<Result<MatchingRunResponse>> CompleteAsync(
        Guid matchingRunId,
        CompleteMatchingRunRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<MatchingRunResponse>(
                validation.Errors);
        }

        var readiness = await GetCompletionReadinessAsync(
            matchingRunId,
            cancellationToken);

        if (readiness.IsFailed)
        {
            return Result.Fail<MatchingRunResponse>(
                readiness.Errors);
        }

        if (!readiness.Value.IsReady)
        {
            return Result.Fail<MatchingRunResponse>(
                readiness.Value.Issues.Select(
                    x => new ConflictError(
                        $"{x.Code}: {x.Message}")));
        }

        var run = await _db.Set<MatchingRun>()
            .SingleOrDefaultAsync(
                x => x.Id == matchingRunId,
                cancellationToken);

        if (run is null)
        {
            return Result.Fail<MatchingRunResponse>(
                new NotFoundError(nameof(MatchingRun), matchingRunId));
        }

        if (run.Status != MatchingRunStatus.Running)
        {
            return Result.Fail<MatchingRunResponse>(
                new ConflictError(
                    $"MatchingRun with status '{run.Status}' cannot be completed."));
        }

        _db.Entry(run)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        run.Status = MatchingRunStatus.Completed;
        run.CompletedAtUtc = DateTimeOffset.UtcNow;
        run.ErrorMessage = null;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<MatchingRunResponse>(
                new ConcurrencyError(
                    nameof(MatchingRun),
                    run.Id));
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                run.Id,
                cancellationToken));
    }

    /// <summary>
    /// Lägger till ett blockerande readiness-problem för matchningskörningen.
    /// </summary>
    private static void AddIssue(
        ICollection<MatchingReadinessIssueResponse> issues,
        string code,
        string message,
        Guid? entityId = null)
    {
        issues.Add(new MatchingReadinessIssueResponse
        {
            Code = code,
            Message = message,
            EntityId = entityId
        });
    }

    /// <summary>
    /// Projekterar en sparad matching run efter statusändringen.
    /// </summary>
    private async Task<MatchingRunResponse> ProjectRequiredAsync(Guid matchingRunId, CancellationToken cancellationToken)
    {
        return await _db.Set<MatchingRun>()
            .AsNoTracking()
            .Where(x => x.Id == matchingRunId)
            .Select(MatchingProjections.RunDetails)
            .SingleAsync(cancellationToken);
    }
}