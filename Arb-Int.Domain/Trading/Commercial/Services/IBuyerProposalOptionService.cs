using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar buyer-proposal-options i Draft-revisioner.
/// </summary>
public interface IBuyerProposalOptionService
{
    /// <summary>
    /// Skapar ett option från en ApprovedForProposal candidate.
    /// </summary>
    Task<Result<BuyerProposalRevisionResponse>> CreateAsync(Guid buyerProposalRevisionId, CreateBuyerProposalOptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar buyer-facing och intern metadata på ett Draft-option.
    /// </summary>
    Task<Result<BuyerProposalRevisionResponse>> UpdateAsync(Guid buyerProposalOptionId, UpdateBuyerProposalOptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort ett option från en Draft-revision.
    /// </summary>
    Task<Result<BuyerProposalRevisionResponse>> DeleteAsync(Guid buyerProposalOptionId, uint rowVersion, CancellationToken cancellationToken = default);
}