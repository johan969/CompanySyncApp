using FluentValidation;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Validerar grundformen för en organisationsanvändarinvitation.
    /// </summary>
    public sealed class InviteOrganizationUserRequestValidator : AbstractValidator<InviteOrganizationUserRequest>
    {
        /// <summary>
        /// Skapar valideringsreglerna för invitationen.
        /// </summary>
        public InviteOrganizationUserRequestValidator()
        {
            RuleFor(x => x.ContactPointId).NotEmpty();
            RuleFor(x => x.ClientUrl).NotEmpty();
        }
    }
}
