using System.Text.Json;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller det immutable maskinella resultatet för en supply-solution-candidate.
/// </summary>
public sealed class SupplySolutionCandidateSnapshot
{
    /// <summary>
    /// Anger snapshotformatets version.
    /// </summary>
    public int SchemaVersion { get; init; } = 1;

    /// <summary>
    /// Identifierar matchningskörningen som genererade kandidaten.
    /// </summary>
    public Guid MatchingRunId { get; init; }

    /// <summary>
    /// Anger kandidatens löpnummer inom körningen.
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
    /// Anger kandidatens lösningstyp.
    /// </summary>
    public SupplySolutionKind Kind { get; init; }

    /// <summary>
    /// Anger matchningsmotorns slutliga outcome.
    /// </summary>
    public SupplySolutionOutcome Outcome { get; init; }

    /// <summary>
    /// Anger matchningsmotorns verifieringsstatus.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Anger kandidatens slutliga maskinella score.
    /// </summary>
    public decimal TotalScore { get; init; }

    /// <summary>
    /// Anger kandidatens täckningsgrad.
    /// </summary>
    public decimal CoveragePercent { get; init; }

    /// <summary>
    /// Anger uppskattad inköpskostnad.
    /// </summary>
    public decimal? EstimatedPurchaseCost { get; init; }

    /// <summary>
    /// Anger uppskattad logistikkostnad.
    /// </summary>
    public decimal? EstimatedLogisticsCost { get; init; }

    /// <summary>
    /// Anger uppskattad bearbetningskostnad.
    /// </summary>
    public decimal? EstimatedProcessingCost { get; init; }

    /// <summary>
    /// Anger uppskattad total kostnad.
    /// </summary>
    public decimal? EstimatedTotalCost { get; init; }

    /// <summary>
    /// Identifierar valutan för kostnadsuppskattningen.
    /// </summary>
    public int? EstimatedCostCurrencyId { get; init; }

    /// <summary>
    /// Anger uppskattad total ledtid.
    /// </summary>
    public int? EstimatedLeadTimeDays { get; init; }

    /// <summary>
    /// Anger om kandidaten innehåller ett utlösande hard fail.
    /// </summary>
    public bool ContainsHardFail { get; init; }

    /// <summary>
    /// Anger kandidatens eventuella giltighetstid.
    /// </summary>
    public DateTimeOffset? ExpiresAtUtc { get; init; }

    /// <summary>
    /// Innehåller kandidatens line-resultat.
    /// </summary>
    public required IReadOnlyList<SupplySolutionLineSnapshot> Lines { get; init; }

    /// <summary>
    /// Innehåller kandidatens process-, supply- och logistiksteg.
    /// </summary>
    public required IReadOnlyList<SupplySolutionStepSnapshot> Steps { get; init; }

    /// <summary>
    /// Innehåller dependency-grafens riktade edges.
    /// </summary>
    public required IReadOnlyList<SupplySolutionStepDependencySnapshot> StepDependencies { get; init; }

    /// <summary>
    /// Innehåller requirement-resultaten.
    /// </summary>
    public required IReadOnlyList<RequirementEvaluationSnapshot> RequirementEvaluations { get; init; }

    /// <summary>
    /// Innehåller kandidatens score-bidrag.
    /// </summary>
    public required IReadOnlyList<ScoreContributionSnapshot> ScoreContributions { get; init; }
}