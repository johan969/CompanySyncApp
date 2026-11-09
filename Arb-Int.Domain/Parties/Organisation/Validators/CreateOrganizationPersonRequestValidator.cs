using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som skapar organisationsrelationer.
    /// </summary>
    public sealed class CreateOrganizationPersonRequestValidator : AbstractValidator<CreateOrganizationPersonRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya organisationsrelationer.
        /// </summary>
        public CreateOrganizationPersonRequestValidator()
        {
            RuleFor(x => x.PersonId)
                .NotEmpty();

            RuleFor(x => x.RelationshipType)
                .IsInEnum();

            RuleFor(x => x.JobTitle)
                .MaximumLength(255);

            RuleFor(x => x.Department)
                .MaximumLength(255);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);

            RuleFor(x => x.ContactFunctionTypeIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("ContactFunctionTypeIds cannot contain duplicate values.");
        }
    }
}