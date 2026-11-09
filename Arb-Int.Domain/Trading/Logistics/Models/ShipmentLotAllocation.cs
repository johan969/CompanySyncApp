using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class ShipmentLotAllocation : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public Guid InventoryLotId { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public string? ContainerReference { get; set; }
        public string? SealReference { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public Shipment Shipment { get; set; } = null!;
        public ShipmentLeg? ShipmentLeg { get; set; }
        public InventoryLot InventoryLot { get; set; } = null!;
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
    }
}
