using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Snapshots;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Implementerar provenance-regler för buyer-proposal-aggregatet.
/// </summary>
public sealed class BuyerProposalReferenceValidator : IBuyerProposalReferenceValidator
{
    private readonly Arb_IntDbContext _db;
    private readonly ISupplySolutionCandidateSnapshotService _candidateSnapshotService;

    /// <summary>
    /// Skapar provenance-validatorn.
    /// </summary>
    public BuyerProposalReferenceValidator(
        Arb_IntDbContext db,
        ISupplySolutionCandidateSnapshotService candidateSnapshotService)
    {
        _db = db;
        _candidateSnapshotService = candidateSnapshotService;
    }

    /// <summary>
    /// Kontrollerar att proposalet använder en ReleasedForMatching revision från rätt RFQ.
    /// </summary>
    public async Task<Result> ValidateBuyerRfqRevisionAsync(
        Guid buyerRfqId,
        Guid buyerRfqRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revision = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqRevisionId)
            .Select(x => new
            {
                x.BuyerRfqId,
                x.Status
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (revision is null)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(BuyerRfqRevision),
                    buyerRfqRevisionId));
        }

        if (revision.BuyerRfqId != buyerRfqId)
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerRfqRevision belongs to another BuyerRfq."));
        }

        if (revision.Status != BuyerRfqRevisionStatus.ReleasedForMatching)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposal requires a ReleasedForMatching BuyerRfqRevision. Current status is '{revision.Status}'."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar att proposal-valutan existerar.
    /// </summary>
    public async Task<Result> ValidateCurrencyAsync(
        int currencyId,
        CancellationToken cancellationToken = default)
    {
        var exists = await _db.Set<Currency>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == currencyId,
                cancellationToken);

        return exists
            ? Result.Ok()
            : Result.Fail(
                new NotFoundError(
                    nameof(Currency),
                    currencyId));
    }

    /// <summary>
    /// Kontrollerar att ett disclosure-package riktas till proposalets buyer och går i riktningen ToBuyer.
    /// </summary>
    public async Task<Result> ValidateDisclosureReferenceAsync(
        Guid buyerBusinessRelationshipId,
        Guid? disclosurePackageRevisionId,
        CancellationToken cancellationToken = default)
    {
        if (!disclosurePackageRevisionId.HasValue)
        {
            return Result.Ok();
        }

        var disclosure = await _db.Set<DisclosurePackageRevision>()
            .AsNoTracking()
            .Where(x => x.Id == disclosurePackageRevisionId.Value)
            .Select(x => new
            {
                x.DisclosurePackage.AudienceBusinessRelationshipId,
                x.DisclosurePackage.Direction
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (disclosure is null)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(DisclosurePackageRevision),
                    disclosurePackageRevisionId.Value));
        }

        if (disclosure.AudienceBusinessRelationshipId != buyerBusinessRelationshipId)
        {
            return Result.Fail(
                new ConflictError(
                    "DisclosurePackageRevision is intended for another BusinessRelationship."));
        }

        if (disclosure.Direction != DisclosureDirection.ToBuyer)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposal requires a ToBuyer DisclosurePackage. Current direction is '{disclosure.Direction}'."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar att kandidaten är ett finaliserat och modererat resultat för exakt proposalets RFQ-revision.
    /// </summary>
    public async Task<Result> ValidateCandidateAsync(
        Guid buyerRfqRevisionId,
        Guid supplySolutionCandidateId,
        DateTimeOffset evaluatedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var candidate = await _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == supplySolutionCandidateId)
            .Select(x => new
            {
                x.Id,
                x.Status,
                x.SnapshotJson,
                x.SnapshotHash,
                x.ExpiresAtUtc,
                MatchingRunStatus = x.MatchingRun.Status,
                x.MatchingRun.BuyerRfqRevisionId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (candidate is null)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(SupplySolutionCandidate),
                    supplySolutionCandidateId));
        }

        if (candidate.BuyerRfqRevisionId != buyerRfqRevisionId)
        {
            return Result.Fail(
                new ConflictError(
                    "SupplySolutionCandidate originates from another BuyerRfqRevision."));
        }

        if (candidate.MatchingRunStatus != MatchingRunStatus.Completed)
        {
            return Result.Fail(
                new ConflictError(
                    "SupplySolutionCandidate must originate from a Completed MatchingRun."));
        }

        if (candidate.Status != SupplySolutionStatus.ApprovedForProposal)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplySolutionCandidate must be ApprovedForProposal. Current status is '{candidate.Status}'."));
        }

        if (candidate.ExpiresAtUtc.HasValue
            && candidate.ExpiresAtUtc.Value <= evaluatedAtUtc)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplySolutionCandidate expired at '{candidate.ExpiresAtUtc.Value:O}'."));
        }

        if (string.IsNullOrWhiteSpace(candidate.SnapshotJson)
            || string.IsNullOrWhiteSpace(candidate.SnapshotHash))
        {
            return Result.Fail(
                new ConflictError(
                    "SupplySolutionCandidate must contain an immutable snapshot."));
        }

        var computedHash = _candidateSnapshotService.ComputeHash(
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
    /// Kontrollerar att proposal-line-provenance håller sig inom samma RFQ-revision och option-candidate.
    /// </summary>
    public async Task<Result> ValidateLineAsync(
        Guid buyerRfqRevisionId,
        Guid supplySolutionCandidateId,
        Guid buyerRfqLineId,
        Guid? supplySolutionLineId,
        Guid? materialId,
        Guid? productId,
        int quantityUnitId,
        CancellationToken cancellationToken = default)
    {
        var rfqLineExists = await _db.Set<BuyerRfqLine>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == buyerRfqLineId
                    && x.BuyerRfqRevisionId == buyerRfqRevisionId,
                cancellationToken);

        if (!rfqLineExists)
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerRfqLine does not belong to the BuyerRfqRevision referenced by the proposal."));
        }

        if (supplySolutionLineId.HasValue)
        {
            var solutionLine = await _db.Set<SupplySolutionLine>()
                .AsNoTracking()
                .Where(x => x.Id == supplySolutionLineId.Value)
                .Select(x => new
                {
                    x.SupplySolutionCandidateId,
                    BuyerRfqLineId = x.MatchingRunLine.BuyerRfqLineId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (solutionLine is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(SupplySolutionLine),
                        supplySolutionLineId.Value));
            }

            if (solutionLine.SupplySolutionCandidateId != supplySolutionCandidateId)
            {
                return Result.Fail(
                    new ConflictError(
                        "SupplySolutionLine belongs to another SupplySolutionCandidate than the BuyerProposalOption."));
            }

            if (solutionLine.BuyerRfqLineId != buyerRfqLineId)
            {
                return Result.Fail(
                    new ConflictError(
                        "SupplySolutionLine belongs to another BuyerRfqLine than the BuyerProposalLine."));
            }
        }

        if (materialId.HasValue)
        {
            var exists = await _db.Set<Material>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == materialId.Value,
                    cancellationToken);

            if (!exists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Material),
                        materialId.Value));
            }
        }

        if (productId.HasValue)
        {
            var exists = await _db.Set<Product>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == productId.Value,
                    cancellationToken);

            if (!exists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Product),
                        productId.Value));
            }
        }

        var unitExists = await _db.Set<UnitOfMeasure>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == quantityUnitId,
                cancellationToken);

        if (!unitExists)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(UnitOfMeasure),
                    quantityUnitId));
        }

        return Result.Ok();
    }
}