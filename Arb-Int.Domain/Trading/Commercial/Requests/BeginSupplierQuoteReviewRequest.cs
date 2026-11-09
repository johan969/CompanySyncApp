namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller revision och concurrency-data för att starta intern review av en supplier quote.
/// </summary>
public sealed class BeginSupplierQuoteReviewRequest
{
    /// <summary>
    /// Identifierar exakt den immutable revision som ska review:as.
    /// </summary>
    public Guid SupplierQuoteRevisionId { get; init; }

    /// <summary>
    /// Concurrency-token för quote-roten.
    /// </summary>
    public uint SupplierQuoteRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för revisionen som ligger till grund för review.
    /// </summary>
    public uint SupplierQuoteRevisionRowVersion { get; init; }
}