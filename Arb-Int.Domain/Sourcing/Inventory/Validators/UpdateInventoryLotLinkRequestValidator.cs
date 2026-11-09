using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar metadata för en inventory lot provenance-länk.
    /// </summary>
    public sealed class UpdateInventoryLotLinkRequestValidator : AbstractValidator<UpdateInventoryLotLinkRequest>
    {
        /// <summary>
        /// Skapar regler för link-kind, kvantitet, yield och anteckningar.
        /// </summary>
        public UpdateInventoryLotLinkRequestValidator()
        {
            RuleFor(x => x.Kind)
                .IsInEnum();

            RuleFor(x => x.InputQuantity)
                .GreaterThanOrEqualTo(0m)
                .When(x => x.InputQuantity.HasValue);

            RuleFor(x => x)
                .Must(x => x.InputQuantity.HasValue == x.QuantityUnitId.HasValue)
                .WithMessage("InputQuantity and QuantityUnitId must either both be set or both be null.");

            RuleFor(x => x.YieldPercent)
                .InclusiveBetween(0m, 100m)
                .When(x => x.YieldPercent.HasValue);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}
