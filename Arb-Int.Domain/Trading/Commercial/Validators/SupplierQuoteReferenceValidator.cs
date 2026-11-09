using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Implementerar referens- och provenance-validering för supplier quotes.
/// </summary>
public sealed class SupplierQuoteReferenceValidator : ISupplierQuoteReferenceValidator
{
    private readonly Arb_IntDbContext _db;

    /// <summary>
    /// Skapar validatorn med aktuell databascontext.
    /// </summary>
    public SupplierQuoteReferenceValidator(Arb_IntDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Kontrollerar att revisionens Currency och eventuell Incoterm existerar.
    /// </summary>
    public async Task<Result> ValidateRevisionReferencesAsync(
        int currencyId,
        int? incotermRuleId,
        CancellationToken cancellationToken = default)
    {
        var currencyExists = await _db.Set<Currency>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == currencyId, cancellationToken);

        if (!currencyExists)
        {
            return Result.Fail(new NotFoundError(nameof(Currency), currencyId));
        }

        if (incotermRuleId.HasValue)
        {
            var incotermExists = await _db.Set<IncotermRule>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == incotermRuleId.Value, cancellationToken);

            if (!incotermExists)
            {
                return Result.Fail(new NotFoundError(nameof(IncotermRule), incotermRuleId.Value));
            }
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar line-referenser och säkerställer att supplier- och RFQ-provenance inte blandas mellan olika aggregat.
    /// </summary>
    public async Task<Result> ValidateLineReferencesAsync(
        Guid supplierBusinessRelationshipId,
        Guid? buyerRfqRevisionId,
        Guid? buyerRfqLineId,
        Guid? supplierMaterialOfferingId,
        Guid? supplierProcessCapabilityId,
        Guid? supplySolutionStepId,
        Guid? inventoryLotId,
        int quantityUnitId,
        int? incotermRuleId,
        CancellationToken cancellationToken = default)
    {
        var unitExists = await _db.Set<UnitOfMeasure>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == quantityUnitId, cancellationToken);

        if (!unitExists)
        {
            return Result.Fail(new NotFoundError(nameof(UnitOfMeasure), quantityUnitId));
        }

        if (incotermRuleId.HasValue)
        {
            var incotermExists = await _db.Set<IncotermRule>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == incotermRuleId.Value, cancellationToken);

            if (!incotermExists)
            {
                return Result.Fail(new NotFoundError(nameof(IncotermRule), incotermRuleId.Value));
            }
        }

        if (buyerRfqLineId.HasValue)
        {
            if (!buyerRfqRevisionId.HasValue)
            {
                return Result.Fail(
                    new ConflictError(
                        "BuyerRfqLineId cannot be supplied when SupplierQuote is not linked to a BuyerRfqRevision."));
            }

            var buyerRfqLineExists = await _db.Set<BuyerRfqLine>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == buyerRfqLineId.Value
                        && x.BuyerRfqRevisionId == buyerRfqRevisionId.Value,
                    cancellationToken);

            if (!buyerRfqLineExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "BuyerRfqLine does not belong to the BuyerRfqRevision referenced by SupplierQuote."));
            }
        }

        if (supplierMaterialOfferingId.HasValue)
        {
            var offeringExists = await _db.Set<SupplierMaterialOffering>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == supplierMaterialOfferingId.Value
                        && x.BusinessRelationshipId == supplierBusinessRelationshipId,
                    cancellationToken);

            if (!offeringExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "SupplierMaterialOffering does not belong to the SupplierBusinessRelationship referenced by SupplierQuote."));
            }
        }

        if (supplierProcessCapabilityId.HasValue)
        {
            var capabilityExists = await _db.Set<SupplierProcessCapability>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == supplierProcessCapabilityId.Value
                        && x.BusinessRelationshipId == supplierBusinessRelationshipId,
                    cancellationToken);

            if (!capabilityExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "SupplierProcessCapability does not belong to the SupplierBusinessRelationship referenced by SupplierQuote."));
            }
        }

        if (inventoryLotId.HasValue)
        {
            var lotExists = await _db.Set<InventoryLot>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == inventoryLotId.Value
                        && x.SupplierBusinessRelationshipId == supplierBusinessRelationshipId,
                    cancellationToken);

            if (!lotExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "InventoryLot does not belong to the SupplierBusinessRelationship referenced by SupplierQuote."));
            }
        }

        if (supplySolutionStepId.HasValue)
        {
            if (!buyerRfqRevisionId.HasValue)
            {
                return Result.Fail(
                    new ConflictError(
                        "SupplySolutionStepId cannot be supplied when SupplierQuote is not linked to a BuyerRfqRevision."));
            }

            var step = await _db.Set<SupplySolutionStep>()
                .AsNoTracking()
                .Where(x => x.Id == supplySolutionStepId.Value)
                .Select(x => new
                {
                    x.ProviderBusinessRelationshipId,
                    BuyerRfqRevisionId = x.SupplySolutionCandidate.MatchingRun.BuyerRfqRevisionId,
                    BuyerRfqLineId = x.SupplySolutionLineId.HasValue
                        ? (Guid?)x.SupplySolutionLine!.MatchingRunLine.BuyerRfqLineId
                        : null
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (step is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(SupplySolutionStep),
                        supplySolutionStepId.Value));
            }

            if (step.BuyerRfqRevisionId != buyerRfqRevisionId.Value)
            {
                return Result.Fail(
                    new ConflictError(
                        "SupplySolutionStep originates from another BuyerRfqRevision."));
            }

            if (step.ProviderBusinessRelationshipId.HasValue
                && step.ProviderBusinessRelationshipId.Value != supplierBusinessRelationshipId)
            {
                return Result.Fail(
                    new ConflictError(
                        "SupplySolutionStep belongs to another provider BusinessRelationship."));
            }

            /*
             * Om både quote-raden och steget är line-specifika måste de peka på samma RFQ-line.
             * Candidate-wide steps får däremot användas utan denna extra line-koppling.
             */
            if (buyerRfqLineId.HasValue
                && step.BuyerRfqLineId.HasValue
                && step.BuyerRfqLineId.Value != buyerRfqLineId.Value)
            {
                return Result.Fail(
                    new ConflictError(
                        "SupplySolutionStep belongs to another BuyerRfqLine than the SupplierQuoteLine."));
            }
        }

        return Result.Ok();
    }
}