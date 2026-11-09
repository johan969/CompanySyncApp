using FluentResults;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar adresser och historiserade PartyAddress-kopplingar.
    /// </summary>
    public interface IPartyAddressService
    {
        /// <summary>
        /// Hämtar en parts adresser.
        /// </summary>
        Task<Result<IReadOnlyList<PartyAddressResponse>>> GetAsync(Guid partyId, bool includeHistory = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lägger till en ny adresskoppling på en Party.
        /// </summary>
        Task<Result<PartyAddressResponse>> CreateAsync(Guid partyId, CreatePartyAddressRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar metadata på en aktiv adresskoppling utan att ändra själva adressen.
        /// </summary>
        Task<Result<PartyAddressResponse>> UpdateAsync(Guid partyAddressId, UpdatePartyAddressRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Avslutar den nuvarande adresskopplingen och skapar en ny.
        /// </summary>
        Task<Result<PartyAddressResponse>> ReplaceAsync(Guid partyAddressId, ReplacePartyAddressRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Avslutar en aktiv adresskoppling.
        /// </summary>
        Task<Result> EndAsync(Guid partyAddressId, EndPartyAddressRequest request, CancellationToken cancellationToken = default);
    }
}