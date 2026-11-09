using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny origin-koppling för ett supplier material offering.
    /// </summary>
    public sealed class CreateSupplierMaterialOfferingOriginRequestValidator : AbstractValidator<CreateSupplierMaterialOfferingOriginRequest>
    {
        /// <summary>
        /// Skapar regler för geografisk plats, roll och anteckningar.
        /// </summary>
        public CreateSupplierMaterialOfferingOriginRequestValidator()
        {
            RuleFor(x => x.GeographicLocationId)
                .NotEmpty();

            RuleFor(x => x.Role)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}
