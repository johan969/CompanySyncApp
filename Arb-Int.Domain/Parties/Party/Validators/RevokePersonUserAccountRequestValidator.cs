using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som återkallar användarkonton.
    /// </summary>
    public sealed class RevokePersonUserAccountRequestValidator : AbstractValidator<RevokePersonUserAccountRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för återkallning.
        /// </summary>
        public RevokePersonUserAccountRequestValidator()
        {
            RuleFor(x => x.RowVersion)
                .NotEqual(0u);
        }
    }
}