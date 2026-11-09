namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en buyer-facing proposal-line och dess interna provenance-referenser.
/// </summary>
public sealed class BuyerProposalLineResponse
{
    /// <summary>
    /// Identifierar proposal-raden.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar optionen.
    /// </summary>
    public Guid BuyerProposalOptionId { get; init; }

    /// <summary>
    /// Identifierar RFQ-raden.
    /// </summary>
    public Guid BuyerRfqLineId { get; init; }

    /// <summary>
    /// Identifierar optional supply-solution-line.
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
    /// Anger net amount.
    /// </summary>
    public decimal NetAmount { get; init; }

    /// <summary>
    /// Anger eventuell skattesats.
    /// </summary>
    public decimal? TaxPercent { get; init; }

    /// <summary>
    /// Anger eventuellt skattebelopp.
    /// </summary>
    public decimal? TaxAmount { get; init; }

    /// <summary>
    /// Anger gross amount.
    /// </summary>
    public decimal GrossAmount { get; init; }

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
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}