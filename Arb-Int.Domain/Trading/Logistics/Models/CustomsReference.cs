using Arb_Int.Domain.Base;
using Arb_Int.Domain.Location;

namespace Arb_Int.Domain.Trading
{
    public class CustomsReference : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public CustomsReferenceKind Kind { get; set; }
        public required string ReferenceNumber { get; set; }
        public Guid? CountryId { get; set; }
        public string? CommodityCode { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? IssuedAtUtc { get; set; }
        public DateTimeOffset? ClearedAtUtc { get; set; }
        public Guid? ShipmentDocumentId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public Shipment Shipment { get; set; } = null!;
        public Country? Country { get; set; }
        public ShipmentDocument? ShipmentDocument { get; set; }
    }
}
