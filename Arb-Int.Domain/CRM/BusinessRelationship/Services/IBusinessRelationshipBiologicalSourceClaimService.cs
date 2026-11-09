using FluentResults;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Hanterar biologiska source-claims med mapping, verifiering, evidens och geografi.
    /// </summary>
    public interface IBusinessRelationshipBiologicalSourceClaimService
    {
        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar claims över samtliga affärsrelationer
        /// som ägs av organisationen.
        /// </summary>
        Task<Result<PagedResult<BiologicalSourceClaimListResponse>>> QueryByOrganizationAsync(Guid owningOrganizationId, BiologicalSourceClaimQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar claims för en affärsrelation.
        /// </summary>
        Task<Result<PagedResult<BiologicalSourceClaimListResponse>>> QueryAsync(Guid businessRelationshipId, BiologicalSourceClaimQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar ett komplett claim med evidens och regioner.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> GetByIdAsync(Guid claimId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar ett nytt biologiskt source-claim.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> CreateAsync(Guid businessRelationshipId, CreateBiologicalSourceClaimRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar claimens deklarerade grunddata.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> UpdateAsync(Guid claimId, UpdateBiologicalSourceClaimRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ändrar claimens mapping till en intern biologisk källa.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> SetMappingAsync(Guid claimId, SetBiologicalSourceClaimMappingRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ändrar claimens verifieringsstatus.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> SetVerificationAsync(Guid claimId, SetBiologicalSourceClaimVerificationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lägger till evidens på claimen.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> AddEvidenceAsync(Guid claimId, CreateBiologicalSourceClaimEvidenceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar en evidenspost och returnerar hela claimen.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> UpdateEvidenceAsync(Guid evidenceId, UpdateBiologicalSourceClaimEvidenceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en evidenspost och returnerar hela claimen.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> DeleteEvidenceAsync(Guid evidenceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lägger till en geografisk plats på claimen.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> AddRegionAsync(Guid claimId, CreateBiologicalSourceClaimRegionRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar en geografisk claim-koppling.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> UpdateRegionAsync(Guid regionId, UpdateBiologicalSourceClaimRegionRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en geografisk claim-koppling.
        /// </summary>
        Task<Result<BiologicalSourceClaimResponse>> DeleteRegionAsync(Guid regionId, CancellationToken cancellationToken = default);
    }
}