using Arb_Int.Domain.Base;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Använder kandidaten som systemets interna lösningsförslag. Den kan bestå av en supplier, en supplier-pool eller flera process- och logistiksteg.
    /// </summary>
    public class SupplySolutionCandidate : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid MatchingRunId { get; set; }
        public int CandidateNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public SupplySolutionKind Kind { get; set; }
        public SupplySolutionOutcome Outcome { get; set; }
        public SupplySolutionStatus Status { get; set; }
        public VerificationState VerificationState { get; set; }
        public decimal TotalScore { get; set; }
        public decimal CoveragePercent { get; set; }
        public decimal? EstimatedPurchaseCost { get; set; }
        public decimal? EstimatedLogisticsCost { get; set; }
        public decimal? EstimatedProcessingCost { get; set; }
        public decimal? EstimatedTotalCost { get; set; }
        public int? EstimatedCostCurrencyId { get; set; }
        public int? EstimatedLeadTimeDays { get; set; }
        public bool ContainsHardFail { get; set; }
        public string? SnapshotJson { get; set; }
        public string? SnapshotHash { get; set; }
        public DateTimeOffset? ExpiresAtUtc { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public MatchingRun MatchingRun { get; set; } = null!;
        public Currency? EstimatedCostCurrency { get; set; }
        public ICollection<SupplySolutionLine> Lines { get; set; } = [];
        public ICollection<SupplySolutionStep> Steps { get; set; } = [];
        public ICollection<RequirementEvaluation> RequirementEvaluations { get; set; } = [];
        public ICollection<ScoreContribution> ScoreContributions { get; set; } = [];
        public ICollection<MatchingModerationDecision> ModerationDecisions { get; set; } = [];
    }
}
