using FluentResults;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar externa källreferenser för parter.
    /// </summary>
    public interface IPartyExternalReferenceService
    {
        /// <summary>
        /// Hämtar samtliga externa referenser för en Party.
        /// </summary>
        Task<Result<IReadOnlyList<PartyExternalReferenceResponse>>> GetByPartyAsync(Guid partyId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar en extern referens via dess interna identifierare.
        /// </summary>
        Task<Result<PartyExternalReferenceResponse>> GetByIdAsync(Guid externalReferenceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar en extern referens via källsystemets stabila identitet.
        /// </summary>
        Task<Result<PartyExternalReferenceResponse>> GetBySourceAsync(string sourceSystem, string sourceRecordKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en ny extern referens för en Party.
        /// </summary>
        Task<Result<PartyExternalReferenceResponse>> CreateAsync(Guid partyId, CreatePartyExternalReferenceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar eller uppdaterar en extern referens utifrån källsystem och källnyckel.
        /// </summary>
        Task<Result<PartyExternalReferenceResponse>> UpsertAsync(Guid partyId, CreatePartyExternalReferenceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar metadata för en befintlig extern referens.
        /// </summary>
        Task<Result<PartyExternalReferenceResponse>> UpdateAsync(Guid externalReferenceId, UpdatePartyExternalReferenceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en felaktig extern referens.
        /// </summary>
        Task<Result> DeleteAsync(Guid externalReferenceId, CancellationToken cancellationToken = default);
    }
}