using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som avslutar organisationsrelationer.
    /// </summary>
    public sealed class EndOrganizationPersonRequestValidator : AbstractValidator<EndOrganizationPersonRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för relationens sluttid.
        /// </summary>
        public EndOrganizationPersonRequestValidator()
        {
            RuleFor(x => x.ValidToUtc)
                .NotEqual(default(DateTimeOffset));
        }
    }
}