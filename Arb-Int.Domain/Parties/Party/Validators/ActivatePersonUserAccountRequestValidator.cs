using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som aktiverar användarkonton.
    /// </summary>
    public sealed class ActivatePersonUserAccountRequestValidator : AbstractValidator<ActivatePersonUserAccountRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för aktivering.
        /// </summary>
        public ActivatePersonUserAccountRequestValidator()
        {
            RuleFor(x => x.RowVersion)
                .NotEqual(0u);
        }
    }
}