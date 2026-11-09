using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Economy;

namespace Arb_Int.Domain.Trading
{
    public class CommercialInvoice : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public InvoiceDirection Direction { get; set; }
        public Guid CounterpartyBusinessRelationshipId { get; set; }
        public Guid? BuyerSalesOrderId { get; set; }
        public Guid? SupplierPurchaseOrderId { get; set; }
        public int? ExternalSystemId { get; set; }
        public ExternalSystemOwnership SystemOwnership { get; set; }
        public required string InvoiceNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public CommercialInvoiceStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public SyncStatus SyncStatus { get; set; }
        public int CurrencyId { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public DateTimeOffset? InvoiceDateUtc { get; set; }
        public DateTimeOffset? DueDateUtc { get; set; }
        public DateTimeOffset? BookedAtUtc { get; set; }
        public string? ExternalInvoiceId { get; set; }
        public string? ExternalInvoiceNumber { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship CounterpartyBusinessRelationship { get; set; } = null!;
        public BuyerSalesOrder? BuyerSalesOrder { get; set; }
        public SupplierPurchaseOrder? SupplierPurchaseOrder { get; set; }
        public ExternalSystem? ExternalSystem { get; set; }
        public Currency Currency { get; set; } = null!;
        public ICollection<CommercialInvoiceLine> Lines { get; set; } = [];
        public ICollection<ExternalAccountingReference> ExternalReferences { get; set; } = [];
        public ICollection<InvoiceSyncAttempt> SyncAttempts { get; set; } = [];
        public ICollection<PaymentStatusSnapshot> PaymentSnapshots { get; set; } = [];
        public ICollection<ReconciliationTask> ReconciliationTasks { get; set; } = [];
    }
}
