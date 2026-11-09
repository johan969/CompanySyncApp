namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett spårbart bidrag till kandidatens totala matchningsscore.
/// </summary>
public sealed class ScoreContributionResponse
{
    /// <summary>
    /// Identifierar score-bidraget.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar kandidaten.
    /// </summary>
    public Guid SupplySolutionCandidateId { get; init; }

    /// <summary>
    /// Identifierar eventuell requirement evaluation som bidraget baseras på.
    /// </summary>
    public Guid? RequirementEvaluationId { get; init; }

    /// <summary>
    /// Anger eventuell score-kategori.
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
    /// Anger score-bidragets poäng.
    /// </summary>
    public decimal Points { get; init; }

    /// <summary>
    /// Förklarar score-beräkningen.
    /// </summary>
    public string? Explanation { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}