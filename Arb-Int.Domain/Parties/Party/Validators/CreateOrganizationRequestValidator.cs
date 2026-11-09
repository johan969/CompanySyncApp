using FluentValidation;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Validerar requests som skapar organisationer.
    /// </summary>
    public sealed class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
    {
        /// <summary>
        /// Konfigurerar valideringsreglerna för organisationer.
        /// </summary>
        public CreateOrganizationRequestValidator()
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