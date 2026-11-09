namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara värden för en befintlig kapacitetsutvärdering.
/// </summary>
public sealed class UpdateCapacityEvaluationRequest
{
    /// <summary>
    /// Anger kapacitetsutfallet.
    /// </summary>
    public CapacityEvaluationOutcome Outcome { get; init; }

    /// <summary>
    /// Kvantitet som kapacitetskontrollen försökte täcka.
    /// </summary>
    public decimal RequestedQuantity { get; init; }

    /// <summary>
    /// Tillgänglig kvantitet om den är känd.
    /// </summary>
    public decimal? AvailableQuantity { get; init; }

    /// <summary>
    /// Kvantitet som faktiskt allokerades.
    /// </summary>
    public decimal AllocatedQuantity { get; init; }

    /// <summary>
    /// Identifierar enheten för värdena.
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
    /// Förklarar utvärderingsresultatet.
    /// </summary>
    public string? Explanation { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa evaluation.
    /// </summary>
    public uint RowVersion { get; init; }
}