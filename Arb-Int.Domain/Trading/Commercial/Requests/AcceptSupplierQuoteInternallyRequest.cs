namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller revision och concurrency-data för intern acceptans av en supplier quote.
/// </summary>
public sealed class AcceptSupplierQuoteInternallyRequest
{
    /// <summary>
    /// Identifierar den revision som accepteras.
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