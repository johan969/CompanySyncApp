using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Sourcing
{
    public class SupplyAnalysis : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid? SupplierMaterialOfferingId { get; set; }
        public Guid? InventoryLotId { get; set; }
        public Guid? SupplyDocumentId { get; set; }
        public Guid? LaboratoryBusinessRelationshipId { get; set; }
        public AnalysisStatus Status { get; set; }
        public string? AnalysisNumber { get; set; }
        public string? SampleIdentifier { get; set; }
        public string? LaboratoryName { get; set; }
        public bool LaboratoryAccredited { get; set; }
        public string? AccreditationReference { get; set; }
        public DateTimeOffset? SampledAtUtc { get; set; }
        public DateTimeOffset? ReceivedAtUtc { get; set; }
        public DateTimeOffset? ReportedAtUtc { get; set; }
        public DateTimeOffset? VerifiedAtUtc { get; set; }
        public Guid? VerifiedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplierMaterialOffering? SupplierMaterialOffering { get; set; }
        public InventoryLot? InventoryLot { get; set; }
        public SupplyDocument? SupplyDocument { get; set; }
        public BusinessRelationship? LaboratoryBusinessRelationship { get; set; }
        public ICollection<SupplyAnalysisResult> Results { get; set; } = [];
    }
}
