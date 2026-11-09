using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar organisationsrelationer.
    /// </summary>
    public sealed class UpdateOrganizationPersonRequestValidator : AbstractValidator<UpdateOrganizationPersonRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för organisationsrelationsuppdateringar.
        /// </summary>
        public UpdateOrganizationPersonRequestValidator()
        {
            RuleFor(x => x.RelationshipType)
                .IsInEnum();

            RuleFor(x => x.JobTitle)
                .MaximumLength(255);

            RuleFor(x => x.Department)
                .MaximumLength(255);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}