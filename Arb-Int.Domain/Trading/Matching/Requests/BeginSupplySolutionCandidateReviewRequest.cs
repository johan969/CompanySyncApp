namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att påbörja manuell granskning av en finaliserad candidate.
/// </summary>
public sealed class BeginSupplySolutionCandidateReviewRequest
{
    /// <summary>
    /// Concurrency-token från senast lästa candidate.
    /// </summary>
    public uint RowVersion { get; init; }
}