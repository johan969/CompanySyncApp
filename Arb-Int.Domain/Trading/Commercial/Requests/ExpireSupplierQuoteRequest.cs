namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller den aktuella revisionen och concurrency-data för explicit expiration.
/// </summary>
public sealed class ExpireSupplierQuoteRequest
{
    /// <summary>
    /// Identifierar revisionen vars ValidToUtc har passerat.
    /// </summary>
    public Guid SupplierQuoteRevisionId { get; init; }

    /// <summary>
    /// Concurrency-token för quote-roten.
    /// </summary>
    public uint SupplierQuoteRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för revisionen.
    /// </summary>
    public uint SupplierQuoteRevisionRowVersion { get; init; }
}