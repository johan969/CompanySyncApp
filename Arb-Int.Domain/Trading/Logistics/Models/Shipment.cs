using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Trading
{
    public class Shipment : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public required string ShipmentNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public ShipmentStatus Status { get; set; }
        public ShipmentMode Mode { get; set; }
        public bool IsConsolidated { get; set; }
        public Guid? FreightForwarderBusinessRelationshipId { get; set; }
        public Guid? OriginLocationId { get; set; }
        public Guid? DestinationLocationId { get; set; }
        public Guid? OriginAddressId { get; set; }
        public Guid? DestinationAddressId { get; set; }
        public DateTimeOffset? PlannedPickupAtUtc { get; set; }
        public DateTimeOffset? ActualPickupAtUtc { get; set; }
        public DateTimeOffset? PlannedDeliveryAtUtc { get; set; }
        public DateTimeOffset? ActualDeliveryAtUtc { get; set; }
        public string? MasterTrackingNumber { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship? FreightForwarderBusinessRelationship { get; set; }
        public GeographicLocation? OriginLocation { get; set; }
        public GeographicLocation? DestinationLocation { get; set; }
        public Address? OriginAddress { get; set; }
        public Address? DestinationAddress { get; set; }
        public ICollection<ShipmentLeg> Legs { get; set; } = [];
        public ICollection<ShipmentOrderLineAllocation> OrderLineAllocations { get; set; } = [];
        public ICollection<ShipmentLotAllocation> LotAllocations { get; set; } = [];
        public ICollection<ShipmentMilestone> Milestones { get; set; } = [];
        public ICollection<ShipmentTrackingEvent> TrackingEvents { get; set; } = [];
        public ICollection<CarrierBooking> CarrierBookings { get; set; } = [];
        public ICollection<ShipmentDocument> Documents { get; set; } = [];
        public ICollection<CustomsReference> CustomsReferences { get; set; } = [];
    }
}
