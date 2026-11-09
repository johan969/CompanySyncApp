using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar fysisk och postal adressdata.
    /// </summary>
    public sealed class AddressRequestValidator : AbstractValidator<AddressRequest>
    {
        /// <summary>
        /// Konfigurerar regler som motsvarar adressmodellens databasbegränsningar.
        /// </summary>
        public AddressRequestValidator()
        {
            RuleFor(x => x.CountryId)
                .NotEmpty();

            RuleFor(x => x.CareOf)
                .MaximumLength(255);

            RuleFor(x => x.PostOfficeBox)
                .MaximumLength(100);

            RuleFor(x => x.AddressLine1)
                .MaximumLength(255);

            RuleFor(x => x.AddressLine2)
                .MaximumLength(255);

            RuleFor(x => x.AddressLine3)
                .MaximumLength(255);

            RuleFor(x => x.PostalCode)
                .MaximumLength(50);

            RuleFor(x => x.Locality)
                .MaximumLength(255);

            RuleFor(x => x.AdministrativeArea)
                .MaximumLength(255);

            RuleFor(x => x)
                .Must(HasAddressData)
                .WithMessage("At least one address field must contain a value.");
        }

        /// <summary>
        /// Kontrollerar att requesten innehåller faktisk adressdata.
        /// </summary>
        private static bool HasAddressData(AddressRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.PostOfficeBox)
                || !string.IsNullOrWhiteSpace(request.AddressLine1)
                || !string.IsNullOrWhiteSpace(request.AddressLine2)
                || !string.IsNullOrWhiteSpace(request.AddressLine3)
                || !string.IsNullOrWhiteSpace(request.PostalCode)
                || !string.IsNullOrWhiteSpace(request.Locality)
                || !string.IsNullOrWhiteSpace(request.AdministrativeArea);
        }
    }
}