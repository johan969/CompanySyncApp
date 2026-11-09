using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya supplier-allokeringar.
/// </summary>
public sealed class CreateSupplyAllocationRequestValidator : AbstractValidator<CreateSupplyAllocationRequest>
{
    /// <summary>
    /// Konfigurerar reglerna som motsvarar allocation-modellens invariants.
    /// </summary>
    public CreateSupplyAllocationRequestValidator()
    {
        RuleFor(x => x.SupplierBusinessRelationshipId).NotEmpty();
        RuleFor(x => x.SupplierMaterialOfferingId).NotEmpty();
        RuleFor(x => x.SupplierOfferingAvailabilityId).NotEqual(Guid.Empty).When(x => x.SupplierOfferingAvailabilityId.HasValue);
        RuleFor(x => x.InventoryLotId).NotEqual(Guid.Empty).When(x => x.InventoryLotId.HasValue);
        RuleFor(x => x.SupplierQuoteLineId).NotEqual(Guid.Empty).When(x => x.SupplierQuoteLineId.HasValue);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.AllocatedQuantity).GreaterThan(0m);
        RuleFor(x => x.QuantityUnitId).GreaterThan(0);
        RuleFor(x => x.AvailabilityVerificationState).IsInEnum();
        RuleFor(x => x.QualityVerificationState).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(4096);
        RuleFor(x => x).Must(HaveValidPriceCurrencyPair).WithMessage("UnitPurchasePrice and PurchasePriceCurrencyId must be supplied together and UnitPurchasePrice cannot be negative.");
    }

    /// <summary>
    /// Kontrollerar samma pris- och valutaregel som databasens check constraint.
    /// </summary>
    private static bool HaveValidPriceCurrencyPair(CreateSupplyAllocationRequest request)
    {
        return (!request.UnitPurchasePrice.HasValue && !request.PurchasePriceCurrencyId.HasValue)
            || (request.UnitPurchasePrice >= 0m && request.PurchasePriceCurrencyId.HasValue);
    }
}