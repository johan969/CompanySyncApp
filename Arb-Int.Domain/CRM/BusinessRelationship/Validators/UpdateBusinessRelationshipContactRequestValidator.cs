using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar kontakter på affärsrelationer.
    /// </summary>
    public sealed class UpdateBusinessRelationshipContactRequestValidator : AbstractValidator<UpdateBusinessRelationshipContactRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för uppdaterade relationskontakter.
        /// </summary>
        public UpdateBusinessRelationshipContactRequestValidator()
        {
            RuleFor(x => x.Role)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}