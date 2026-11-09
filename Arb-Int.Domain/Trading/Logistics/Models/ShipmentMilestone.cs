using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class ShipmentMilestone : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public int ShipmentMilestoneTypeId { get; set; }
        public ShipmentMilestoneStatus Status { get; set; }
        public DateTimeOffset? PlannedAtUtc { get; set; }
        public DateTimeOffset? ActualAtUtc { get; set; }
        public string? ExternalReference { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public Shipment Shipment { get; set; } = null!;
        public ShipmentLeg? ShipmentLeg { get; set; }
        public ShipmentMilestoneType ShipmentMilestoneType { get; set; } = null!;
    }
}
