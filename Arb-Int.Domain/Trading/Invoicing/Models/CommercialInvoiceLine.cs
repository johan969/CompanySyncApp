using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class CommercialInvoiceLine : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid CommercialInvoiceId { get; set; }
        public Guid? BuyerSalesOrderLineId { get; set; }
        public Guid? SupplierPurchaseOrderLineId { get; set; }
        public int LineNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Quantity { get; set; }
        public int? QuantityUnitId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public string? ExternalLineId { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public CommercialInvoice CommercialInvoice { get; set; } = null!;
        public BuyerSalesOrderLine? BuyerSalesOrderLine { get; set; }
        public SupplierPurchaseOrderLine? SupplierPurchaseOrderLine { get; set; }
        public UnitOfMeasure? QuantityUnit { get; set; }
    }
}
