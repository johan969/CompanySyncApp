namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en buyer-facing proposal-line.
/// </summary>
public sealed class CreateBuyerProposalLineRequest
{
    /// <summary>
    /// Identifierar RFQ-raden som proposal-raden svarar mot.
    /// </summary>
    public Guid BuyerRfqLineId { get; init; }

    /// <summary>
    /// Identifierar eventuell solution-line bakom det buyer-facing erbjudandet.
    /// </summary>
    public Guid? SupplySolutionLineId { get; init; }

    /// <summary>
    /// Identifierar eventuellt material som erbjuds.
    /// </summary>
    public Guid? MaterialId { get; init; }

    /// <summary>
    /// Identifierar eventuell produkt som erbjuds.
    /// </summary>
    public Guid? ProductId { get; init; }

    /// <summary>
    /// Anger radnumret inom optionen.
    /// </summary>
    public int LineNumber { get; init; }

    /// <summary>
    /// Anger buyer-facing namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger buyer-facing beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger erbjuden kvantitet.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetsenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger buyer-facing pris per enhet.
    /// </summary>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// Anger eventuell skattesats i procent.
    /// </summary>
    public decimal? TaxPercent { get; init; }

    /// <summary>
    /// Anger tidigaste beräknade leverans.
    /// </summary>
    public DateTimeOffset? EstimatedDeliveryFromUtc { get; init; }

    /// <summary>
    /// Anger senaste beräknade leverans.
    /// </summary>
    public DateTimeOffset? EstimatedDeliveryToUtc { get; init; }

    /// <summary>
    /// Innehåller buyer-facing anteckningar på raden.
    /// </summary>
    public string? BuyerFacingNotes { get; init; }
}