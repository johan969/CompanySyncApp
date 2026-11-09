namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att allokera en RFQ-rad till ett leveranstillfälle.
/// </summary>
public sealed class CreateBuyerRfqDeliveryLineAllocationRequest
{
    /// <summary>
    /// Identifierar RFQ-raden som ska allokeras.
    /// </summary>
    public Guid BuyerRfqLineId { get; init; }

    /// <summary>
    /// Anger kvantiteten från raden som ska levereras vid leveranstillfället.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetens enhet.
    /// </summary>
    public int QuantityUnitId { get; init; }
}