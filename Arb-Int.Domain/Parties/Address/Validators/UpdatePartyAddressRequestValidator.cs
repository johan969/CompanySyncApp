using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar metadataändringar på PartyAddress.
    /// </summary>
    public sealed class UpdatePartyAddressRequestValidator : AbstractValidator<UpdatePartyAddressRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för metadataändringar.
        /// </summary>
        public UpdatePartyAddressRequestValidator()
        {
            RuleFor(x => x.Label)
                .MaximumLength(255);

            RuleFor(x => x.Attention)
                .MaximumLength(255);
        }
    }
}