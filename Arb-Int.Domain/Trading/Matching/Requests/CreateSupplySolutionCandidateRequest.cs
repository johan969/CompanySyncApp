using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller grunddata för en lösningskandidat som genereras av matchningsmotorn.
/// </summary>
public sealed class CreateSupplySolutionCandidateRequest
{
    /// <summary>
    /// Kandidatens löpnummer inom matchningskörningen.
    /// </summary>
    public int CandidateNumber { get; init; }

    /// <summary>
    /// Kandidatens visningsnamn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver hur kandidaten uppfyller RFQ:n.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger om lösningen använder en supplier, supplier-pool, transformationskedja eller hybrid.
    /// </summary>
    public SupplySolutionKind Kind { get; init; }

    /// <summary>
    /// Samlad verifieringsstatus från den information som hittills finns tillgänglig.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Anger kandidatens beräknade täckningsgrad i procent.
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
    /// Identifierar valutan för uppskattade kostnader.
    /// </summary>
    public int? EstimatedCostCurrencyId { get; init; }

    /// <summary>
    /// Uppskattad total ledtid i dagar.
    /// </summary>
    public int? EstimatedLeadTimeDays { get; init; }

    /// <summary>
    /// Tidpunkt efter vilken det genererade lösningsförslaget inte längre bör användas.
    /// </summary>
    public DateTimeOffset? ExpiresAtUtc { get; init; }
}