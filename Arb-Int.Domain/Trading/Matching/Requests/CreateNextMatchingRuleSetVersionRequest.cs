namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att klona aktuell regelversion till nästa draft.
/// </summary>
public sealed class CreateNextMatchingRuleSetVersionRequest
{
    /// <summary>
    /// Concurrency-token för regelverksroten.
    /// </summary>
    public uint MatchingRuleSetRowVersion { get; init; }
}