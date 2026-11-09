namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller ett fryst score-bidrag.
/// </summary>
public sealed class ScoreContributionSnapshot
{
    /// <summary>
    /// Anger eventuell evaluation som score-bidraget härstammar från.
    /// </summary>
    public int? RequirementEvaluationNumber { get; init; }

    /// <summary>
    /// Anger score-kategorin.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Anger vikten som användes.
    /// </summary>
    public decimal Weight { get; init; }

    /// <summary>
    /// Anger den normaliserade score som regelmotorn använde.
    /// </summary>
    public decimal NormalizedScore { get; init; }

    /// <summary>
    /// Anger slutligt poängbidrag.
    /// </summary>
    public decimal Points { get; init; }

    /// <summary>
    /// Förklarar score-bidraget.
    /// </summary>
    public string? Explanation { get; init; }
}