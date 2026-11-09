using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som ersätter en PartyAddress.
    /// </summary>
    public sealed class ReplacePartyAddressRequestValidator : AbstractValidator<ReplacePartyAddressRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för adressersättningar.
        /// </summary>
        public ReplacePartyAddressRequestValidator()
        {
            RuleFor(x => x.Address)
                .NotNull()
                .SetValidator(new AddressRequestValidator());

            RuleFor(x => x.UsageType)
                .IsInEnum()
                .NotEqual(AddressUsageType.Unknown);

            RuleFor(x => x.Label)
                .MaximumLength(255);

            RuleFor(x => x.Attention)
                .MaximumLength(255);
        }
    }
}