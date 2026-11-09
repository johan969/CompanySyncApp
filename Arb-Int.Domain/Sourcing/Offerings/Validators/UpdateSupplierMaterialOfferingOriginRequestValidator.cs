using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar metadata för en origin-koppling.
    /// </summary>
    public sealed class UpdateSupplierMaterialOfferingOriginRequestValidator : AbstractValidator<UpdateSupplierMaterialOfferingOriginRequest>
    {
        /// <summary>
        /// Skapar regler för roll och anteckningar.
        /// </summary>
        public UpdateSupplierMaterialOfferingOriginRequestValidator()
        {
            RuleFor(x => x.Role)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}
