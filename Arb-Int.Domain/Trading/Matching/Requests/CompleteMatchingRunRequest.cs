namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att slutföra en matching run.
/// </summary>
public sealed class CompleteMatchingRunRequest
{
    /// <summary>
    /// Concurrency-token för matchningskörningen.
    /// </summary>
    public uint RowVersion { get; init; }
}