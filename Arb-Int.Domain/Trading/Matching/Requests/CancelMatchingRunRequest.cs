namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att avbryta en matchningskörning.
/// </summary>
public sealed class CancelMatchingRunRequest
{
    /// <summary>
    /// Concurrency-token från senast lästa körning.
    /// </summary>
    public uint RowVersion { get; init; }
}