using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar metadata för en supplier process capability.
    /// </summary>
    public sealed class UpdateSupplierProcessCapabilityRequestValidator : AbstractValidator<UpdateSupplierProcessCapabilityRequest>
    {
        /// <summary>
        /// Skapar regler för deklarerad metadata och observationsdatum.
        /// </summary>
        public UpdateSupplierProcessCapabilityRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(512);
            RuleFor(x => x.DeclaredProcessName).NotEmpty().MaximumLength(512);
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.Notes).MaximumLength(4096);
            RuleFor(x => x.LastObservedUtc).GreaterThanOrEqualTo(x => x.FirstObservedUtc);
        }
    }
}
