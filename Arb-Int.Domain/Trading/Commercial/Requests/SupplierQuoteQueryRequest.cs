namespace Arb_Int.Domain.Trading;

/// <summary>
/// Anger filtrering, sortering och paginering för SupplierQuote.
/// </summary>
public sealed class SupplierQuoteQueryRequest
{
    /// <summary>
    /// Fritext som matchas mot quote-nummer, namn, supplierreferens, suppliernamn och RFQ-nummer.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Begränsar resultatet till en specifik supplier-relation.
    /// </summary>
    public Guid? SupplierBusinessRelationshipId { get; init; }

    /// <summary>
    /// Begränsar resultatet till en specifik supplier-part.
    /// </summary>
    public Guid? SupplierPartyId { get; init; }

    /// <summary>
    /// Begränsar resultatet till en specifik BuyerRfq.
    /// </summary>
    public Guid? BuyerRfqId { get; init; }

    /// <summary>
    /// Begränsar resultatet till en specifik BuyerRfqRevision.
    /// </summary>
    public Guid? BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Begränsar resultatet till en quote-status.
    /// </summary>
    public SupplierQuoteStatus? Status { get; init; }

    /// <summary>
    /// Begränsar resultatet till en ansvarig användare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Anger sorteringsfält.
    /// </summary>
    public SupplierQuoteSortField SortBy { get; init; } = SupplierQuoteSortField.QuoteNumber;

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
/// Anger tillgängliga sorteringsfält för SupplierQuote.
/// </summary>
public enum SupplierQuoteSortField
{
    QuoteNumber = 0,
    Name = 1,
    SupplierName = 2,
    ReceivedAtUtc = 3,
    CreateDateUtc = 4,
    LatestRevisionNumber = 5
}
