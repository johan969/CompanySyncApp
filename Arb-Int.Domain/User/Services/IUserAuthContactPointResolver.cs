using FluentResults;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Löser den PartyContactPoint som representerar ett Identity-kontos auth-e-postadress.
    /// </summary>
    public interface IUserAuthContactPointResolver
    {
        /// <summary>
        /// Hämtar vald auth-e-postkontaktpunkt eller använder en säker legacy-fallback när exakt en aktiv e-postkontaktpunkt matchar Identity-e-posten.
        /// </summary>
        Task<Result<UserAuthEmailContactPointResponse>> ResolveAsync(Guid applicationUserId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Innehåller den kontaktpunkt som används för auth-relaterad e-post.
    /// </summary>
    public sealed class UserAuthEmailContactPointResponse
    {
        /// <summary>Identifierar PartyContactPoint.</summary>
        public Guid ContactPointId { get; init; }
        /// <summary>Identifierar domänpersonen som kontaktpunkten tillhör.</summary>
        public Guid PersonId { get; init; }
        /// <summary>Den kanoniska e-postadressen från kontaktpunkten.</summary>
        public required string Email { get; init; }
    }
}
