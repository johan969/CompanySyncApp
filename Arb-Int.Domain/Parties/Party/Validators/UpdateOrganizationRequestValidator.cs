using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar organisationer.
    /// </summary>
    public sealed class UpdateOrganizationRequestValidator : AbstractValidator<UpdateOrganizationRequest>
    {
        /// <summary>
        /// Konfigurerar valideringsreglerna för organisationsuppdateringar.
        /// </summary>
        public UpdateOrganizationRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(255);

            RuleFor(x => x.LegalName)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(255);

            RuleFor(x => x.TradingName)
                .MaximumLength(255);

            RuleFor(x => x.OrganizationKind)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}