using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar kvantitetsbilden för en inventory lot.
    /// </summary>
    public sealed class SetInventoryLotQuantitiesRequestValidator : AbstractValidator<SetInventoryLotQuantitiesRequest>
    {
        /// <summary>
        /// Skapar regler som speglar lotens databasconstraints.
        /// </summary>
        public SetInventoryLotQuantitiesRequestValidator()
        {
            RuleFor(x => x.TotalQuantity).GreaterThanOrEqualTo(0m);
            RuleFor(x => x.AvailableQuantity).GreaterThanOrEqualTo(0m);
            RuleFor(x => x.ReservedQuantity).GreaterThanOrEqualTo(0m);
            RuleFor(x => x.QuantityUnitId).GreaterThan(0);
            RuleFor(x => x).Must(x => x.AvailableQuantity + x.ReservedQuantity <= x.TotalQuantity)
                .WithMessage("AvailableQuantity plus ReservedQuantity cannot exceed TotalQuantity.");
        }
    }
}
