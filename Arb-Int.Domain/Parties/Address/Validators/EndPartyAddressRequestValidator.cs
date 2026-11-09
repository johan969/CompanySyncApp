using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som avslutar en PartyAddress.
    /// </summary>
    public sealed class EndPartyAddressRequestValidator : AbstractValidator<EndPartyAddressRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för sluttiden.
        /// </summary>
        public EndPartyAddressRequestValidator()
        {
            RuleFor(x => x.ValidToUtc)
                .NotEqual(default(DateTimeOffset));
        }
    }
}