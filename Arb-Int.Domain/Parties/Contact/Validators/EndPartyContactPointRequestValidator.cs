using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som avslutar kontaktpunkter.
    /// </summary>
    public sealed class EndPartyContactPointRequestValidator : AbstractValidator<EndPartyContactPointRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för sluttiden.
        /// </summary>
        public EndPartyContactPointRequestValidator()
        {
            RuleFor(x => x.ValidToUtc)
                .NotEqual(default(DateTimeOffset));
        }
    }
}