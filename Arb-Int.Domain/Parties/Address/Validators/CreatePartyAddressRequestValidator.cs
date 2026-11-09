using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som lägger till adresser på parter.
    /// </summary>
    public sealed class CreatePartyAddressRequestValidator : AbstractValidator<CreatePartyAddressRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya PartyAddress-kopplingar.
        /// </summary>
        public CreatePartyAddressRequestValidator()
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