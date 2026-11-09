using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som ersätter en organisationsrelations funktioner.
    /// </summary>
    public sealed class SetOrganizationPersonFunctionsRequestValidator : AbstractValidator<SetOrganizationPersonFunctionsRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för funktionsuppsättningen.
        /// </summary>
        public SetOrganizationPersonFunctionsRequestValidator()
        {
            RuleFor(x => x.ContactFunctionTypeIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("ContactFunctionTypeIds cannot contain duplicate values.");

            RuleForEach(x => x.ContactFunctionTypeIds)
                .NotEqual(default(int));
        }
    }
}