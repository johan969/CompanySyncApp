using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class ShipmentTrackingEvent : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public int? ExternalSystemId { get; set; }
        public required string EventCode { get; set; }
        public required string EventName { get; set; }
        public DateTimeOffset OccurredAtUtc { get; set; }
        public Guid? LocationId { get; set; }
        public string? LocationText { get; set; }
        public string? ExternalEventId { get; set; }
        public string? PayloadJson { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public Shipment Shipment { get; set; } = null!;
        public ShipmentLeg? ShipmentLeg { get; set; }
        public ExternalSystem? ExternalSystem { get; set; }
        public GeographicLocation? Location { get; set; }
    }
}
