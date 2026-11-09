using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar presentation och validity för en certification scheme-version.
    /// </summary>
    public sealed class UpdateCertificationSchemeRequestValidator : AbstractValidator<UpdateCertificationSchemeRequest>
    {
        /// <summary>
        /// Skapar regler för name, description och validity.
        /// </summary>
        public UpdateCertificationSchemeRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Description)
                .NotNull();

            RuleFor(x => x.ValidToUtc)
                .GreaterThanOrEqualTo(x => x.ValidFromUtc!.Value)
                .When(x => x.ValidFromUtc.HasValue && x.ValidToUtc.HasValue);
        }
    }
}
