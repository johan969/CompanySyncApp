using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class MatchingModerationDecision : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplySolutionCandidateId { get; set; }
        public MatchingModerationDecisionKind Decision { get; set; }
        public Guid DecidedByUserId { get; set; }
        public DateTimeOffset DecidedAtUtc { get; set; }
        public bool IsManualOverride { get; set; }
        public SupplySolutionOutcome? OverriddenOutcome { get; set; }
        public decimal? OverriddenScore { get; set; }
        public required string Reason { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplySolutionCandidate SupplySolutionCandidate { get; set; } = null!;
    }
}
