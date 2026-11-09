using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class IncotermRule : NamedReferenceEntity, IEntity<int>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned, ISortOrder
    {
        public int Id { get; set; }
        public required string Version { get; set; }
        public int SortOrder { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
    }
}
