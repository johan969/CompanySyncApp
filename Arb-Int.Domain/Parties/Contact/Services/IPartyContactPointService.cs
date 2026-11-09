using FluentResults;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar kontaktpunkter för organisationer och personer.
    /// </summary>
    public interface IPartyContactPointService
    {
        /// <summary>
        /// Hämtar en parts kontaktpunkter.
        /// </summary>
        Task<Result<IReadOnlyList<PartyContactPointResponse>>> GetAsync(Guid partyId, bool includeHistory = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en ny kontaktpunkt.
        /// </summary>
        Task<Result<PartyContactPointResponse>> CreateAsync(Guid partyId, CreatePartyContactPointRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar metadata på en aktiv kontaktpunkt.
        /// </summary>
        Task<Result<PartyContactPointResponse>> UpdateAsync(Guid contactPointId, UpdatePartyContactPointRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Avslutar den nuvarande kontaktpunkten och skapar en ny.
        /// </summary>
        Task<Result<PartyContactPointResponse>> ReplaceAsync(Guid contactPointId, ReplacePartyContactPointRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Avslutar en aktiv kontaktpunkt.
        /// </summary>
        Task<Result> EndAsync(Guid contactPointId, EndPartyContactPointRequest request, CancellationToken cancellationToken = default);
    }
}