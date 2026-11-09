namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Innehåller återanvändbara strukturella regler för supplier quotes.
/// </summary>
internal static class SupplierQuoteValidationRules
{
    /// <summary>
    /// Kontrollerar att ett tidsfönster ligger i korrekt ordning.
    /// </summary>
    public static bool HaveValidWindow(DateTimeOffset? fromUtc, DateTimeOffset? toUtc)
    {
        return !fromUtc.HasValue
            || !toUtc.HasValue
            || toUtc.Value >= fromUtc.Value;
    }

    /// <summary>
    /// Kontrollerar att minst ett kommersiellt target anges på offertraden.
    /// </summary>
    public static bool HaveLineTarget(
        Guid? supplierMaterialOfferingId,
        Guid? supplierProcessCapabilityId,
        Guid? supplySolutionStepId,
        Guid? inventoryLotId)
    {
        return supplierMaterialOfferingId.HasValue
            || supplierProcessCapabilityId.HasValue
            || supplySolutionStepId.HasValue
            || inventoryLotId.HasValue;
    }
}