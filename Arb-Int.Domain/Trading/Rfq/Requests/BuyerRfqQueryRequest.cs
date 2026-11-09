namespace Arb_Int.Domain.Trading;

/// <summary>
/// Anger filtrering, sortering och paginering för BuyerRfq.
/// </summary>
public sealed class BuyerRfqQueryRequest
{
    /// <summary>
    /// Fritext som matchas mot RFQ-nummer, namn, buyer reference och köparnamn.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Begränsar resultatet till en RFQ-status.
    /// </summary>
    public BuyerRfqStatus? Status { get; init; }

    /// <summary>
    /// Begränsar resultatet till en prioritet.
    /// </summary>
    public BuyerRfqPriority? Priority { get; init; }

    /// <summary>
    /// Begränsar resultatet till en informationssynlighet.
    /// </summary>
    public InformationVisibility? Visibility { get; init; }

    /// <summary>
    /// Begränsar resultatet till en ansvarig användare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Begränsar resultatet till en specifik köpare.
    /// </summary>
    public Guid? BuyerPartyId { get; init; }

    /// <summary>
    /// Anger sorteringsfält.
    /// </summary>
    public BuyerRfqSortField SortBy { get; init; } = BuyerRfqSortField.RfqNumber;

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
/// Anger tillgängliga sorteringsfält för BuyerRfq.
/// </summary>
public enum BuyerRfqSortField
{
    RfqNumber = 0,
    Name = 1,
    OpenedAtUtc = 2,
    CreateDateUtc = 3,
    LatestRevisionNumber = 4
}