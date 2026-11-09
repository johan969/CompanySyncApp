using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class BuyerSalesOrderLine : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid BuyerSalesOrderId { get; set; }
        public Guid BuyerProposalAcceptanceId { get; set; }
        public Guid BuyerProposalAcceptanceLineId { get; set; }
        public Guid? MaterialId { get; set; }
        public Guid? ProductId { get; set; }
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
        public string? BuyerLineReference { get; set; }
        public DateTimeOffset? RequiredDeliveryFromUtc { get; set; }
        public DateTimeOffset? RequiredDeliveryToUtc { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerSalesOrder BuyerSalesOrder { get; set; } = null!;
        public BuyerProposalAcceptanceLine BuyerProposalAcceptanceLine { get; set; } = null!;
        public Material? Material { get; set; }
        public Product? Product { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
        public ICollection<BuyerSalesOrderDeliveryLine> DeliveryLines { get; set; } = [];
        public ICollection<SalesPurchaseAllocation> PurchaseAllocations { get; set; } = [];
    }
}
