using FluentResults;

namespace Arb_Int.Domain.Crm;

/// <summary>
/// Hanterar BuyerDemand, state history, scopes och evidence.
/// </summary>
public interface IBuyerDemandService
{
    /// <summary>
    /// Hämtar en BuyerDemand.
    /// </summary>
    Task<Result<BuyerDemandResponse>> GetByIdAsync(Guid buyerDemandId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Söker BuyerDemand för en intern organisation.
    /// </summary>
    Task<Result<PagedResult<BuyerDemandListResponse>>> QueryAsync(Guid owningOrganizationId, BuyerDemandQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar state history för en BuyerDemand.
    /// </summary>
    Task<Result<IReadOnlyList<BuyerDemandStateHistoryResponse>>> GetStateHistoryAsync(Guid buyerDemandId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar en BuyerDemand på en buyer-relation.
    /// </summary>
    Task<Result<BuyerDemandResponse>> CreateAsync(Guid businessRelationshipId, CreateBuyerDemandRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar BuyerDemand-metadata utan att ändra state.
    /// </summary>
    Task<Result<BuyerDemandResponse>> UpdateAsync(Guid buyerDemandId, UpdateBuyerDemandRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ändrar affärsmässigt state och skriver state history atomiskt.
    /// </summary>
    Task<Result<BuyerDemandResponse>> ChangeStateAsync(Guid buyerDemandId, ChangeBuyerDemandStateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lägger till ett matchningsbart scope.
    /// </summary>
    Task<Result<BuyerDemandResponse>> AddScopeAsync(Guid buyerDemandId, CreateBuyerDemandScopeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett BuyerDemand-scope.
    /// </summary>
    Task<Result<BuyerDemandResponse>> UpdateScopeAsync(Guid buyerDemandScopeId, UpdateBuyerDemandScopeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort ett felaktigt BuyerDemand-scope.
    /// </summary>
    Task<Result<BuyerDemandResponse>> DeleteScopeAsync(Guid buyerDemandScopeId, uint rowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lägger till evidens på en BuyerDemand.
    /// </summary>
    Task<Result<BuyerDemandResponse>> AddEvidenceAsync(Guid buyerDemandId, CreateBuyerDemandEvidenceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar evidens på en BuyerDemand.
    /// </summary>
    Task<Result<BuyerDemandResponse>> UpdateEvidenceAsync(Guid buyerDemandEvidenceId, UpdateBuyerDemandEvidenceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en felaktig evidenspost.
    /// </summary>
    Task<Result<BuyerDemandResponse>> DeleteEvidenceAsync(Guid buyerDemandEvidenceId, uint rowVersion, CancellationToken cancellationToken = default);
}