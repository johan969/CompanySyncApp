using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar expiration av immutable matching-candidates utan att ändra deras maskinella snapshot.
/// </summary>
public sealed class SupplySolutionCandidateLifecycleService : ISupplySolutionCandidateLifecycleService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;

    /// <summary>
    /// Skapar lifecycle-servicen med databas- och valideringsberoenden.
    /// </summary>
    public SupplySolutionCandidateLifecycleService(Arb_IntDbContext db, IValidationService validationService)
    {
        _db = db;
        _validationService = validationService;
    }

    /// <summary>
    /// Markerar en candidate som Expired utan att ändra det frysta maskinresultatet.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> ExpireAsync(
        Guid supplySolutionCandidateId,
        ExpireSupplySolutionCandidateRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var candidate = await _db.Set<SupplySolutionCandidate>()
            .Include(x => x.MatchingRun)
            .SingleOrDefaultAsync(
                x => x.Id == supplySolutionCandidateId,
                cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(
                    nameof(SupplySolutionCandidate),
                    supplySolutionCandidateId));
        }

        if (candidate.Status == SupplySolutionStatus.Expired)
        {
            return Result.Ok(
                await ProjectRequiredAsync(
                    candidate.Id,
                    cancellationToken));
        }

        if (candidate.MatchingRun.Status != MatchingRunStatus.Completed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError(
                    "SupplySolutionCandidate cannot expire before its MatchingRun is Completed."));
        }

        if (!CanExpire(candidate.Status))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError(
                    $"SupplySolutionCandidate with status '{candidate.Status}' cannot be expired."));
        }

        if (string.IsNullOrWhiteSpace(candidate.SnapshotJson)
            || string.IsNullOrWhiteSpace(candidate.SnapshotHash))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError(
                    "SupplySolutionCandidate must be finalized before it can expire."));
        }

        _db.Entry(candidate)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        candidate.Status = SupplySolutionStatus.Expired;
        candidate.ActionDateUtc = DateTimeOffset.UtcNow;

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
    /// Markerar förfallna finaliserade candidates som Expired i en begränsad worker-batch.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateExpirationBatchResponse>> ExpireDueAsync(
        DateTimeOffset evaluatedAtUtc,
        int batchSize = 500,
        CancellationToken cancellationToken = default)
    {
        if (batchSize <= 0)
        {
            return Result.Fail<SupplySolutionCandidateExpirationBatchResponse>(
                new ConflictError("BatchSize must be greater than zero."));
        }

        var candidateIds = await _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Where(x =>
                x.ExpiresAtUtc.HasValue
                && x.ExpiresAtUtc.Value <= evaluatedAtUtc
                && x.SnapshotJson != null
                && x.SnapshotHash != null
                && x.MatchingRun.Status == MatchingRunStatus.Completed
                && (
                    x.Status == SupplySolutionStatus.Generated
                    || x.Status == SupplySolutionStatus.UnderReview
                    || x.Status == SupplySolutionStatus.Shortlisted
                    || x.Status == SupplySolutionStatus.ApprovedForProposal
                ))
            .OrderBy(x => x.ExpiresAtUtc)
            .ThenBy(x => x.Id)
            .Select(x => x.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (candidateIds.Count == 0)
        {
            return Result.Ok(
                new SupplySolutionCandidateExpirationBatchResponse
                {
                    EvaluatedAtUtc = evaluatedAtUtc,
                    CandidateCount = 0,
                    ExpiredCount = 0
                });
        }

        /*
         * Statusvillkoret upprepas i UPDATE-queryn så att en candidate som
         * ändrats mellan SELECT och UPDATE inte skrivs över av worker-jobbet.
         */
        var expiredCount = await _db.Set<SupplySolutionCandidate>()
            .Where(x =>
                candidateIds.Contains(x.Id)
                && x.ExpiresAtUtc.HasValue
                && x.ExpiresAtUtc.Value <= evaluatedAtUtc
                && (
                    x.Status == SupplySolutionStatus.Generated
                    || x.Status == SupplySolutionStatus.UnderReview
                    || x.Status == SupplySolutionStatus.Shortlisted
                    || x.Status == SupplySolutionStatus.ApprovedForProposal
                ))
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.Status,
                        SupplySolutionStatus.Expired)
                    .SetProperty(
                        x => x.ActionDateUtc,
                        evaluatedAtUtc),
                cancellationToken);

        return Result.Ok(
            new SupplySolutionCandidateExpirationBatchResponse
            {
                EvaluatedAtUtc = evaluatedAtUtc,
                CandidateCount = candidateIds.Count,
                ExpiredCount = expiredCount
            });
    }

    /// <summary>
    /// Kontrollerar om kandidatens workflow-status får övergå till Expired.
    /// </summary>
    private static bool CanExpire(SupplySolutionStatus status)
    {
        return status is SupplySolutionStatus.Generated
            or SupplySolutionStatus.UnderReview
            or SupplySolutionStatus.Shortlisted
            or SupplySolutionStatus.ApprovedForProposal;
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
}