using FluentResults;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Hanterar affärsrelationer och deras CRM-livscykel.
    /// </summary>
    public interface IBusinessRelationshipService
    {
        /// <summary>
        /// Hämtar en affärsrelation.
        /// </summary>
        Task<Result<BusinessRelationshipResponse>> GetByIdAsync(Guid businessRelationshipId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker affärsrelationer för en ägande intern organisation.
        /// </summary>
        Task<Result<PagedResult<BusinessRelationshipListResponse>>> QueryAsync(Guid owningOrganizationId, BusinessRelationshipQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar tillgängliga typer av affärsrelationer.
        /// </summary>
        Task<Result<IReadOnlyList<BusinessRelationshipTypeResponse>>> GetTypesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar steg för en affärsrelationstyp.
        /// </summary>
        Task<Result<IReadOnlyList<BusinessRelationshipStageResponse>>> GetStagesAsync(int businessRelationshipTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar historiken över stegbyten för en affärsrelation.
        /// </summary>
        Task<Result<IReadOnlyList<BusinessRelationshipStageHistoryResponse>>> GetStageHistoryAsync(Guid businessRelationshipId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en ny affärsrelation.
        /// </summary>
        Task<Result<BusinessRelationshipResponse>> CreateAsync(Guid owningOrganizationId, CreateBusinessRelationshipRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar metadata för en affärsrelation.
        /// </summary>
        Task<Result<BusinessRelationshipResponse>> UpdateAsync(Guid businessRelationshipId, UpdateBusinessRelationshipRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Byter steg på en affärsrelation och skriver stage history.
        /// </summary>
        Task<Result<BusinessRelationshipResponse>> ChangeStageAsync(Guid businessRelationshipId, ChangeBusinessRelationshipStageRequest request, CancellationToken cancellationToken = default);
    }
}