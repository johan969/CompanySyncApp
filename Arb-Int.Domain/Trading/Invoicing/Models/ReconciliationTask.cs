using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class ReconciliationTask : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid? CommercialInvoiceId { get; set; }
        public Guid? BuyerSalesOrderId { get; set; }
        public Guid? SupplierPurchaseOrderId { get; set; }
        public Guid? ShipmentId { get; set; }
        public ReconciliationTaskType Type { get; set; }
        public ReconciliationTaskStatus Status { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTimeOffset? DueAtUtc { get; set; }
        public DateTimeOffset? CompletedAtUtc { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public CommercialInvoice? CommercialInvoice { get; set; }
        public BuyerSalesOrder? BuyerSalesOrder { get; set; }
        public SupplierPurchaseOrder? SupplierPurchaseOrder { get; set; }
        public Shipment? Shipment { get; set; }
    }
}
