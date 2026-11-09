using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar buyer-proposal-roten, revisionshistorik och immutable issue-lifecycle.
/// </summary>
public interface IBuyerProposalService
{
    /// <summary>
    /// Hämtar proposalet med hela revisionshistoriken.
    /// </summary>
    Task<Result<BuyerProposalResponse>> GetByIdAsync(Guid buyerProposalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Söker buyer proposals för en intern organisation.
    /// </summary>
    Task<Result<PagedResult<BuyerProposalListResponse>>> QueryAsync(Guid owningOrganizationId, BuyerProposalQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar en specifik proposal-revision.
    /// </summary>
    Task<Result<BuyerProposalRevisionResponse>> GetRevisionByIdAsync(Guid buyerProposalRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar proposal-roten tillsammans med revision 1 som Draft.
    /// </summary>
    Task<Result<BuyerProposalResponse>> CreateAsync(Guid buyerRfqId, CreateBuyerProposalRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar metadata på proposal-roten.
    /// </summary>
    Task<Result<BuyerProposalResponse>> UpdateAsync(Guid buyerProposalId, UpdateBuyerProposalRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar villkor på en Draft proposal-revision.
    /// </summary>
    Task<Result<BuyerProposalRevisionResponse>> UpdateRevisionAsync(Guid buyerProposalRevisionId, UpdateBuyerProposalRevisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar nästa Draft-revision.
    /// </summary>
    Task<Result<BuyerProposalResponse>> CreateNextRevisionAsync(Guid buyerProposalId, CreateNextBuyerProposalRevisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kontrollerar om en Draft-revision kan utfärdas.
    /// </summary>
    Task<Result<BuyerProposalIssueReadinessResponse>> GetIssueReadinessAsync(Guid buyerProposalRevisionId, DateTimeOffset evaluatedAtUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fryser revisionens buyer-facing innehåll och markerar den som Issued.
    /// </summary>
    Task<Result<BuyerProposalResponse>> IssueRevisionAsync(Guid buyerProposalRevisionId, IssueBuyerProposalRevisionRequest request, CancellationToken cancellationToken = default);
}