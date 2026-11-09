using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny characteristic-koppling för ett supplier material offering.
    /// </summary>
    public sealed class CreateSupplierMaterialOfferingCharacteristicRequestValidator : AbstractValidator<CreateSupplierMaterialOfferingCharacteristicRequest>
    {
        /// <summary>
        /// Skapar regler för characteristic-identiteten.
        /// </summary>
        public CreateSupplierMaterialOfferingCharacteristicRequestValidator()
        {
            RuleFor(x => x.CharacteristicId)
                .NotEmpty();
        }
    }
}
