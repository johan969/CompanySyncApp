namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett enskilt bidrag till en candidates matchningsscore.
/// </summary>
public sealed class CreateScoreContributionRequest
{
    /// <summary>
    /// Identifierar eventuell requirement evaluation som bidraget härstammar från.
    /// </summary>
    public Guid? RequirementEvaluationId { get; init; }

    /// <summary>
    /// Kategoriserar score-bidraget, exempelvis Quality eller Logistics.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Anger vikten som användes i scoring-regeln.
    /// </summary>
    public decimal Weight { get; init; }

    /// <summary>
    /// Anger den normaliserade score som regelmotorn beräknade.
    /// </summary>
    public decimal NormalizedScore { get; init; }

    /// <summary>
    /// Anger de slutliga poäng som bidraget gav kandidaten.
    /// </summary>
    public decimal Points { get; init; }

    /// <summary>
    /// Förklarar hur score-bidraget beräknades.
    /// </summary>
    public string? Explanation { get; init; }
}