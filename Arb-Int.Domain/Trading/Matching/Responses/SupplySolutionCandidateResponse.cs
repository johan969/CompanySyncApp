using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller hela persistenta resultatbilden för en supply-solution-candidate.
/// </summary>
public sealed class SupplySolutionCandidateResponse
{
    /// <summary>
    /// Identifierar kandidaten.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar matchningskörningen.
    /// </summary>
    public Guid MatchingRunId { get; init; }

    /// <summary>
    /// Matchningskörningens status.
    /// </summary>
    public MatchingRunStatus MatchingRunStatus { get; init; }

    /// <summary>
    /// Kandidatens nummer.
    /// </summary>
    public int CandidateNumber { get; init; }

    /// <summary>
    /// Kandidatens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Kandidatens beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger lösningstypen.
    /// </summary>
    public SupplySolutionKind Kind { get; init; }

    /// <summary>
    /// Anger maskinellt matchningsutfall.
    /// </summary>
    public SupplySolutionOutcome Outcome { get; init; }

    /// <summary>
    /// Anger kandidatens workflow-status.
    /// </summary>
    public SupplySolutionStatus Status { get; init; }

    /// <summary>
    /// Anger samlad verifieringsstatus.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Kandidatens totala score.
    /// </summary>
    public decimal TotalScore { get; init; }

    /// <summary>
    /// Kandidatens coverage.
    /// </summary>
    public decimal CoveragePercent { get; init; }

    /// <summary>
    /// Uppskattad inköpskostnad.
    /// </summary>
    public decimal? EstimatedPurchaseCost { get; init; }

    /// <summary>
    /// Uppskattad logistikkostnad.
    /// </summary>
    public decimal? EstimatedLogisticsCost { get; init; }

    /// <summary>
    /// Uppskattad bearbetningskostnad.
    /// </summary>
    public decimal? EstimatedProcessingCost { get; init; }

    /// <summary>
    /// Uppskattad total kostnad.
    /// </summary>
    public decimal? EstimatedTotalCost { get; init; }

    /// <summary>
    /// Identifierar kostnadsvalutan.
    /// </summary>
    public int? EstimatedCostCurrencyId { get; init; }

    /// <summary>
    /// Uppskattad ledtid i dagar.
    /// </summary>
    public int? EstimatedLeadTimeDays { get; init; }

    /// <summary>
    /// Anger om någon utvärdering resulterat i hard fail.
    /// </summary>
    public bool ContainsHardFail { get; init; }

    /// <summary>
    /// Immutable kandidat-snapshot när kandidaten senare har finaliserats.
    /// </summary>
    public string? SnapshotJson { get; init; }

    /// <summary>
    /// SHA-256 för kandidat-snapshotet.
    /// </summary>
    public string? SnapshotHash { get; init; }

    /// <summary>
    /// Tidpunkt då kandidaten upphör att vara aktuell.
    /// </summary>
    public DateTimeOffset? ExpiresAtUtc { get; init; }

    /// <summary>
    /// Solution lines som täcker run-lines.
    /// </summary>
    public required IReadOnlyList<SupplySolutionLineResponse> Lines { get; init; }

    /// <summary>
    /// Process-, supply- och logistiksteg som bygger kandidatens lösningskedja.
    /// </summary>
    public required IReadOnlyList<SupplySolutionStepResponse> Steps { get; init; }

    /// <summary>
    /// Riktade dependencies mellan kandidatens solution steps.
    /// </summary>
    public required IReadOnlyList<SupplySolutionStepDependencyResponse> StepDependencies { get; init; }

    /// <summary>
    /// Antal requirement evaluations.
    /// </summary>
    public int RequirementEvaluationCount { get; init; }

    /// <summary>
    /// Antal score contributions.
    /// </summary>
    public int ScoreContributionCount { get; init; }

    /// <summary>
    /// Antal moderation decisions.
    /// </summary>
    public int ModerationDecisionCount { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }

    /// <summary>
    /// Requirement evaluations som förklarar kandidatens matchningsutfall.
    /// </summary>
    public required IReadOnlyList<RequirementEvaluationResponse> RequirementEvaluations { get; init; }

    /// <summary>
    /// Score contributions som förklarar kandidatens totala score.
    /// </summary>
    public required IReadOnlyList<ScoreContributionResponse> ScoreContributions { get; init; }

    /// <summary>
    /// Anger kandidatens maskinella eller senast explicit override:ade outcome.
    /// </summary>
    public SupplySolutionOutcome EffectiveOutcome { get; init; }

    /// <summary>
    /// Anger kandidatens maskinella eller senast explicit override:ade score.
    /// </summary>
    public decimal EffectiveScore { get; init; }

    /// <summary>
    /// Innehåller moderation-historiken med senaste beslut först.
    /// </summary>
    public required IReadOnlyList<MatchingModerationDecisionResponse> ModerationDecisions { get; init; }
}