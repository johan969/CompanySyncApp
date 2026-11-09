using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som skapar RFQ-rader.
/// </summary>
public sealed class CreateBuyerRfqLineRequestValidator : AbstractValidator<CreateBuyerRfqLineRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för nya RFQ-rader.
    /// </summary>
    public CreateBuyerRfqLineRequestValidator()
    {
        BuyerRfqLineValidationRules.AddRules(
            this,
            x => x.LineNumber,
            x => x.Name,
            x => x.Description,
            x => x.BuyerLineReference,
            x => x.BuyerOriginalText,
            x => x.MaterialId,
            x => x.MaterialTypeId,
            x => x.IntendedUseCharacteristicId,
            x => x.RequiredQuantity,
            x => x.QuantityUnitId,
            x => x.LowerQuantityTolerancePercent,
            x => x.UpperQuantityTolerancePercent,
            x => x.AllowMultipleSuppliers,
            x => x.AllowMultipleOrigins,
            x => x.AllowMultipleLots,
            x => x.RequireSingleSupplier,
            x => x.RequireSingleOrigin,
            x => x.RequireSingleLot,
            x => x.MaximumSupplierCount,
            x => x.InternalNotes);
    }
}