using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som uppdaterar delivery-line-allokeringar.
/// </summary>
public sealed class UpdateBuyerRfqDeliveryLineAllocationRequestValidator : AbstractValidator<UpdateBuyerRfqDeliveryLineAllocationRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för uppdaterade delivery-line-allokeringar.
    /// </summary>
    public UpdateBuyerRfqDeliveryLineAllocationRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.QuantityUnitId).GreaterThan(0);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}