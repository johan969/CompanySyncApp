namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att finalisera en generated candidate.
/// </summary>
public sealed class FinalizeSupplySolutionCandidateRequest
{
    /// <summary>
    /// Concurrency-token för kandidaten.
    /// </summary>
    public uint SupplySolutionCandidateRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för matching run som äger kandidaten.
    /// </summary>
    public uint MatchingRunRowVersion { get; init; }
}