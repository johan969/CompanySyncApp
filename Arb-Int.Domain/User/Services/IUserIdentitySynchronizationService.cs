using FluentResults;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Synkroniserar de Identity-fält som speglar domänpersonen och dess valda auth-e-postkontaktpunkt.
    /// </summary>
    public interface IUserIdentitySynchronizationService
    {
        /// <summary>
        /// Uppdaterar Identity-profilen och den länkade Personens duplicerade namn i samma operation.
        /// </summary>
        Task<Result> UpdateProfileAsync(Guid applicationUserId, UpdateUserDetailRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Synkroniserar Identity-namn och auth-e-post från en redan uppdaterad Person.
        /// <para>Anropas från PartyService efter en lyckad personuppdatering när en UserAccount finns.</para>
        /// </summary>
        Task<Result> SynchronizeIdentityFromPersonAsync(Guid personId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Flyttar Identity-kontots auth-e-postkoppling när den valda PartyContactPoint ersätts historiskt.
        /// </summary>
        Task<Result> ReplaceAuthEmailContactPointAsync(Guid oldContactPointId, Guid replacementContactPointId, CancellationToken cancellationToken = default);
    }
}
