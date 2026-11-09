using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som lägger till kontakter på affärsrelationer.
    /// </summary>
    public sealed class CreateBusinessRelationshipContactRequestValidator : AbstractValidator<CreateBusinessRelationshipContactRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya relationskontakter.
        /// </summary>
        public CreateBusinessRelationshipContactRequestValidator()
        {
            RuleFor(x => x.OrganizationPersonId)
                .NotEmpty();

            RuleFor(x => x.Role)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}