using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Trading
{
    public class ExternalConversation : IEntity<Guid>, INamed, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid ExternalBusinessRelationshipId { get; set; }
        public required string Name { get; set; }
        public ExternalConversationStatus Status { get; set; }
        public CommunicationChannel PrimaryChannel { get; set; }
        public string? Subject { get; set; }
        public Guid? BuyerRfqId { get; set; }
        public Guid? SupplierQuoteId { get; set; }
        public Guid? BuyerProposalId { get; set; }
        public Guid? BuyerSalesOrderId { get; set; }
        public Guid? SupplierPurchaseOrderId { get; set; }
        public Guid? OwnerUserId { get; set; }
        public DateTimeOffset? LastMessageAtUtc { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship ExternalBusinessRelationship { get; set; } = null!;
        public BuyerRfq? BuyerRfq { get; set; }
        public SupplierQuote? SupplierQuote { get; set; }
        public BuyerProposal? BuyerProposal { get; set; }
        public BuyerSalesOrder? BuyerSalesOrder { get; set; }
        public SupplierPurchaseOrder? SupplierPurchaseOrder { get; set; }
        public ICollection<ExternalMessage> Messages { get; set; } = [];
    }
}
