namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att köa en reproducerbar matchningskörning.
/// </summary>
public sealed class QueueMatchingRunRequest
{
    /// <summary>
    /// Identifierar regelverket som ska användas.
    /// </summary>
    public Guid MatchingRuleSetId { get; init; }

    /// <summary>
    /// Identifierar en specifik regelversion eller null för automatisk versionsupplösning.
    /// </summary>
    public Guid? MatchingRuleSetVersionId { get; init; }

    /// <summary>
    /// Identifierar eventuell användare som initierade körningen.
    /// </summary>
    public Guid? StartedByUserId { get; init; }

    /// <summary>
    /// Beskriver vad som initierade körningen, exempelvis Manual eller RfqReleased.
    /// </summary>
    public string? TriggerName { get; init; }

    /// <summary>
    /// Korrelations-ID för loggning och spårning över tjänstegränser.
    /// </summary>
    public string? CorrelationId { get; init; }
}