using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar metadata för en biologisk capability-avgränsning.
    /// </summary>
    public sealed class UpdateSupplierProcessCapabilityBiologicalSourceScopeRequestValidator : AbstractValidator<UpdateSupplierProcessCapabilityBiologicalSourceScopeRequest>
    {
        /// <summary>
        /// Skapar regler för anteckningar.
        /// </summary>
        public UpdateSupplierProcessCapabilityBiologicalSourceScopeRequestValidator()
        {
            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}
