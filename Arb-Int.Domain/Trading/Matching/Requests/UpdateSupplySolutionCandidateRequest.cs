using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara maskinberäknade metadata för en generated candidate.
/// </summary>
public sealed class UpdateSupplySolutionCandidateRequest
{
    /// <summary>
    /// Kandidatens visningsnamn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver lösningsförslaget.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger kandidatens lösningstyp.
    /// </summary>
    public SupplySolutionKind Kind { get; init; }

    /// <summary>
    /// Samlad verifieringsstatus.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Anger kandidatens beräknade täckningsgrad.
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
    /// Identifierar valutan för kostnadsuppskattningarna.
    /// </summary>
    public int? EstimatedCostCurrencyId { get; init; }

    /// <summary>
    /// Uppskattad ledtid i dagar.
    /// </summary>
    public int? EstimatedLeadTimeDays { get; init; }

    /// <summary>
    /// Tidpunkt då kandidaten upphör att vara aktuell.
    /// </summary>
    public DateTimeOffset? ExpiresAtUtc { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa kandidat.
    /// </summary>
    public uint RowVersion { get; init; }
}