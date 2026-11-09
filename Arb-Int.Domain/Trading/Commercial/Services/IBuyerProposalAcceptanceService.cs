using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar buyer acceptance och efterföljande intern approval utan att ändra det utfärdade proposal-snapshotet.
/// </summary>
public interface IBuyerProposalAcceptanceService
{
    /// <summary>
    /// Hämtar en acceptance med accepterade quantities.
    /// </summary>
    Task<Result<BuyerProposalAcceptanceResponse>> GetByIdAsync(Guid buyerProposalAcceptanceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar acceptance-historiken för en proposal-revision.
    /// </summary>
    Task<Result<IReadOnlyList<BuyerProposalAcceptanceResponse>>> GetByRevisionAsync(Guid buyerProposalRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registrerar en buyer acceptance som PendingInternalApproval.
    /// </summary>
    Task<Result<BuyerProposalAcceptanceResponse>> RecordAsync(Guid buyerProposalOptionId, RecordBuyerProposalAcceptanceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Godkänner en pending acceptance och räknar om proposalets cumulative acceptance-state.
    /// </summary>
    Task<Result<BuyerProposalResponse>> ApproveAsync(Guid buyerProposalAcceptanceId, ApproveBuyerProposalAcceptanceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Avslår en pending acceptance utan att ändra proposalets tidigare approved quantities.
    /// </summary>
    Task<Result<BuyerProposalAcceptanceResponse>> RejectAsync(Guid buyerProposalAcceptanceId, RejectBuyerProposalAcceptanceRequest request, CancellationToken cancellationToken = default);
}