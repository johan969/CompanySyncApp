namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller en radallokering i ett RFQ-snapshot.
/// </summary>
public sealed class BuyerRfqDeliveryLineAllocationSnapshot
{
    /// <summary>
    /// Anger radnumret som allokeringen gäller.
    /// </summary>
    public int BuyerRfqLineNumber { get; init; }

    /// <summary>
    /// Anger allokerad kvantitet.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetsenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }
}