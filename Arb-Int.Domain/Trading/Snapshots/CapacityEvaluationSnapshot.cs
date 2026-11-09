namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller ett fryst capacity-resultat.
/// </summary>
public sealed class CapacityEvaluationSnapshot
{
    /// <summary>
    /// Identifierar eventuell allocation genom dess sekvensnummer.
    /// </summary>
    public int? AllocationSequence { get; init; }

    /// <summary>
    /// Anger kapacitetsutfallet.
    /// </summary>
    public CapacityEvaluationOutcome Outcome { get; init; }

    /// <summary>
    /// Anger efterfrågad kvantitet.
    /// </summary>
    public decimal RequestedQuantity { get; init; }

    /// <summary>
    /// Anger tillgänglig kvantitet när den är känd.
    /// </summary>
    public decimal? AvailableQuantity { get; init; }

    /// <summary>
    /// Anger använd kvantitet.
    /// </summary>
    public decimal AllocatedQuantity { get; init; }

    /// <summary>
    /// Identifierar kapacitetsvärdenas enhet.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger tidigaste kända tillgänglighet.
    /// </summary>
    public DateTimeOffset? AvailabilityFromUtc { get; init; }

    /// <summary>
    /// Anger senaste kända tillgänglighet.
    /// </summary>
    public DateTimeOffset? AvailabilityToUtc { get; init; }

    /// <summary>
    /// Förklarar kapacitetsresultatet.
    /// </summary>
    public string? Explanation { get; init; }
}