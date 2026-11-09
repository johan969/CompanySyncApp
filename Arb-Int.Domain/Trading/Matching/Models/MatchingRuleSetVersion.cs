using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class MatchingRuleSetVersion : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid MatchingRuleSetId { get; set; }
        public int VersionNumber { get; set; }
        public string? Description { get; set; }
        public required string ConfigurationJson { get; set; }
        public required string ConfigurationHash { get; set; }
        public DateTimeOffset? EffectiveFromUtc { get; set; }
        public DateTimeOffset? EffectiveToUtc { get; set; }
        public DateTimeOffset? PublishedAtUtc { get; set; }
        public Guid? PublishedByUserId { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public MatchingRuleSet MatchingRuleSet { get; set; } = null!;
        public ICollection<MatchingRun> MatchingRuns { get; set; } = [];
    }
}
