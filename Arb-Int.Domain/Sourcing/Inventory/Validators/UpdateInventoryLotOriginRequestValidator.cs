using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar metadata för en inventory lot origin.
    /// </summary>
    public sealed class UpdateInventoryLotOriginRequestValidator : AbstractValidator<UpdateInventoryLotOriginRequest>
    {
        /// <summary>
        /// Skapar regler för roll, verifiering, proportion och anteckningar.
        /// </summary>
        public UpdateInventoryLotOriginRequestValidator()
        {
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
