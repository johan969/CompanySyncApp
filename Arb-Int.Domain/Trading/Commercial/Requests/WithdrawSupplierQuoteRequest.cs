namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data när suppliern drar tillbaka hela quoten.
/// </summary>
public sealed class WithdrawSupplierQuoteRequest
{
    /// <summary>
    /// Concurrency-token för quote-roten.
    /// </summary>
    public uint RowVersion { get; init; }
}