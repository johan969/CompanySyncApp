using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny jurisdiction för en certification scheme-version.
    /// </summary>
    public sealed class CreateCertificationSchemeJurisdictionRequestValidator : AbstractValidator<CreateCertificationSchemeJurisdictionRequest>
    {
        /// <summary>
        /// Skapar regler för kind, code och name.
        /// </summary>
        public CreateCertificationSchemeJurisdictionRequestValidator()
        {
            RuleFor(x => x.Kind)
                .IsInEnum();

            RuleFor(x => x.Code)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
