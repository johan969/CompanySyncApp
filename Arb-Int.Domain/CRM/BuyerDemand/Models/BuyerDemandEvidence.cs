using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    public class BuyerDemandEvidence : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerDemandId { get; set; }
        public EvidenceSourceKind SourceKind { get; set; }
        public EvidencePosition Position { get; set; }
        public Guid? ReferenceEntryId { get; set; }
        public Guid? CrmActivityId { get; set; }
        public string? SourceUri { get; set; }
        public string? SourceExternalId { get; set; }
        public string? Excerpt { get; set; }
        public string? Notes { get; set; }
        public decimal? Confidence { get; set; }
        public DateTimeOffset ObservedAtUtc { get; set; }
        public string? EvidenceHash { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerDemand BuyerDemand { get; set; } = null!;
        public ReferenceEntry? ReferenceEntry { get; set; }
        public CrmActivity? CrmActivity { get; set; }
    }
}
