using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class MatchingRuleSet : NamedReferenceEntity, IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public MatchingRuleSetStatus Status { get; set; }
        public int LatestVersionNumber { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public ICollection<MatchingRuleSetVersion> Versions { get; set; } = [];
    }
}
