using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar buyer-proposal-lines och automatiska monetary totals.
/// </summary>
public interface IBuyerProposalLineService
{
    /// <summary>
    /// Skapar en line i ett Draft-option.
    /// </summary>
    Task<Result<BuyerProposalRevisionResponse>> CreateAsync(Guid buyerProposalOptionId, CreateBuyerProposalLineRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar en line i ett Draft-option.
    /// </summary>
    Task<Result<BuyerProposalRevisionResponse>> UpdateAsync(Guid buyerProposalLineId, UpdateBuyerProposalLineRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en line och räknar om optionens totals.
    /// </summary>
    Task<Result<BuyerProposalRevisionResponse>> DeleteAsync(Guid buyerProposalLineId, uint rowVersion, CancellationToken cancellationToken = default);
}