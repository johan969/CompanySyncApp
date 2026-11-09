using System.Linq.Expressions;

namespace Arb_Int.Domain.Trading.Projections;

/// <summary>
/// Innehåller EF Core-projections för RFQ-leveranser och radallokeringar.
/// </summary>
public static class BuyerRfqDeliveryProjections
{
    /// <summary>
    /// Projekterar en delivery-line-allokering till response-objektet.
    /// </summary>
    public static Expression<Func<BuyerRfqDeliveryLineAllocation, BuyerRfqDeliveryLineAllocationResponse>> Allocation => allocation => new BuyerRfqDeliveryLineAllocationResponse
    {
        Id = allocation.Id,
        BuyerRfqRevisionId = allocation.BuyerRfqRevisionId,
        BuyerRfqDeliveryId = allocation.BuyerRfqDeliveryId,
        BuyerRfqLineId = allocation.BuyerRfqLineId,
        BuyerRfqLineNumber = allocation.BuyerRfqLine.LineNumber,
        BuyerRfqLineName = allocation.BuyerRfqLine.Name,
        Quantity = allocation.Quantity,
        QuantityUnitId = allocation.QuantityUnitId,
        RowVersion = allocation.RowVersion
    };

    /// <summary>
    /// Projekterar ett RFQ-leveranstillfälle med dess radallokeringar.
    /// </summary>
    public static Expression<Func<BuyerRfqDelivery, BuyerRfqDeliveryResponse>> Details => delivery => new BuyerRfqDeliveryResponse
    {
        Id = delivery.Id,
        BuyerRfqRevisionId = delivery.BuyerRfqRevisionId,
        BuyerRfqId = delivery.BuyerRfqRevision.BuyerRfqId,
        RevisionStatus = delivery.BuyerRfqRevision.Status,
        DeliveryNumber = delivery.DeliveryNumber,
        DestinationAddressId = delivery.DestinationAddressId,
        DestinationLocationId = delivery.DestinationLocationId,
        RequestedDeliveryFromUtc = delivery.RequestedDeliveryFromUtc,
        RequestedDeliveryToUtc = delivery.RequestedDeliveryToUtc,
        IncotermRuleId = delivery.IncotermRuleId,
        NamedPlace = delivery.NamedPlace,
        MinimumTemperatureCelsius = delivery.MinimumTemperatureCelsius,
        MaximumTemperatureCelsius = delivery.MaximumTemperatureCelsius,
        PackagingNotes = delivery.PackagingNotes,
        DeliveryNotes = delivery.DeliveryNotes,
        SortOrder = delivery.SortOrder,
        CreateDateUtc = delivery.CreateDateUtc,
        RowVersion = delivery.RowVersion,
        LineAllocations = delivery.LineAllocations
            .OrderBy(x => x.BuyerRfqLine.LineNumber)
            .ThenBy(x => x.Id)
            .Select(x => new BuyerRfqDeliveryLineAllocationResponse
            {
                Id = x.Id,
                BuyerRfqRevisionId = x.BuyerRfqRevisionId,
                BuyerRfqDeliveryId = x.BuyerRfqDeliveryId,
                BuyerRfqLineId = x.BuyerRfqLineId,
                BuyerRfqLineNumber = x.BuyerRfqLine.LineNumber,
                BuyerRfqLineName = x.BuyerRfqLine.Name,
                Quantity = x.Quantity,
                QuantityUnitId = x.QuantityUnitId,
                RowVersion = x.RowVersion
            })
            .ToList()
    };
}