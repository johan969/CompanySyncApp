using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Trading.Snapshots;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar kandidat-review och append-only mänskliga moderation-beslut utan att ändra det maskinella snapshotet.
/// </summary>
public sealed class MatchingModerationService : IMatchingModerationService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ISupplySolutionCandidateSnapshotService _snapshotService;

    /// <summary>
    /// Skapar moderation-servicen med databas-, validerings- och snapshot-beroenden.
    /// </summary>
    public MatchingModerationService(
        Arb_IntDbContext db,
        IValidationService validationService,
        ISupplySolutionCandidateSnapshotService snapshotService)
    {
        _db = db;
        _validationService = validationService;
        _snapshotService = snapshotService;
    }

    /// <summary>
    /// Hämtar moderation-historiken i omvänd kronologisk ordning.
    /// </summary>
    public async Task<Result<IReadOnlyList<MatchingModerationDecisionResponse>>> GetHistoryAsync(
        Guid supplySolutionCandidateId,
        CancellationToken cancellationToken = default)
    {
        var candidate = await _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == supplySolutionCandidateId)
            .Select(x => new
            {
                x.Id,
                x.Outcome,
                x.TotalScore
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<IReadOnlyList<MatchingModerationDecisionResponse>>(
                new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var decisions = await _db.Set<MatchingModerationDecision>()
            .AsNoTracking()
            .Where(x => x.SupplySolutionCandidateId == candidate.Id)
            .OrderByDescending(x => x.DecidedAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(x => new MatchingModerationDecisionResponse
            {
                Id = x.Id,
                SupplySolutionCandidateId = x.SupplySolutionCandidateId,
                Decision = x.Decision,
                DecidedByUserId = x.DecidedByUserId,
                DecidedAtUtc = x.DecidedAtUtc,
                IsManualOverride = x.IsManualOverride,
                OverriddenOutcome = x.OverriddenOutcome,
                OverriddenScore = x.OverriddenScore,
                EffectiveOutcome = x.IsManualOverride && x.OverriddenOutcome.HasValue
                    ? x.OverriddenOutcome.Value
                    : candidate.Outcome,
                EffectiveScore = x.IsManualOverride && x.OverriddenScore.HasValue
                    ? x.OverriddenScore.Value
                    : candidate.TotalScore,
                Reason = x.Reason,
                Notes = x.Notes,
                RowVersion = x.RowVersion
            })
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<MatchingModerationDecisionResponse>>(decisions);
    }

    /// <summary>
    /// Påbörjar review först när hela matching run är Completed och kandidatens maskinresultat är verifierbart.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> BeginReviewAsync(
        Guid supplySolutionCandidateId,
        BeginSupplySolutionCandidateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var candidate = await LoadCandidateAsync(
            supplySolutionCandidateId,
            cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var readiness = ValidateModerationBaseline(candidate);

        if (readiness.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(readiness.Errors);
        }

        if (candidate.Status != SupplySolutionStatus.Generated)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError(
                    $"Only a Generated candidate can begin review. Current status is '{candidate.Status}'."));
        }

        _db.Entry(candidate)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        candidate.Status = SupplySolutionStatus.UnderReview;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConcurrencyError(
                    nameof(SupplySolutionCandidate),
                    candidate.Id));
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Sparar ett nytt moderation-beslut och ändrar endast kandidatens workflow-status.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> DecideAsync(
        Guid supplySolutionCandidateId,
        CreateMatchingModerationDecisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var candidate = await LoadCandidateAsync(
            supplySolutionCandidateId,
            cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var readiness = ValidateModerationBaseline(candidate);

        if (readiness.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(readiness.Errors);
        }

        var transitionResult = ResolveTargetStatus(
            candidate.Status,
            request.Decision);

        if (transitionResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                transitionResult.Errors);
        }

        var now = DateTimeOffset.UtcNow;

        var decision = new MatchingModerationDecision
        {
            Id = Guid.CreateVersion7(),
            SupplySolutionCandidateId = candidate.Id,
            Decision = request.Decision,
            DecidedByUserId = request.DecidedByUserId,
            DecidedAtUtc = now,
            IsManualOverride = request.IsManualOverride,
            OverriddenOutcome = request.OverriddenOutcome,
            OverriddenScore = request.OverriddenScore,
            Reason = request.Reason.Trim(),
            Notes = TrimToNull(request.Notes)
        };

        _db.Entry(candidate)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplySolutionCandidateRowVersion;

        /*
         * ActionDateUtc ändras även när status förblir UnderReview,
         * exempelvis vid RequestVerification.
         */
        candidate.ActionDateUtc = now;
        candidate.Status = transitionResult.Value;

        _db.Set<MatchingModerationDecision>().Add(decision);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConcurrencyError(
                    nameof(SupplySolutionCandidate),
                    candidate.Id));
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Översätter ett moderation-beslut till en tillåten candidate-status.
    /// <para>
    /// Beslut och workflow hålls explicita i stället för att exponera en generell ChangeStatus-metod.<br />
    /// Exempel: Shortlist kräver UnderReview medan ApproveForProposal kräver Shortlisted.
    /// </para>
    /// </summary>
    private static Result<SupplySolutionStatus> ResolveTargetStatus(
        SupplySolutionStatus currentStatus,
        MatchingModerationDecisionKind decision)
    {
        return decision switch
        {
            MatchingModerationDecisionKind.Shortlist
                when currentStatus == SupplySolutionStatus.UnderReview
                => Result.Ok(SupplySolutionStatus.Shortlisted),

            MatchingModerationDecisionKind.Reject
                when currentStatus is SupplySolutionStatus.UnderReview
                    or SupplySolutionStatus.Shortlisted
                => Result.Ok(SupplySolutionStatus.Rejected),

            MatchingModerationDecisionKind.RequestVerification
                when currentStatus is SupplySolutionStatus.UnderReview
                    or SupplySolutionStatus.Shortlisted
                => Result.Ok(SupplySolutionStatus.UnderReview),

            MatchingModerationDecisionKind.ApproveForProposal
                when currentStatus == SupplySolutionStatus.Shortlisted
                => Result.Ok(SupplySolutionStatus.ApprovedForProposal),

            MatchingModerationDecisionKind.Reopen
                when currentStatus is SupplySolutionStatus.Shortlisted
                    or SupplySolutionStatus.Rejected
                    or SupplySolutionStatus.ApprovedForProposal
                    or SupplySolutionStatus.Expired
                => Result.Ok(SupplySolutionStatus.UnderReview),

            _ => Result.Fail<SupplySolutionStatus>(
                new ConflictError(
                    $"Moderation decision '{decision}' is not valid when candidate status is '{currentStatus}'."))
        };
    }

    /// <summary>
    /// Kontrollerar att kandidaten kommer från en Completed run, inte har en förfallen status och har ett verifierbart immutable snapshot.
    /// </summary>
    private Result ValidateModerationBaseline(
        SupplySolutionCandidate candidate)
    {
        if (candidate.MatchingRun.Status != MatchingRunStatus.Completed)
        {
            return Result.Fail(
                new ConflictError(
                    $"Moderation requires a Completed MatchingRun. Current status is '{candidate.MatchingRun.Status}'."));
        }
        var expirationResult = ValidateNotExpired(
            candidate,
            DateTimeOffset.UtcNow);

        if (expirationResult.IsFailed)
        {
            return expirationResult;
        }

        if (string.IsNullOrWhiteSpace(candidate.SnapshotJson)
            || string.IsNullOrWhiteSpace(candidate.SnapshotHash))
        {
            return Result.Fail(
                new ConflictError(
                    "SupplySolutionCandidate must be finalized before moderation."));
        }

        var computedHash = _snapshotService.ComputeHash(
            candidate.SnapshotJson);

        if (!string.Equals(
                computedHash,
                candidate.SnapshotHash,
                StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail(
                new ConflictError(
                    "SupplySolutionCandidate snapshot hash is invalid."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar att kandidaten fortfarande ligger inom sin tidsmässiga giltighet.
    /// </summary>
    private static Result ValidateNotExpired(
        SupplySolutionCandidate candidate,
        DateTimeOffset evaluatedAtUtc)
    {
        if (candidate.Status == SupplySolutionStatus.Expired)
        {
            return Result.Fail(
                new ConflictError(
                    "SupplySolutionCandidate has expired."));
        }

        if (candidate.ExpiresAtUtc.HasValue
            && candidate.ExpiresAtUtc.Value <= evaluatedAtUtc)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplySolutionCandidate expired at '{candidate.ExpiresAtUtc.Value:O}'."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Läser candidate tillsammans med MatchingRun för moderation-operationer.
    /// </summary>
    private Task<SupplySolutionCandidate?> LoadCandidateAsync(
        Guid candidateId,
        CancellationToken cancellationToken)
    {
        return _db.Set<SupplySolutionCandidate>()
            .Include(x => x.MatchingRun)
            .SingleOrDefaultAsync(
                x => x.Id == candidateId,
                cancellationToken);
    }

    /// <summary>
    /// Projekterar kandidatens aktuella machine- och moderation-resultat.
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
    /// Trimmar text och ersätter whitespace med null.
    /// </summary>
    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}