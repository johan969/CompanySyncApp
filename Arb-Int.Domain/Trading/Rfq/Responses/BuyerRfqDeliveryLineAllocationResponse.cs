namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en kvantitetsallokering mellan en RFQ-rad och ett leveranstillfälle.
/// </summary>
public sealed class BuyerRfqDeliveryLineAllocationResponse
{
    /// <summary>
    /// Identifierar allokeringen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar RFQ-revisionen.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Identifierar leveranstillfället.
    /// </summary>
    public Guid BuyerRfqDeliveryId { get; init; }

    /// <summary>
    /// Identifierar RFQ-raden.
    /// </summary>
    public Guid BuyerRfqLineId { get; init; }

    /// <summary>
    /// RFQ-radens nummer inom revisionen.
    /// </summary>
    public int BuyerRfqLineNumber { get; init; }

    /// <summary>
    /// RFQ-radens namn.
    /// </summary>
    public required string BuyerRfqLineName { get; init; }

    /// <summary>
    /// Anger den kvantitet från raden som ska levereras vid detta leveranstillfälle.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetens enhet.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}