namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att publicera en matchningsregelversion.
/// </summary>
public sealed class PublishMatchingRuleSetVersionRequest
{
    /// <summary>
    /// Identifierar eventuell användare som publicerar versionen.
    /// </summary>
    public Guid? PublishedByUserId { get; init; }

    /// <summary>
    /// Concurrency-token för regelverksroten.
    /// </summary>
    public uint MatchingRuleSetRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för regelversionen.
    /// </summary>
    public uint MatchingRuleSetVersionRowVersion { get; init; }
}