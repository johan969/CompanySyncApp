using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som uppdaterar RFQ-rader.
/// </summary>
public sealed class UpdateBuyerRfqLineRequestValidator : AbstractValidator<UpdateBuyerRfqLineRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för RFQ-radsuppdateringar.
    /// </summary>
    public UpdateBuyerRfqLineRequestValidator()
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

        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}