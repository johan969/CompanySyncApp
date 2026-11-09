using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Trading
{
    public class ShipmentLeg : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public int SequenceNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public ShipmentMode Mode { get; set; }
        public ShipmentLegStatus Status { get; set; }
        public Guid? CarrierBusinessRelationshipId { get; set; }
        public Guid? FromLocationId { get; set; }
        public Guid? ToLocationId { get; set; }
        public Guid? FromAddressId { get; set; }
        public Guid? ToAddressId { get; set; }
        public DateTimeOffset? PlannedDepartureAtUtc { get; set; }
        public DateTimeOffset? ActualDepartureAtUtc { get; set; }
        public DateTimeOffset? PlannedArrivalAtUtc { get; set; }
        public DateTimeOffset? ActualArrivalAtUtc { get; set; }
        public string? TrackingNumber { get; set; }
        public string? BookingReference { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public Shipment Shipment { get; set; } = null!;
        public BusinessRelationship? CarrierBusinessRelationship { get; set; }
        public GeographicLocation? FromLocation { get; set; }
        public GeographicLocation? ToLocation { get; set; }
        public Address? FromAddress { get; set; }
        public Address? ToAddress { get; set; }
    }
}
