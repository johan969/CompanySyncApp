using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya supplier-quote-lines.
/// </summary>
public sealed class CreateSupplierQuoteLineRequestValidator : AbstractValidator<CreateSupplierQuoteLineRequest>
{
    /// <summary>
    /// Konfigurerar samma strukturella regler som line-modellen och databasen.
    /// </summary>
    public CreateSupplierQuoteLineRequestValidator()
    {
        RuleFor(x => x.BuyerRfqLineId).NotEqual(Guid.Empty).When(x => x.BuyerRfqLineId.HasValue);
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.SupplierMaterialOfferingId).NotEqual(Guid.Empty).When(x => x.SupplierMaterialOfferingId.HasValue);
        RuleFor(x => x.SupplierProcessCapabilityId).NotEqual(Guid.Empty).When(x => x.SupplierProcessCapabilityId.HasValue);
        RuleFor(x => x.SupplySolutionStepId).NotEqual(Guid.Empty).When(x => x.SupplySolutionStepId.HasValue);
        RuleFor(x => x.InventoryLotId).NotEqual(Guid.Empty).When(x => x.InventoryLotId.HasValue);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.OfferedQuantity).GreaterThan(0m);
        RuleFor(x => x.QuantityUnitId).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0m);
        RuleFor(x => x.IncotermRuleId).GreaterThan(0).When(x => x.IncotermRuleId.HasValue);
        RuleFor(x => x.NamedPlace).MaximumLength(512);
        RuleFor(x => x.PriceBasis).MaximumLength(1000);
        RuleFor(x => x.SupplierLineReference).MaximumLength(255);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);

        RuleFor(x => x)
            .Must(x => SupplierQuoteValidationRules.HaveLineTarget(
                x.SupplierMaterialOfferingId,
                x.SupplierProcessCapabilityId,
                x.SupplySolutionStepId,
                x.InventoryLotId))
            .WithMessage("At least one supplier quote line target is required.");

        RuleFor(x => x)
            .Must(x => SupplierQuoteValidationRules.HaveValidWindow(x.AvailableFromUtc, x.AvailableToUtc))
            .WithMessage("AvailableToUtc cannot be earlier than AvailableFromUtc.");
    }
}