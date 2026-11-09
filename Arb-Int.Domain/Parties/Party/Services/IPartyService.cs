using FluentResults;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar sökning, skapande och ändring av parter.
    /// </summary>
    public interface IPartyService
    {
        /// <summary>
        /// Hämtar en Party oavsett om den är en organisation eller person.
        /// </summary>
        Task<Result<PartyDetailsResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker och paginerar parter.
        /// </summary>
        Task<Result<PagedResult<PartyListResponse>>> QueryAsync(PartyQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en ny organisation.
        /// </summary>
        Task<Result<PartyDetailsResponse>> CreateOrganizationAsync(CreateOrganizationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar en befintlig organisation.
        /// </summary>
        Task<Result<PartyDetailsResponse>> UpdateOrganizationAsync(Guid id, UpdateOrganizationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en ny person.
        /// </summary>
        Task<Result<PartyDetailsResponse>> CreatePersonAsync(CreatePersonRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar en befintlig person.
        /// </summary>
        Task<Result<PartyDetailsResponse>> UpdatePersonAsync(Guid id, UpdatePersonRequest request, CancellationToken cancellationToken = default);
    }
}