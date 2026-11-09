using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Trading
{
    public class SupplierQuote : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid SupplierBusinessRelationshipId { get; set; }
        public Guid? BuyerRfqRevisionId { get; set; }
        public required string QuoteNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public SupplierQuoteStatus Status { get; set; }
        public int LatestRevisionNumber { get; set; }
        public string? SupplierReference { get; set; }
        public DateTimeOffset? ReceivedAtUtc { get; set; }
        public Guid? OwnerUserId { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship SupplierBusinessRelationship { get; set; } = null!;
        public BuyerRfqRevision? BuyerRfqRevision { get; set; }
        public ICollection<SupplierQuoteRevision> Revisions { get; set; } = [];
    }
}
