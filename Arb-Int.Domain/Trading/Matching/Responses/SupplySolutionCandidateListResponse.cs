using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett kompakt lösningsförslag för listor och ranking.
/// </summary>
public sealed class SupplySolutionCandidateListResponse
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
    /// Kandidatens löpnummer inom körningen.
    /// </summary>
    public int CandidateNumber { get; init; }

    /// <summary>
    /// Kandidatens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger lösningstypen.
    /// </summary>
    public SupplySolutionKind Kind { get; init; }

    /// <summary>
    /// Anger maskinellt utfall.
    /// </summary>
    public SupplySolutionOutcome Outcome { get; init; }

    /// <summary>
    /// Anger kandidatens workflow-status.
    /// </summary>
    public SupplySolutionStatus Status { get; init; }

    /// <summary>
    /// Anger kandidatens verifieringsstatus.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Kandidatens totala score.
    /// </summary>
    public decimal TotalScore { get; init; }

    /// <summary>
    /// Kandidatens coverage i procent.
    /// </summary>
    public decimal CoveragePercent { get; init; }

    /// <summary>
    /// Uppskattad total kostnad.
    /// </summary>
    public decimal? EstimatedTotalCost { get; init; }

    /// <summary>
    /// Identifierar eventuell kostnadsvaluta.
    /// </summary>
    public int? EstimatedCostCurrencyId { get; init; }

    /// <summary>
    /// Uppskattad ledtid i dagar.
    /// </summary>
    public int? EstimatedLeadTimeDays { get; init; }

    /// <summary>
    /// Anger om kandidaten innehåller ett hard fail.
    /// </summary>
    public bool ContainsHardFail { get; init; }

    /// <summary>
    /// Antal RFQ-rader som kandidaten innehåller resultat för.
    /// </summary>
    public int LineCount { get; init; }

    /// <summary>
    /// Tidpunkt då kandidaten upphör att vara aktuell.
    /// </summary>
    public DateTimeOffset? ExpiresAtUtc { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}