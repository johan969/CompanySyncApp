using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class DisclosurePackageRevision : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid DisclosurePackageId { get; set; }
        public int RevisionNumber { get; set; }
        public DisclosurePackageStatus Status { get; set; }
        public string? Subject { get; set; }
        public string? ModeratedSummary { get; set; }
        public string? SnapshotJson { get; set; }
        public required string ContentHash { get; set; }
        public DateTimeOffset? ApprovedAtUtc { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public DateTimeOffset? SentAtUtc { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public DisclosurePackage DisclosurePackage { get; set; } = null!;
        public ICollection<DisclosurePackageItem> Items { get; set; } = [];
    }
}
