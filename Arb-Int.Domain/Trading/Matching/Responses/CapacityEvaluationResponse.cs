namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett sparat kapacitetsresultat från matchningsmotorn.
/// </summary>
public sealed class CapacityEvaluationResponse
{
    /// <summary>
    /// Identifierar kapacitetsutvärderingen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar solution line som utvärderingen hör till.
    /// </summary>
    public Guid SupplySolutionLineId { get; init; }

    /// <summary>
    /// Identifierar eventuell allocation som utvärderingen gäller.
    /// </summary>
    public Guid? SupplyAllocationId { get; init; }

    /// <summary>
    /// Anger kapacitetsutfallet.
    /// </summary>
    public CapacityEvaluationOutcome Outcome { get; init; }

    /// <summary>
    /// Efterfrågad kvantitet i kontrollen.
    /// </summary>
    public decimal RequestedQuantity { get; init; }

    /// <summary>
    /// Tillgänglig kvantitet om den är känd.
    /// </summary>
    public decimal? AvailableQuantity { get; init; }

    /// <summary>
    /// Kvantitet som användes i lösningen.
    /// </summary>
    public decimal AllocatedQuantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetsenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Tidigaste kända tillgänglighet.
    /// </summary>
    public DateTimeOffset? AvailabilityFromUtc { get; init; }

    /// <summary>
    /// Senaste kända tillgänglighet.
    /// </summary>
    public DateTimeOffset? AvailabilityToUtc { get; init; }

    /// <summary>
    /// Förklarar resultatet.
    /// </summary>
    public string? Explanation { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}