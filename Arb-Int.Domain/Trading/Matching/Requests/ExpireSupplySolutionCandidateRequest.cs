namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att explicit markera en candidate som utgången.
/// </summary>
public sealed class ExpireSupplySolutionCandidateRequest
{
    /// <summary>
    /// Concurrency-token från senast lästa candidate.
    /// </summary>
    public uint RowVersion { get; init; }
}