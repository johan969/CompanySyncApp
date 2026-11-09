using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class ShipmentOrderLineAllocation : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public Guid? BuyerSalesOrderLineId { get; set; }
        public Guid? SupplierPurchaseOrderLineId { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public Shipment Shipment { get; set; } = null!;
        public ShipmentLeg? ShipmentLeg { get; set; }
        public BuyerSalesOrderLine? BuyerSalesOrderLine { get; set; }
        public SupplierPurchaseOrderLine? SupplierPurchaseOrderLine { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
    }
}
