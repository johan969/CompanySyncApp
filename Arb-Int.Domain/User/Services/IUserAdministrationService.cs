using Arb_Int.Domain.Parties;
using FluentResults;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Hanterar organisationens användaröversikt och onboarding av befintliga domänpersoner.
    /// </summary>
    public interface IUserAdministrationService
    {
        /// <summary>
        /// Hämtar organisationens personer tillsammans med eventuell PersonUserAccount- och Identity-information.
        /// </summary>
        Task<Result<IReadOnlyList<OrganizationUserListResponse>>> GetOrganizationUsersAsync(Guid organizationId, OrganizationUserQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar Identity-konto för en befintlig organisationsperson, skapar PersonUserAccount och skickar invitation.
        /// </summary>
        Task<Result<PersonUserAccountResponse>> InviteOrganizationPersonAsync(Guid organizationId, Guid personId, InviteOrganizationUserRequest request, CancellationToken cancellationToken = default);
    }
}
