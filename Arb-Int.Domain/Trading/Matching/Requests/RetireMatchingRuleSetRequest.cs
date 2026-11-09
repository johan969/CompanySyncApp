namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att pensionera ett matchningsregelverk.
/// </summary>
public sealed class RetireMatchingRuleSetRequest
{
    /// <summary>
    /// Concurrency-token från senast lästa regelverk.
    /// </summary>
    public uint RowVersion { get; init; }
}