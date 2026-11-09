using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class BuyerSalesOrderDeliveryLine : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid BuyerSalesOrderDeliveryId { get; set; }
        public Guid BuyerSalesOrderId { get; set; }
        public Guid BuyerSalesOrderLineId { get; set; }
        public decimal PlannedQuantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerSalesOrderDelivery BuyerSalesOrderDelivery { get; set; } = null!;
        public BuyerSalesOrderLine BuyerSalesOrderLine { get; set; } = null!;
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
    }
}
