using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar statusändring för en supply certification.
    /// </summary>
    public sealed class ChangeSupplyCertificationStatusRequestValidator : AbstractValidator<ChangeSupplyCertificationStatusRequest>
    {
        /// <summary>
        /// Skapar regler för certification status.
        /// </summary>
        public ChangeSupplyCertificationStatusRequestValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
