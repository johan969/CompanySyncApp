using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Economy;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Använder sales ordern för avtalet mellan köparen och Algaebra. Den kan försörjas av flera supplier purchase orders.
    /// </summary>
    public class BuyerSalesOrder : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid BuyerBusinessRelationshipId { get; set; }
        public Guid BuyerProposalAcceptanceId { get; set; }
        public required string OrderNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public SalesOrderStatus Status { get; set; }
        public int CurrencyId { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public string? BuyerPurchaseOrderReference { get; set; }
        public int? IncotermRuleId { get; set; }
        public string? NamedPlace { get; set; }
        public string? PaymentTerms { get; set; }
        public DateTimeOffset OrderDateUtc { get; set; }
        public DateTimeOffset? ConfirmedAtUtc { get; set; }
        public DateTimeOffset? ExpectedDeliveryFromUtc { get; set; }
        public DateTimeOffset? ExpectedDeliveryToUtc { get; set; }
        public Guid? OwnerUserId { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship BuyerBusinessRelationship { get; set; } = null!;
        public BuyerProposalAcceptance BuyerProposalAcceptance { get; set; } = null!;
        public Currency Currency { get; set; } = null!;
        public IncotermRule? IncotermRule { get; set; }
        public ICollection<BuyerSalesOrderLine> Lines { get; set; } = [];
        public ICollection<BuyerSalesOrderDelivery> Deliveries { get; set; } = [];
        public ICollection<BuyerSalesOrderStatusHistory> StatusHistory { get; set; } = [];
    }
}
