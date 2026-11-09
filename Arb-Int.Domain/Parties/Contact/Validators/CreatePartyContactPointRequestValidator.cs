using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som skapar kontaktpunkter.
    /// </summary>
    public sealed class CreatePartyContactPointRequestValidator : AbstractValidator<CreatePartyContactPointRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya kontaktpunkter.
        /// </summary>
        public CreatePartyContactPointRequestValidator()
        {
            RuleFor(x => x.OrganizationPersonId)
                .NotEqual(Guid.Empty)
                .When(x => x.OrganizationPersonId.HasValue);

            RuleFor(x => x.Kind)
                .IsInEnum()
                .NotEqual(ContactPointKind.Unknown);

            RuleFor(x => x.Label)
                .MaximumLength(255);

            RuleFor(x => x.Value)
                .NotEmpty()
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .MaximumLength(2048);

            RuleFor(x => x.Extension)
                .MaximumLength(50);

            RuleFor(x => x.PermissionStatus)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}