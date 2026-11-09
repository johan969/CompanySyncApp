namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara värden för en delivery-line-allokering.
/// </summary>
public sealed class UpdateBuyerRfqDeliveryLineAllocationRequest
{
    /// <summary>
    /// Anger kvantiteten från raden som ska levereras vid leveranstillfället.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetens enhet.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa version.
    /// </summary>
    public uint RowVersion { get; init; }
}