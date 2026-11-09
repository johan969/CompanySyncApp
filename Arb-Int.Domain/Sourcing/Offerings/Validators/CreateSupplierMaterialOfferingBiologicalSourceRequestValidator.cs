using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny biologisk source-koppling för ett supplier material offering.
    /// </summary>
    public sealed class CreateSupplierMaterialOfferingBiologicalSourceRequestValidator : AbstractValidator<CreateSupplierMaterialOfferingBiologicalSourceRequest>
    {
        /// <summary>
        /// Skapar regler för claim, roll, proportion och anteckningar.
        /// </summary>
        public CreateSupplierMaterialOfferingBiologicalSourceRequestValidator()
        {
            RuleFor(x => x.BusinessRelationshipBiologicalSourceClaimId)
                .NotEmpty();

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
