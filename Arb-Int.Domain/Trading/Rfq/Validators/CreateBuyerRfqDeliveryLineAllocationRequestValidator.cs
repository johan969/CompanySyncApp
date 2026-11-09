using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som skapar delivery-line-allokeringar.
/// </summary>
public sealed class CreateBuyerRfqDeliveryLineAllocationRequestValidator : AbstractValidator<CreateBuyerRfqDeliveryLineAllocationRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för nya delivery-line-allokeringar.
    /// </summary>
    public CreateBuyerRfqDeliveryLineAllocationRequestValidator()
    {
        RuleFor(x => x.BuyerRfqLineId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.QuantityUnitId).GreaterThan(0);
    }
}