using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny biologisk avgränsning för en supplier process capability.
    /// </summary>
    public sealed class CreateSupplierProcessCapabilityBiologicalSourceScopeRequestValidator : AbstractValidator<CreateSupplierProcessCapabilityBiologicalSourceScopeRequest>
    {
        /// <summary>
        /// Skapar regler för claim och anteckningar.
        /// </summary>
        public CreateSupplierProcessCapabilityBiologicalSourceScopeRequestValidator()
        {
            RuleFor(x => x.BusinessRelationshipBiologicalSourceClaimId)
                .NotEmpty();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}
