using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar organisationsidentifierare.
    /// </summary>
    public sealed class UpdateOrganizationIdentifierRequestValidator : AbstractValidator<UpdateOrganizationIdentifierRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för uppdaterade organisationsidentifierare.
        /// </summary>
        public UpdateOrganizationIdentifierRequestValidator()
        {
            RuleFor(x => x.Scheme)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(50)
                .Matches("^[A-Za-z0-9][A-Za-z0-9-]*$")
                .WithMessage("Scheme may only contain letters, numbers and hyphens.");

            RuleFor(x => x.Value)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(255);

            RuleFor(x => x.Issuer)
                .MaximumLength(255);

            RuleFor(x => x.IssuingCountryId)
                .NotEqual(Guid.Empty)
                .When(x => x.IssuingCountryId.HasValue);
        }
    }
}