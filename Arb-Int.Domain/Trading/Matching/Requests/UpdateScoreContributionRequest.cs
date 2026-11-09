namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara beräkningsvärden för ett score contribution.
/// </summary>
public sealed class UpdateScoreContributionRequest
{
    /// <summary>
    /// Kategoriserar score-bidraget.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Anger vikten som användes.
    /// </summary>
    public decimal Weight { get; init; }

    /// <summary>
    /// Anger den normaliserade score som beräknades.
    /// </summary>
    public decimal NormalizedScore { get; init; }

    /// <summary>
    /// Anger bidragets slutliga poäng.
    /// </summary>
    public decimal Points { get; init; }

    /// <summary>
    /// Förklarar score-beräkningen.
    /// </summary>
    public string? Explanation { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa contribution.
    /// </summary>
    public uint RowVersion { get; init; }
}