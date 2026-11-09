namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller ett fryst line-resultat.
/// </summary>
public sealed class SupplySolutionLineSnapshot
{
    /// <summary>
    /// Identifierar den ursprungliga RFQ-raden.
    /// </summary>
    public Guid BuyerRfqLineId { get; init; }

    /// <summary>
    /// Anger RFQ-radens stabila nummer inom revisionen.
    /// </summary>
    public int BuyerRfqLineNumber { get; init; }

    /// <summary>
    /// Anger efterfrågad kvantitet.
    /// </summary>
    public decimal RequiredQuantity { get; init; }

    /// <summary>
    /// Anger täckt kvantitet.
    /// </summary>
    public decimal CoveredQuantity { get; init; }

    /// <summary>
    /// Anger kvarvarande underskott.
    /// </summary>
    public decimal ShortageQuantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetsenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger om raden är fullt täckt.
    /// </summary>
    public bool IsFullyCovered { get; init; }

    /// <summary>
    /// Anger radens täckningsgrad.
    /// </summary>
    public decimal CoveragePercent { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Innehåller supplier-allokeringarna för raden.
    /// </summary>
    public required IReadOnlyList<SupplyAllocationSnapshot> Allocations { get; init; }

    /// <summary>
    /// Innehåller kapacitetsutvärderingarna för raden.
    /// </summary>
    public required IReadOnlyList<CapacityEvaluationSnapshot> CapacityEvaluations { get; init; }
}