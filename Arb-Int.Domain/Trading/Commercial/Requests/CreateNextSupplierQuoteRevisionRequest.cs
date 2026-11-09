namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att skapa nästa supplier-quote-revision.
/// </summary>
public sealed class CreateNextSupplierQuoteRevisionRequest
{
    /// <summary>
    /// Concurrency-token för quote-roten.
    /// </summary>
    public uint SupplierQuoteRowVersion { get; init; }
}