using FluentResults;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar kopplingar och livscykel mellan domänpersoner och Identity-användare.
    /// </summary>
    public interface IPersonUserAccountService
    {
        /// <summary>
        /// Hämtar en användarkontokoppling via dess identifierare.
        /// </summary>
        Task<Result<PersonUserAccountResponse>> GetByIdAsync(Guid personUserAccountId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar användarkontokopplingen för en person.
        /// </summary>
        Task<Result<PersonUserAccountResponse>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar användarkontokopplingen för en Identity-användare.
        /// </summary>
        Task<Result<PersonUserAccountResponse>> GetByApplicationUserAsync(Guid applicationUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Kopplar ett befintligt Identity-konto till en person och markerar kontot som inviterat.
        /// </summary>
        Task<Result<PersonUserAccountResponse>> InviteAsync(Guid personId, InvitePersonUserAccountRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar metadata för en användarkontokoppling.
        /// </summary>
        Task<Result<PersonUserAccountResponse>> UpdateAsync(Guid personUserAccountId, UpdatePersonUserAccountRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktiverar ett tidigare inviterat användarkonto.
        /// </summary>
        Task<Result<PersonUserAccountResponse>> ActivateAsync(Guid personUserAccountId, ActivatePersonUserAccountRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Återkallar åtkomsten för en inviterad eller aktiv användare.
        /// </summary>
        Task<Result<PersonUserAccountResponse>> RevokeAsync(Guid personUserAccountId, RevokePersonUserAccountRequest request, CancellationToken cancellationToken = default);
    }
}