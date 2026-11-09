using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Validerar referenser och provenance för supplier-quote-revisioner och lines.
/// </summary>
public interface ISupplierQuoteReferenceValidator
{
    /// <summary>
    /// Kontrollerar valuta och eventuell Incoterm för en revision.
    /// </summary>
    Task<Result> ValidateRevisionReferencesAsync(int currencyId, int? incotermRuleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kontrollerar att line-referenser existerar och hör till rätt supplier och RFQ.
    /// </summary>
    Task<Result> ValidateLineReferencesAsync(
        Guid supplierBusinessRelationshipId,
        Guid? buyerRfqRevisionId,
        Guid? buyerRfqLineId,
        Guid? supplierMaterialOfferingId,
        Guid? supplierProcessCapabilityId,
        Guid? supplySolutionStepId,
        Guid? inventoryLotId,
        int quantityUnitId,
        int? incotermRuleId,
        CancellationToken cancellationToken = default);
}