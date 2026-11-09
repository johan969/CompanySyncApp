namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency- och mottagningsdata när en supplier-revision fryses.
/// </summary>
public sealed class ReceiveSupplierQuoteRevisionRequest
{
    /// <summary>
    /// Anger faktisk mottagningstid eller null för aktuell UTC-tid.
    /// </summary>
    public DateTimeOffset? ReceivedAtUtc { get; init; }

    /// <summary>
    /// Concurrency-token för quote-roten.
    /// </summary>
    public uint SupplierQuoteRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för revisionen.
    /// </summary>
    public uint SupplierQuoteRevisionRowVersion { get; init; }
}