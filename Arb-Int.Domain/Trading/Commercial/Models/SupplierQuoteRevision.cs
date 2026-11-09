using Arb_Int.Domain.Base;
using Arb_Int.Domain.Economy;

namespace Arb_Int.Domain.Trading
{
    public class SupplierQuoteRevision : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplierQuoteId { get; set; }
        public Guid SupplierBusinessRelationshipId { get; set; }
        public int RevisionNumber { get; set; }
        public SupplierQuoteRevisionStatus Status { get; set; }
        public int CurrencyId { get; set; }
        public int? IncotermRuleId { get; set; }
        public string? NamedPlace { get; set; }
        public string? PaymentTerms { get; set; }
        public int? LeadTimeDays { get; set; }
        public DateTimeOffset? ValidFromUtc { get; set; }
        public DateTimeOffset? ValidToUtc { get; set; }
        public DateTimeOffset? ReceivedAtUtc { get; set; }
        public string? OriginalDocumentReference { get; set; }
        public string? SnapshotJson { get; set; }
        public string? SnapshotHash { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplierQuote SupplierQuote { get; set; } = null!;
        public Currency Currency { get; set; } = null!;
        public IncotermRule? IncotermRule { get; set; }
        public ICollection<SupplierQuoteLine> Lines { get; set; } = [];
    }
}
