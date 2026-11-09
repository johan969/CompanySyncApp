using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny origin-post för en inventory lot.
    /// </summary>
    public sealed class CreateInventoryLotOriginRequestValidator : AbstractValidator<CreateInventoryLotOriginRequest>
    {
        /// <summary>
        /// Skapar regler för location, roll, verifiering, proportion och anteckningar.
        /// </summary>
        public CreateInventoryLotOriginRequestValidator()
        {
            RuleFor(x => x.GeographicLocationId)
                .NotEmpty();

            RuleFor(x => x.Role)
                .IsInEnum();

            RuleFor(x => x.VerificationStatus)
                .IsInEnum();

            RuleFor(x => x.Proportion)
                .InclusiveBetween(0m, 1m)
                .When(x => x.Proportion.HasValue);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}
