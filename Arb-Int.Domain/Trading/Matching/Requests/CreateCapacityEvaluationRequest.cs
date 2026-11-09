namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller resultatet från en kapacitetsutvärdering för en solution line eller allocation.
/// </summary>
public sealed class CreateCapacityEvaluationRequest
{
    /// <summary>
    /// Identifierar eventuell allocation som utvärderingen gäller. Null innebär line-nivå.
    /// </summary>
    public Guid? SupplyAllocationId { get; init; }

    /// <summary>
    /// Anger kapacitetsutfallet.
    /// </summary>
    public CapacityEvaluationOutcome Outcome { get; init; }

    /// <summary>
    /// Kvantitet som kapacitetskontrollen försökte täcka.
    /// </summary>
    public decimal RequestedQuantity { get; init; }

    /// <summary>
    /// Kvantitet som källan uppgav eller beräknades ha tillgänglig.
    /// </summary>
    public decimal? AvailableQuantity { get; init; }

    /// <summary>
    /// Kvantitet som faktiskt användes i lösningen.
    /// </summary>
    public decimal AllocatedQuantity { get; init; }

    /// <summary>
    /// Identifierar enheten för kapacitetsvärdena.
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
    /// Förklarar hur kapacitetsutfallet beräknades.
    /// </summary>
    public string? Explanation { get; init; }
}