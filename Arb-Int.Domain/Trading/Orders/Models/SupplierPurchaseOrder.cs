using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Economy;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Använder purchase ordern för avtalet mellan Algaebra och exakt en supplier eller servicepartner.
    /// </summary>
    public class SupplierPurchaseOrder : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid SupplierBusinessRelationshipId { get; set; }
        public Guid? SupplierQuoteRevisionId { get; set; }
        public required string OrderNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public int CurrencyId { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public string? SupplierReference { get; set; }
        public int? IncotermRuleId { get; set; }
        public string? NamedPlace { get; set; }
        public string? PaymentTerms { get; set; }
        public DateTimeOffset OrderDateUtc { get; set; }
        public DateTimeOffset? SentAtUtc { get; set; }
        public DateTimeOffset? AcknowledgedAtUtc { get; set; }
        public DateTimeOffset? ExpectedAvailabilityFromUtc { get; set; }
        public DateTimeOffset? ExpectedAvailabilityToUtc { get; set; }
        public Guid? OwnerUserId { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship SupplierBusinessRelationship { get; set; } = null!;
        public SupplierQuoteRevision? SupplierQuoteRevision { get; set; }
        public Currency Currency { get; set; } = null!;
        public IncotermRule? IncotermRule { get; set; }
        public ICollection<SupplierPurchaseOrderLine> Lines { get; set; } = [];
        public ICollection<SupplierPurchaseOrderStatusHistory> StatusHistory { get; set; } = [];
    }
}
