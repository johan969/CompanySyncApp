using Arb_Int.Domain.Base;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class ShipmentDocument : IEntity<Guid>, INamed, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public int DocumentTypeId { get; set; }
        public required string Name { get; set; }
        public DocumentStatus Status { get; set; }
        public string? StorageProvider { get; set; }
        public string? StorageKey { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public string? ContentHash { get; set; }
        public string? ExternalDocumentId { get; set; }
        public DateTimeOffset? IssuedAtUtc { get; set; }
        public DateTimeOffset? VerifiedAtUtc { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public Shipment Shipment { get; set; } = null!;
        public ShipmentLeg? ShipmentLeg { get; set; }
        public DocumentType DocumentType { get; set; } = null!;
    }
}
