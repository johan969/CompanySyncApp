using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar metadata för en certification scheme jurisdiction.
    /// </summary>
    public sealed class UpdateCertificationSchemeJurisdictionRequestValidator : AbstractValidator<UpdateCertificationSchemeJurisdictionRequest>
    {
        /// <summary>
        /// Skapar regler för kind och name.
        /// </summary>
        public UpdateCertificationSchemeJurisdictionRequestValidator()
        {
            RuleFor(x => x.Kind)
                .IsInEnum();

            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
