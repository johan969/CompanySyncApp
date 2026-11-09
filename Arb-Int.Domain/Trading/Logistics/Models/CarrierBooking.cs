using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Economy;

namespace Arb_Int.Domain.Trading
{
    public class CarrierBooking : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public Guid CarrierBusinessRelationshipId { get; set; }
        public int? ExternalSystemId { get; set; }
        public CarrierBookingStatus Status { get; set; }
        public string? BookingReference { get; set; }
        public string? ExternalBookingId { get; set; }
        public DateTimeOffset? RequestedAtUtc { get; set; }
        public DateTimeOffset? ConfirmedAtUtc { get; set; }
        public decimal? CostAmount { get; set; }
        public int? CostCurrencyId { get; set; }
        public string? ServiceLevel { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public Shipment Shipment { get; set; } = null!;
        public ShipmentLeg? ShipmentLeg { get; set; }
        public BusinessRelationship CarrierBusinessRelationship { get; set; } = null!;
        public ExternalSystem? ExternalSystem { get; set; }
        public Currency? CostCurrency { get; set; }
    }
}
