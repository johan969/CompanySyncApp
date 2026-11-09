using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar verifieringsstatus för en supplier process capability.
    /// </summary>
    public sealed class SetSupplierProcessCapabilityVerificationRequestValidator : AbstractValidator<SetSupplierProcessCapabilityVerificationRequest>
    {
        /// <summary>
        /// Skapar regler för verification-status.
        /// </summary>
        public SetSupplierProcessCapabilityVerificationRequestValidator()
        {
            RuleFor(x => x.VerificationStatus).IsInEnum();
        }
    }
}
