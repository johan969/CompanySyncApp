using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar RFQ-rader och deras materialalternativ inom draft-revisioner.
/// </summary>
public interface IBuyerRfqLineService
{
    /// <summary>
    /// Hämtar samtliga rader i en RFQ-revision.
    /// </summary>
    Task<Result<IReadOnlyList<BuyerRfqLineResponse>>> GetByRevisionAsync(Guid buyerRfqRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar en enskild RFQ-rad.
    /// </summary>
    Task<Result<BuyerRfqLineResponse>> GetByIdAsync(Guid buyerRfqLineId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar en RFQ-rad från grunden i en draft-revision.
    /// </summary>
    Task<Result<BuyerRfqLineResponse>> CreateAsync(Guid buyerRfqRevisionId, CreateBuyerRfqLineRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar en RFQ-rad från ett BuyerDemandScope som tillhör samma demand och buyer-relation.
    /// </summary>
    Task<Result<BuyerRfqLineResponse>> CreateFromDemandScopeAsync(Guid buyerRfqRevisionId, Guid buyerDemandScopeId, CreateBuyerRfqLineFromDemandScopeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar en RFQ-rad i en draft-revision.
    /// </summary>
    Task<Result<BuyerRfqLineResponse>> UpdateAsync(Guid buyerRfqLineId, UpdateBuyerRfqLineRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en RFQ-rad som inte används av krav eller leveransallokeringar.
    /// </summary>
    Task<Result> DeleteAsync(Guid buyerRfqLineId, uint rowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lägger till ett materialalternativ på en RFQ-rad.
    /// </summary>
    Task<Result<BuyerRfqLineResponse>> AddMaterialOptionAsync(Guid buyerRfqLineId, CreateBuyerRfqLineMaterialOptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett materialalternativ på en RFQ-rad.
    /// </summary>
    Task<Result<BuyerRfqLineResponse>> UpdateMaterialOptionAsync(Guid materialOptionId, UpdateBuyerRfqLineMaterialOptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort ett materialalternativ från en RFQ-rad.
    /// </summary>
    Task<Result<BuyerRfqLineResponse>> DeleteMaterialOptionAsync(Guid materialOptionId, uint rowVersion, CancellationToken cancellationToken = default);
}