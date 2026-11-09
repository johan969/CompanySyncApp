namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara värden för en proposal-line i en Draft-option.
/// </summary>
public sealed class UpdateBuyerProposalLineRequest
{
    /// <summary>
    /// Identifierar RFQ-raden som proposal-raden svarar mot.
    /// </summary>
    public Guid BuyerRfqLineId { get; init; }

    /// <summary>
    /// Identifierar eventuell solution-line bakom erbjudandet.
    /// </summary>
    public Guid? SupplySolutionLineId { get; init; }

    /// <summary>
    /// Identifierar eventuellt material.
    /// </summary>
    public Guid? MaterialId { get; init; }

    /// <summary>
    /// Identifierar eventuell produkt.
    /// </summary>
    public Guid? ProductId { get; init; }

    /// <summary>
    /// Anger radnumret.
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
    /// Innehåller buyer-facing anteckningar.
    /// </summary>
    public string? BuyerFacingNotes { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa line.
    /// </summary>
    public uint RowVersion { get; init; }
}