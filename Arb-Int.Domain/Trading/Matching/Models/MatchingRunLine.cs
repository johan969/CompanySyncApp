using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class MatchingRunLine : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid MatchingRunId { get; set; }
        public Guid BuyerRfqRevisionId { get; set; }
        public Guid BuyerRfqLineId { get; set; }
        public decimal RequiredQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public string? InputSnapshotJson { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public MatchingRun MatchingRun { get; set; } = null!;
        public BuyerRfqLine BuyerRfqLine { get; set; } = null!;
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
    }
}
