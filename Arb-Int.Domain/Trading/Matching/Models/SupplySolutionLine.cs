using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class SupplySolutionLine : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplySolutionCandidateId { get; set; }
        public Guid MatchingRunId { get; set; }
        public Guid MatchingRunLineId { get; set; }
        public decimal RequiredQuantity { get; set; }
        public decimal CoveredQuantity { get; set; }
        public decimal ShortageQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public bool IsFullyCovered { get; set; }
        public decimal CoveragePercent { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplySolutionCandidate SupplySolutionCandidate { get; set; } = null!;
        public MatchingRunLine MatchingRunLine { get; set; } = null!;
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
        public ICollection<SupplyAllocation> Allocations { get; set; } = [];
        public ICollection<CapacityEvaluation> CapacityEvaluations { get; set; } = [];
    }
}
