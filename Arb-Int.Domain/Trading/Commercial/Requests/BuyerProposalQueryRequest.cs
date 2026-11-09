namespace Arb_Int.Domain.Trading;

/// <summary>
/// Anger filtrering, sortering och paginering för BuyerProposal.
/// </summary>
public sealed class BuyerProposalQueryRequest
{
    /// <summary>
    /// Fritext som matchas mot proposal-nummer, namn, buyer-namn och RFQ-nummer.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Begränsar resultatet till en specifik buyer-relation.
    /// </summary>
    public Guid? BuyerBusinessRelationshipId { get; init; }

    /// <summary>
    /// Begränsar resultatet till en specifik buyer-part.
    /// </summary>
    public Guid? BuyerPartyId { get; init; }

    /// <summary>
    /// Begränsar resultatet till en specifik BuyerRfq.
    /// </summary>
    public Guid? BuyerRfqId { get; init; }

    /// <summary>
    /// Begränsar resultatet till en proposal-status.
    /// </summary>
    public BuyerProposalStatus? Status { get; init; }

    /// <summary>
    /// Begränsar resultatet till en ansvarig användare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Anger sorteringsfält.
    /// </summary>
    public BuyerProposalSortField SortBy { get; init; } = BuyerProposalSortField.ProposalNumber;

    /// <summary>
    /// Anger sorteringsriktning.
    /// </summary>
    public SortDirection SortDirection { get; init; } = SortDirection.Ascending;

    /// <summary>
    /// Sidnummer med start på 1.
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Maximalt antal poster per sida.
    /// </summary>
    public int PageSize { get; init; } = 50;
}

/// <summary>
/// Anger tillgängliga sorteringsfält för BuyerProposal.
/// </summary>
public enum BuyerProposalSortField
{
    ProposalNumber = 0,
    Name = 1,
    BuyerName = 2,
    IssuedAtUtc = 3,
    ClosedAtUtc = 4,
    CreateDateUtc = 5,
    LatestRevisionNumber = 6
}
