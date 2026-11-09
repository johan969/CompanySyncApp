namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller revision och concurrency-data för att avslå en supplier quote internt.
/// </summary>
public sealed class RejectSupplierQuoteRequest
{
    /// <summary>
    /// Identifierar revisionen som avslås.
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