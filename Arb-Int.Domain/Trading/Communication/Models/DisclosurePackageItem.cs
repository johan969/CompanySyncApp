using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class DisclosurePackageItem : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid DisclosurePackageRevisionId { get; set; }
        public required string FieldKey { get; set; }
        public string? Label { get; set; }
        public DisclosureItemClassification Classification { get; set; }
        public string? SanitizedTextValue { get; set; }
        public string? SanitizedJsonValue { get; set; }
        public string? SourceEntityType { get; set; }
        public Guid? SourceEntityId { get; set; }
        public string? SourcePropertyName { get; set; }
        public int SortOrder { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public DisclosurePackageRevision DisclosurePackageRevision { get; set; } = null!;
    }
}
