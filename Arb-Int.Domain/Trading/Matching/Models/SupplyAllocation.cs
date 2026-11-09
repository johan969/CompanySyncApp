using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class SupplyAllocation : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplySolutionLineId { get; set; }
        public Guid SupplierBusinessRelationshipId { get; set; }
        public Guid SupplierMaterialOfferingId { get; set; }
        public Guid? SupplierOfferingAvailabilityId { get; set; }
        public Guid? InventoryLotId { get; set; }
        public Guid? SupplierQuoteLineId { get; set; }
        public SupplyAllocationStatus Status { get; set; }
        public int AllocationSequence { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public decimal? UnitPurchasePrice { get; set; }
        public int? PurchasePriceCurrencyId { get; set; }
        public decimal? EstimatedAdditionalCost { get; set; }
        public VerificationState AvailabilityVerificationState { get; set; }
        public VerificationState QualityVerificationState { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplySolutionLine SupplySolutionLine { get; set; } = null!;
        public BusinessRelationship SupplierBusinessRelationship { get; set; } = null!;
        public SupplierMaterialOffering SupplierMaterialOffering { get; set; } = null!;
        public SupplierOfferingAvailability? SupplierOfferingAvailability { get; set; }
        public InventoryLot? InventoryLot { get; set; }
        public SupplierQuoteLine? SupplierQuoteLine { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
        public Currency? PurchasePriceCurrency { get; set; }
    }
}
