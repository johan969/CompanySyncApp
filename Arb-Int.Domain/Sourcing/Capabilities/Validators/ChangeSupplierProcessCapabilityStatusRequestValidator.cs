using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar statusändring för en supplier process capability.
    /// </summary>
    public sealed class ChangeSupplierProcessCapabilityStatusRequestValidator : AbstractValidator<ChangeSupplierProcessCapabilityStatusRequest>
    {
        /// <summary>
        /// Skapar regler för capability-status.
        /// </summary>
        public ChangeSupplierProcessCapabilityStatusRequestValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
