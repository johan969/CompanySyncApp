using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class SupplierPurchaseOrderLine : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid SupplierPurchaseOrderId { get; set; }
        public Guid SupplierBusinessRelationshipId { get; set; }
        public Guid? SupplierQuoteLineId { get; set; }
        public SupplierLineKind Kind { get; set; }
        public Guid? SupplierMaterialOfferingId { get; set; }
        public Guid? SupplierProcessCapabilityId { get; set; }
        public Guid? SupplySolutionStepId { get; set; }
        public Guid? InventoryLotId { get; set; }
        public Guid? MaterialId { get; set; }
        public int LineNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal FulfilledQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public DateTimeOffset? RequiredAvailabilityFromUtc { get; set; }
        public DateTimeOffset? RequiredAvailabilityToUtc { get; set; }
        public string? SupplierLineReference { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplierPurchaseOrder SupplierPurchaseOrder { get; set; } = null!;
        public SupplierQuoteLine? SupplierQuoteLine { get; set; }
        public SupplierMaterialOffering? SupplierMaterialOffering { get; set; }
        public SupplierProcessCapability? SupplierProcessCapability { get; set; }
        public SupplySolutionStep? SupplySolutionStep { get; set; }
        public InventoryLot? InventoryLot { get; set; }
        public Material? Material { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
        public ICollection<SalesPurchaseAllocation> SalesAllocations { get; set; } = [];
    }
}
