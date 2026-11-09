using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar metadata för en biologisk source-koppling.
    /// </summary>
    public sealed class UpdateSupplierMaterialOfferingBiologicalSourceRequestValidator : AbstractValidator<UpdateSupplierMaterialOfferingBiologicalSourceRequest>
    {
        /// <summary>
        /// Skapar regler för roll, proportion och anteckningar.
        /// </summary>
        public UpdateSupplierMaterialOfferingBiologicalSourceRequestValidator()
        {
            RuleFor(x => x.Role)
                .IsInEnum();

            RuleFor(x => x.ProportionPercent)
                .InclusiveBetween(0m, 100m)
                .When(x => x.ProportionPercent.HasValue);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}
