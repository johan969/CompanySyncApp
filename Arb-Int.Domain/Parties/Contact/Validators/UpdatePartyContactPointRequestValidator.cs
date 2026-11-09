using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar metadataändringar på kontaktpunkter.
    /// </summary>
    public sealed class UpdatePartyContactPointRequestValidator : AbstractValidator<UpdatePartyContactPointRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för metadataändringar.
        /// </summary>
        public UpdatePartyContactPointRequestValidator()
        {
            RuleFor(x => x.Label)
                .MaximumLength(255);

            RuleFor(x => x.Extension)
                .MaximumLength(50);

            RuleFor(x => x.PermissionStatus)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}