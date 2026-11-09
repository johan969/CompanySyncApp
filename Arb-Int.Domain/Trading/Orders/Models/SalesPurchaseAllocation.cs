using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Economy;

namespace Arb_Int.Domain.Trading
{
    public class SalesPurchaseAllocation : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid BuyerSalesOrderLineId { get; set; }
        public Guid SupplierPurchaseOrderLineId { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public decimal? AllocatedPurchaseCost { get; set; }
        public int? PurchaseCostCurrencyId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerSalesOrderLine BuyerSalesOrderLine { get; set; } = null!;
        public SupplierPurchaseOrderLine SupplierPurchaseOrderLine { get; set; } = null!;
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
        public Currency? PurchaseCostCurrency { get; set; }
    }
}
