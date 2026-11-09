namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att avbryta en draft-revision.
/// </summary>
public sealed class CancelBuyerRfqRevisionRequest
{
    /// <summary>
    /// Concurrency-token från senast lästa revision.
    /// </summary>
    public uint RowVersion { get; init; }
}