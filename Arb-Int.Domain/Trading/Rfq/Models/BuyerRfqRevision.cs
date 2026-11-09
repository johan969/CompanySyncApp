using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Skapar en ny revision när en släppt RFQ ändras, så att matchningar och förslag alltid kan härledas till exakt underlag.
    /// </summary>
    public class BuyerRfqRevision : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerRfqId { get; set; }
        public int RevisionNumber { get; set; }
        public BuyerRfqRevisionStatus Status { get; set; }
        public string? Summary { get; set; }
        public string? BuyerOriginalText { get; set; }
        public string? InternalInterpretation { get; set; }
        public DateTimeOffset? ResponseRequestedByUtc { get; set; }
        public DateTimeOffset? ReleasedAtUtc { get; set; }
        public Guid? ReleasedByUserId { get; set; }
        public DateTimeOffset? SupersededAtUtc { get; set; }
        public string? SnapshotHash { get; set; }
        public string? SnapshotJson { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerRfq BuyerRfq { get; set; } = null!;
        public ICollection<BuyerRfqLine> Lines { get; set; } = [];
        public ICollection<BuyerRfqDelivery> Deliveries { get; set; } = [];
        public ICollection<BuyerRfqRequirement> Requirements { get; set; } = [];
        public ICollection<MatchingRun> MatchingRuns { get; set; } = [];
    }
}
