using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Trading.Snapshots;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Validerar och fryser kandidatens kompletta maskinella resultatgraf.
/// </summary>
public sealed class SupplySolutionCandidateFinalizationService : ISupplySolutionCandidateFinalizationService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ISupplySolutionCandidateSnapshotService _snapshotService;

    /// <summary>
    /// Skapar finalization-servicen.
    /// </summary>
    public SupplySolutionCandidateFinalizationService(
        Arb_IntDbContext db,
        IValidationService validationService,
        ISupplySolutionCandidateSnapshotService snapshotService)
    {
        _db = db;
        _validationService = validationService;
        _snapshotService = snapshotService;
    }

    /// <summary>
    /// Kontrollerar kandidatens fullständighet innan immutable snapshot skapas.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateFinalizationReadinessResponse>> GetReadinessAsync(
        Guid supplySolutionCandidateId,
        CancellationToken cancellationToken = default)
    {
        var candidate = await _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Include(x => x.MatchingRun)
                .ThenInclude(x => x.Lines)
            .Include(x => x.Lines)
                .ThenInclude(x => x.MatchingRunLine)
            .Include(x => x.Steps)
            .Include(x => x.RequirementEvaluations)
            .Include(x => x.ModerationDecisions)
            .AsSplitQuery()
            .SingleOrDefaultAsync(x => x.Id == supplySolutionCandidateId, cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateFinalizationReadinessResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var issues = new List<MatchingReadinessIssueResponse>();

        if (candidate.MatchingRun.Status != MatchingRunStatus.Running)
        {
            AddIssue(
                issues,
                "RunNotRunning",
                $"MatchingRun has status '{candidate.MatchingRun.Status}'.",
                candidate.MatchingRunId);
        }

        if (candidate.Status != SupplySolutionStatus.Generated)
        {
            AddIssue(
                issues,
                "CandidateNotGenerated",
                $"SupplySolutionCandidate has status '{candidate.Status}'.",
                candidate.Id);
        }

        if (candidate.Outcome == SupplySolutionOutcome.Incomplete)
        {
            AddIssue(
                issues,
                "CandidateOutcomeIncomplete",
                "SupplySolutionCandidate must have a final machine outcome before it can be finalized.",
                candidate.Id);
        }

        if (candidate.SnapshotJson is not null || candidate.SnapshotHash is not null)
        {
            AddIssue(
                issues,
                "CandidateAlreadyFinalized",
                "SupplySolutionCandidate already contains snapshot data.",
                candidate.Id);
        }

        if (candidate.ModerationDecisions.Count > 0)
        {
            AddIssue(
                issues,
                "ModerationExistsBeforeFinalization",
                "Machine output must be finalized before moderation decisions are created.",
                candidate.Id);
        }

        ValidateRunLines(candidate, issues);

        var requirements = await _db.Set<BuyerRfqRequirement>()
            .AsNoTracking()
            .Where(x =>
                x.BuyerRfqRevisionId == candidate.MatchingRun.BuyerRfqRevisionId
                && x.IsActive)
            .Select(x => new
            {
                x.Id,
                x.Name
            })
            .ToListAsync(cancellationToken);

        var evaluatedRequirementIds = candidate.RequirementEvaluations
            .Select(x => x.BuyerRfqRequirementId)
            .ToHashSet();

        foreach (var requirement in requirements)
        {
            if (!evaluatedRequirementIds.Contains(requirement.Id))
            {
                AddIssue(
                    issues,
                    "RequirementNotEvaluated",
                    $"Active requirement '{requirement.Name}' has no RequirementEvaluation.",
                    requirement.Id);
            }
        }

        var dependencies = await _db.Set<SupplySolutionStepDependency>()
            .AsNoTracking()
            .Where(x => x.SupplySolutionCandidateId == candidate.Id)
            .Select(x => new DependencyEdge
            {
                PredecessorStepId = x.PredecessorStepId,
                SuccessorStepId = x.SuccessorStepId
            })
            .ToListAsync(cancellationToken);

        if (ContainsCycle(candidate.Steps.Select(x => x.Id), dependencies))
        {
            AddIssue(
                issues,
                "StepGraphContainsCycle",
                "SupplySolutionStep dependency graph contains a cycle.",
                candidate.Id);
        }

        return Result.Ok(
            new SupplySolutionCandidateFinalizationReadinessResponse
            {
                SupplySolutionCandidateId = candidate.Id,
                IsReady = issues.Count == 0,
                Issues = issues
            });
    }

    /// <summary>
    /// Kontrollerar att kandidaten innehåller exakt ett line-resultat för varje MatchingRunLine.
    /// </summary>
    private static void ValidateRunLines(
        SupplySolutionCandidate candidate,
        ICollection<MatchingReadinessIssueResponse> issues)
    {
        var runLines = candidate.MatchingRun.Lines.ToDictionary(x => x.Id);

        var solutionLinesByRunLine = candidate.Lines
            .GroupBy(x => x.MatchingRunLineId)
            .ToDictionary(x => x.Key, x => x.ToList());

        foreach (var runLine in runLines.Values)
        {
            if (!solutionLinesByRunLine.TryGetValue(runLine.Id, out var solutionLines)
                || solutionLines.Count == 0)
            {
                AddIssue(
                    issues,
                    "RunLineMissingFromCandidate",
                    $"MatchingRunLine '{runLine.Id}' has no SupplySolutionLine.",
                    runLine.Id);

                continue;
            }

            if (solutionLines.Count != 1)
            {
                AddIssue(
                    issues,
                    "RunLineDuplicated",
                    $"MatchingRunLine '{runLine.Id}' has multiple SupplySolutionLines.",
                    runLine.Id);

                continue;
            }

            var line = solutionLines[0];

            if (line.RequiredQuantity != runLine.RequiredQuantity)
            {
                AddIssue(
                    issues,
                    "RequiredQuantityMismatch",
                    "SupplySolutionLine.RequiredQuantity differs from MatchingRunLine.RequiredQuantity.",
                    line.Id);
            }

            if (line.QuantityUnitId != runLine.QuantityUnitId)
            {
                AddIssue(
                    issues,
                    "QuantityUnitMismatch",
                    "SupplySolutionLine.QuantityUnitId differs from MatchingRunLine.QuantityUnitId.",
                    line.Id);
            }

            if (line.CoveredQuantity + line.ShortageQuantity != line.RequiredQuantity)
            {
                AddIssue(
                    issues,
                    "LineQuantityMismatch",
                    "CoveredQuantity plus ShortageQuantity must equal RequiredQuantity.",
                    line.Id);
            }

            if (line.CoveragePercent < 0m || line.CoveragePercent > 100m)
            {
                AddIssue(
                    issues,
                    "InvalidLineCoverage",
                    "SupplySolutionLine.CoveragePercent must be between 0 and 100.",
                    line.Id);
            }

            if (line.IsFullyCovered && line.ShortageQuantity != 0m)
            {
                AddIssue(
                    issues,
                    "FullyCoveredWithShortage",
                    "A fully covered SupplySolutionLine cannot contain shortage quantity.",
                    line.Id);
            }
        }

        foreach (var line in candidate.Lines)
        {
            if (!runLines.ContainsKey(line.MatchingRunLineId))
            {
                AddIssue(
                    issues,
                    "ForeignRunLine",
                    "SupplySolutionLine references a MatchingRunLine outside the candidate's MatchingRun.",
                    line.Id);
            }
        }
    }

    /// <summary>
    /// Kontrollerar hela dependency-grafen med topologisk sortering.
    /// <para>
    /// Returnerar true om grafen innehåller en cykel eller om en dependency pekar på ett step som inte finns i kandidaten.<br />
    /// Exempel: A → B → C → A returnerar true.
    /// </para>
    /// </summary>
    private static bool ContainsCycle(
        IEnumerable<Guid> stepIds,
        IEnumerable<DependencyEdge> dependencies)
    {
        var ids = stepIds.ToHashSet();

        var indegree = ids.ToDictionary(
            x => x,
            _ => 0);

        var adjacency = ids.ToDictionary(
            x => x,
            _ => new List<Guid>());

        foreach (var dependency in dependencies)
        {
            if (!ids.Contains(dependency.PredecessorStepId)
                || !ids.Contains(dependency.SuccessorStepId))
            {
                return true;
            }

            adjacency[dependency.PredecessorStepId]
                .Add(dependency.SuccessorStepId);

            indegree[dependency.SuccessorStepId]++;
        }

        var queue = new Queue<Guid>(
            indegree
                .Where(x => x.Value == 0)
                .Select(x => x.Key));

        var visitedCount = 0;

        while (queue.Count > 0)
        {
            var currentStepId = queue.Dequeue();
            visitedCount++;

            foreach (var successorStepId in adjacency[currentStepId])
            {
                indegree[successorStepId]--;

                if (indegree[successorStepId] == 0)
                {
                    queue.Enqueue(successorStepId);
                }
            }
        }

        return visitedCount != ids.Count;
    }

    /// <summary>
    /// Fryser kandidatens maskinella resultat efter readiness- och concurrency-kontroll.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> FinalizeAsync(
        Guid supplySolutionCandidateId,
        FinalizeSupplySolutionCandidateRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var candidate = await _db.Set<SupplySolutionCandidate>()
            .Include(x => x.MatchingRun)
            .SingleOrDefaultAsync(x => x.Id == supplySolutionCandidateId, cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        /*
         * Finalize är idempotent. Ett retry efter lyckad SaveChanges ska inte
         * generera ett nytt snapshot eller ändra det maskinella resultatet.
         */
        if (candidate.SnapshotJson is not null
            && candidate.SnapshotHash is not null)
        {
            var computedHash = _snapshotService.ComputeHash(candidate.SnapshotJson);

            if (!string.Equals(
                    computedHash,
                    candidate.SnapshotHash,
                    StringComparison.OrdinalIgnoreCase))
            {
                return Result.Fail<SupplySolutionCandidateResponse>(
                    new ConflictError("Stored SupplySolutionCandidate snapshot is invalid."));
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    candidate.Id,
                    cancellationToken));
        }

        if (candidate.SnapshotJson is not null
            || candidate.SnapshotHash is not null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError("SupplySolutionCandidate contains incomplete snapshot data."));
        }

        if (candidate.RowVersion != request.SupplySolutionCandidateRowVersion)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConcurrencyError(nameof(SupplySolutionCandidate), candidate.Id));
        }

        if (candidate.MatchingRun.RowVersion != request.MatchingRunRowVersion)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConcurrencyError(nameof(MatchingRun), candidate.MatchingRunId));
        }

        var readiness = await GetReadinessAsync(
            candidate.Id,
            cancellationToken);

        if (readiness.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                readiness.Errors);
        }

        if (!readiness.Value.IsReady)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                readiness.Value.Issues.Select(
                    x => new ConflictError($"{x.Code}: {x.Message}")));
        }

        var snapshot = await _snapshotService.CreateAsync(
            candidate.Id,
            cancellationToken);

        _db.Entry(candidate)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplySolutionCandidateRowVersion;

        _db.Entry(candidate.MatchingRun)
            .Property(x => x.RowVersion)
            .OriginalValue = request.MatchingRunRowVersion;

        candidate.SnapshotJson = snapshot.Json;
        candidate.SnapshotHash = snapshot.Hash;

        TouchRun(candidate.MatchingRun);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            var error = exception.Entries.Any(x => x.Entity is MatchingRun)
                ? new ConcurrencyError(nameof(MatchingRun), candidate.MatchingRunId)
                : new ConcurrencyError(nameof(SupplySolutionCandidate), candidate.Id);

            return Result.Fail<SupplySolutionCandidateResponse>(error);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Verifierar att PostgreSQL jsonb fortfarande representerar samma canonical snapshot.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateSnapshotVerificationResponse>> VerifySnapshotAsync(
        Guid supplySolutionCandidateId,
        CancellationToken cancellationToken = default)
    {
        var candidate = await _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == supplySolutionCandidateId)
            .Select(x => new
            {
                x.Id,
                x.SnapshotJson,
                x.SnapshotHash
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateSnapshotVerificationResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        if (string.IsNullOrWhiteSpace(candidate.SnapshotJson)
            || string.IsNullOrWhiteSpace(candidate.SnapshotHash))
        {
            return Result.Fail<SupplySolutionCandidateSnapshotVerificationResponse>(
                new ConflictError("SupplySolutionCandidate has not been finalized."));
        }

        var computedHash = _snapshotService.ComputeHash(
            candidate.SnapshotJson);

        return Result.Ok(
            new SupplySolutionCandidateSnapshotVerificationResponse
            {
                SupplySolutionCandidateId = candidate.Id,
                StoredHash = candidate.SnapshotHash,
                ComputedHash = computedHash,
                IsValid = string.Equals(
                    candidate.SnapshotHash,
                    computedHash,
                    StringComparison.OrdinalIgnoreCase)
            });
    }

    /// <summary>
    /// Markerar run-roten för aggregate-level optimistic concurrency.
    /// </summary>
    private static void TouchRun(MatchingRun run)
    {
        run.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Lägger till ett readiness-problem.
    /// </summary>
    private static void AddIssue(
        ICollection<MatchingReadinessIssueResponse> issues,
        string code,
        string message,
        Guid? entityId = null)
    {
        issues.Add(
            new MatchingReadinessIssueResponse
            {
                Code = code,
                Message = message,
                EntityId = entityId
            });
    }

    /// <summary>
    /// Projekterar kandidaten efter finalisering.
    /// </summary>
    private async Task<SupplySolutionCandidateResponse> ProjectRequiredAsync(
        Guid candidateId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == candidateId)
            .Select(SupplySolutionCandidateProjections.Details)
            .SingleAsync(cancellationToken);
    }


    /// <summary>
    /// Representerar en minimal dependency-edge för grafvalidering.
    /// </summary>
    private sealed class DependencyEdge
    {
        /// <summary>
        /// Identifierar predecessor-steget.
        /// </summary>
        public Guid PredecessorStepId { get; init; }

        /// <summary>
        /// Identifierar successor-steget.
        /// </summary>
        public Guid SuccessorStepId { get; init; }
    }
}
