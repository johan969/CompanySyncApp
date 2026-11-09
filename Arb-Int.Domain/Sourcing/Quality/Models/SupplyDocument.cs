using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Trading;

namespace Arb_Int.Domain.Sourcing
{
    public class SupplyDocument : IEntity<Guid>, INamed, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public int DocumentTypeId { get; set; }
        public Guid? SubjectBusinessRelationshipId { get; set; }
        public Guid? SupplierMaterialOfferingId { get; set; }
        public Guid? SupplierProcessCapabilityId { get; set; }
        public Guid? InventoryLotId { get; set; }
        public Guid? ShipmentId { get; set; }
        public required string Name { get; set; }
        public DocumentStatus Status { get; set; }
        public Guid? ReferenceEntryId { get; set; }
        public string? StorageProvider { get; set; }
        public string? StorageKey { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public string? ContentHash { get; set; }
        public string? ExternalDocumentId { get; set; }
        public DateTimeOffset? IssuedAtUtc { get; set; }
        public DateTimeOffset? ValidFromUtc { get; set; }
        public DateTimeOffset? ValidToUtc { get; set; }
        public DateTimeOffset? VerifiedAtUtc { get; set; }
        public Guid? VerifiedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public DocumentType DocumentType { get; set; } = null!;
        public BusinessRelationship? SubjectBusinessRelationship { get; set; }
        public SupplierMaterialOffering? SupplierMaterialOffering { get; set; }
        public SupplierProcessCapability? SupplierProcessCapability { get; set; }
        public InventoryLot? InventoryLot { get; set; }
        public Shipment? Shipment { get; set; }
        public ReferenceEntry? ReferenceEntry { get; set; }
    }
}
