using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar ändringar av claimens verifieringsstatus.
    /// </summary>
    public sealed class SetBiologicalSourceClaimVerificationRequestValidator : AbstractValidator<SetBiologicalSourceClaimVerificationRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för verifieringsändringar.
        /// </summary>
        public SetBiologicalSourceClaimVerificationRequestValidator()
        {
            RuleFor(x => x.VerificationStatus)
                .IsInEnum();
        }
    }
}