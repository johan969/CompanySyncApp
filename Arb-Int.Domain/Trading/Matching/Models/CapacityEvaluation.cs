using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class CapacityEvaluation : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplySolutionLineId { get; set; }
        public Guid? SupplyAllocationId { get; set; }
        public CapacityEvaluationOutcome Outcome { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public DateTimeOffset? AvailabilityFromUtc { get; set; }
        public DateTimeOffset? AvailabilityToUtc { get; set; }
        public string? Explanation { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplySolutionLine SupplySolutionLine { get; set; } = null!;
        public SupplyAllocation? SupplyAllocation { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
    }
}
