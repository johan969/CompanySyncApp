using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar active-status för en certification scheme-version.
    /// </summary>
    public sealed class SetCertificationSchemeActiveRequestValidator : AbstractValidator<SetCertificationSchemeActiveRequest>
    {
        /// <summary>
        /// Skapar validatorn för explicit active-status.
        /// </summary>
        public SetCertificationSchemeActiveRequestValidator()
        {
        }
    }
}
