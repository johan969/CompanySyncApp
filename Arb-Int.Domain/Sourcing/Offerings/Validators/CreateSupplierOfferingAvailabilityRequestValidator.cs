using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny availability-post för ett supplier material offering.
    /// </summary>
    public sealed class CreateSupplierOfferingAvailabilityRequestValidator : AbstractValidator<CreateSupplierOfferingAvailabilityRequest>
    {
        /// <summary>
        /// Skapar regler för status, kvantiteter, MOQ, tidsfönster och referenser.
        /// </summary>
        public CreateSupplierOfferingAvailabilityRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum();

            RuleFor(x => x.AvailableQuantity)
                .GreaterThanOrEqualTo(0m);

            RuleFor(x => x.ReservedQuantity)
                .GreaterThanOrEqualTo(0m)
                .LessThanOrEqualTo(x => x.AvailableQuantity);

            RuleFor(x => x.QuantityUnitId)
                .GreaterThan(0);

            RuleFor(x => x.MinimumOrderQuantity)
                .GreaterThan(0m)
                .When(x => x.MinimumOrderQuantity.HasValue);

            RuleFor(x => x)
                .Must(
                    x => x.MinimumOrderQuantity.HasValue
                        == x.MinimumOrderUnitId.HasValue)
                .WithMessage(
                    "MinimumOrderQuantity and MinimumOrderUnitId must either both be set or both be null.");

            RuleFor(x => x.AvailableToUtc)
                .GreaterThanOrEqualTo(x => x.AvailableFromUtc)
                .When(x => x.AvailableToUtc.HasValue);

            RuleFor(x => x.LeadTimeDays)
                .GreaterThanOrEqualTo(0)
                .When(x => x.LeadTimeDays.HasValue);

            RuleFor(x => x.SourceReference)
                .MaximumLength(1000);
        }
    }
}
