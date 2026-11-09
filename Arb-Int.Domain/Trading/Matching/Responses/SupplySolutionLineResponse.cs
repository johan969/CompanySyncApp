namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller coverage, allocations och capacity-resultat för en RFQ-rad i en kandidat.
/// </summary>
public sealed class SupplySolutionLineResponse
{
    /// <summary>
    /// Identifierar solution line.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar kandidaten.
    /// </summary>
    public Guid SupplySolutionCandidateId { get; init; }

    /// <summary>
    /// Identifierar matching run.
    /// </summary>
    public Guid MatchingRunId { get; init; }

    /// <summary>
    /// Identifierar fryst MatchingRunLine.
    /// </summary>
    public Guid MatchingRunLineId { get; init; }

    /// <summary>
    /// Identifierar ursprunglig RFQ-line.
    /// </summary>
    public Guid BuyerRfqLineId { get; init; }

    /// <summary>
    /// RFQ-radens nummer.
    /// </summary>
    public int BuyerRfqLineNumber { get; init; }

    /// <summary>
    /// Efterfrågad kvantitet från run-inputen.
    /// </summary>
    public decimal RequiredQuantity { get; init; }

    /// <summary>
    /// Kvantitet som kandidaten täcker.
    /// </summary>
    public decimal CoveredQuantity { get; init; }

    /// <summary>
    /// Kvantitet som fortfarande saknas.
    /// </summary>
    public decimal ShortageQuantity { get; init; }

    /// <summary>
    /// Identifierar line-kvantitetens enhet.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger om hela required quantity är täckt.
    /// </summary>
    public bool IsFullyCovered { get; init; }

    /// <summary>
    /// Anger täckningsgraden i procent.
    /// </summary>
    public decimal CoveragePercent { get; init; }

    /// <summary>
    /// Interna anteckningar.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Supplier-allokeringarna som stödjer line-resultatet.
    /// </summary>
    public required IReadOnlyList<SupplyAllocationResponse> Allocations { get; init; }

    /// <summary>
    /// Kapacitetsutvärderingar på line- eller allocation-nivå.
    /// </summary>
    public required IReadOnlyList<CapacityEvaluationResponse> CapacityEvaluations { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}