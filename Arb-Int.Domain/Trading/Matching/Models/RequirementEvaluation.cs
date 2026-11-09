using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class RequirementEvaluation : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplySolutionCandidateId { get; set; }
        public Guid BuyerRfqRequirementId { get; set; }
        public Guid? SupplySolutionLineId { get; set; }
        public Guid? SupplyAllocationId { get; set; }
        public Guid? SupplySolutionStepId { get; set; }
        public RequirementEvaluationOutcome Outcome { get; set; }
        public bool IsHardFail { get; set; }
        public VerificationState VerificationState { get; set; }
        public decimal? ActualNumericValue { get; set; }
        public string? ActualTextValue { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public string? Explanation { get; set; }
        public string? EvidenceSnapshotJson { get; set; }
        public string? RuleTraceJson { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplySolutionCandidate SupplySolutionCandidate { get; set; } = null!;
        public BuyerRfqRequirement BuyerRfqRequirement { get; set; } = null!;
        public SupplySolutionLine? SupplySolutionLine { get; set; }
        public SupplyAllocation? SupplyAllocation { get; set; }
        public SupplySolutionStep? SupplySolutionStep { get; set; }
        public UnitOfMeasure? UnitOfMeasure { get; set; }
    }
}
