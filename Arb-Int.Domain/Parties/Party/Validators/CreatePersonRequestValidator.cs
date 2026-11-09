using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som skapar personer.
    /// </summary>
    public sealed class CreatePersonRequestValidator : AbstractValidator<CreatePersonRequest>
    {
        /// <summary>
        /// Konfigurerar valideringsreglerna för personer.
        /// </summary>
        public CreatePersonRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(255);

            RuleFor(x => x.GivenName)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(100);

            RuleFor(x => x.MiddleName)
                .MaximumLength(100);

            RuleFor(x => x.FamilyName)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(100);

            RuleFor(x => x.PreferredName)
                .MaximumLength(100);

            RuleFor(x => x.Honorific)
                .MaximumLength(50);

            RuleFor(x => x.PreferredLanguageCode)
                .MaximumLength(16);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}