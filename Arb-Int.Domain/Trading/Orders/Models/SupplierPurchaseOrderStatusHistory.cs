using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class SupplierPurchaseOrderStatusHistory : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplierPurchaseOrderId { get; set; }
        public PurchaseOrderStatus FromStatus { get; set; }
        public PurchaseOrderStatus ToStatus { get; set; }
        public DateTimeOffset ChangedAtUtc { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public required string Reason { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplierPurchaseOrder SupplierPurchaseOrder { get; set; } = null!;
    }
}
