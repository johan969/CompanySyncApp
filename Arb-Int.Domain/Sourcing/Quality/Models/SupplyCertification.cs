using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Trading;

namespace Arb_Int.Domain.Sourcing
{
    public class SupplyCertification : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public int CertificationSchemeId { get; set; }
        public int? CertificationSchemeJurisdictionId { get; set; }
        public Guid? CertifiedBusinessRelationshipId { get; set; }
        public Guid? SupplierMaterialOfferingId { get; set; }
        public Guid? SupplierProcessCapabilityId { get; set; }
        public Guid? InventoryLotId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? CertifiedSiteLocationId { get; set; }
        public CertificationScope Scope { get; set; }
        public CertificationStatus Status { get; set; }
        public string? CertificateNumber { get; set; }
        public string? CertificateHolderName { get; set; }
        public string? IssuerName { get; set; }
        public Guid? IssuerBusinessRelationshipId { get; set; }
        public string? CertifiedScopeText { get; set; }
        public DateTimeOffset? ValidFromUtc { get; set; }
        public DateTimeOffset? ValidToUtc { get; set; }
        public Guid? SupplyDocumentId { get; set; }
        public DateTimeOffset? VerifiedAtUtc { get; set; }
        public Guid? VerifiedByUserId { get; set; }
        public string? VerificationNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public CertificationScheme CertificationScheme { get; set; } = null!;
        public CertificationSchemeJurisdiction? CertificationSchemeJurisdiction { get; set; }
        public BusinessRelationship? CertifiedBusinessRelationship { get; set; }
        public SupplierMaterialOffering? SupplierMaterialOffering { get; set; }
        public SupplierProcessCapability? SupplierProcessCapability { get; set; }
        public InventoryLot? InventoryLot { get; set; }
        public Shipment? Shipment { get; set; }
        public GeographicLocation? CertifiedSiteLocation { get; set; }
        public BusinessRelationship? IssuerBusinessRelationship { get; set; }
        public SupplyDocument? SupplyDocument { get; set; }
    }
}
