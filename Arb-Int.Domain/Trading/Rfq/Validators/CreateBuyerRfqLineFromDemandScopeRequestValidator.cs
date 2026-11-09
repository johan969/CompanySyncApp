using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som skapar RFQ-rader från BuyerDemandScope.
/// </summary>
public sealed class CreateBuyerRfqLineFromDemandScopeRequestValidator : AbstractValidator<CreateBuyerRfqLineFromDemandScopeRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för scope-baserad radkonvertering.
    /// </summary>
    public CreateBuyerRfqLineFromDemandScopeRequestValidator()
    {
        RuleFor(x => x.LineNumber).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x));
        RuleFor(x => x.Description).NotNull();
        RuleFor(x => x.BuyerLineReference).MaximumLength(255);
        RuleFor(x => x.BuyerOriginalText).MaximumLength(8000);
        RuleFor(x => x.RequiredQuantity).GreaterThan(0).When(x => x.RequiredQuantity.HasValue);
        RuleFor(x => x.QuantityUnitId).GreaterThan(0).When(x => x.QuantityUnitId.HasValue);
        RuleFor(x => x.LowerQuantityTolerancePercent).InclusiveBetween(0m, 100m).When(x => x.LowerQuantityTolerancePercent.HasValue);
        RuleFor(x => x.UpperQuantityTolerancePercent).InclusiveBetween(0m, 100m).When(x => x.UpperQuantityTolerancePercent.HasValue);
        RuleFor(x => x.MaximumSupplierCount).GreaterThan(0).When(x => x.MaximumSupplierCount.HasValue);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);

        RuleFor(x => x).Must(x => !(x.AllowMultipleSuppliers && x.RequireSingleSupplier)).WithMessage("AllowMultipleSuppliers and RequireSingleSupplier cannot both be true.");
        RuleFor(x => x).Must(x => !(x.AllowMultipleOrigins && x.RequireSingleOrigin)).WithMessage("AllowMultipleOrigins and RequireSingleOrigin cannot both be true.");
        RuleFor(x => x).Must(x => !(x.AllowMultipleLots && x.RequireSingleLot)).WithMessage("AllowMultipleLots and RequireSingleLot cannot both be true.");
    }
}