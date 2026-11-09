using FluentValidation;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Validerar grundformen för en PersonUserAccount-invitation.
    /// </summary>
    public sealed class InvitePersonUserAccountRequestValidator : AbstractValidator<InvitePersonUserAccountRequest>
    {
        /// <summary>
        /// Skapar valideringsreglerna för invitationen.
        /// </summary>
        public InvitePersonUserAccountRequestValidator()
        {
            RuleFor(x => x.ApplicationUserId).NotEmpty();
            RuleFor(x => x.AuthEmailContactPointId).NotEmpty();
            RuleFor(x => x.Notes).MaximumLength(4096);
        }
    }
}
