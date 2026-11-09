using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class ScoreContribution : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplySolutionCandidateId { get; set; }
        public Guid? RequirementEvaluationId { get; set; }
        public string? Category { get; set; }
        public decimal Weight { get; set; }
        public decimal NormalizedScore { get; set; }
        public decimal Points { get; set; }
        public string? Explanation { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplySolutionCandidate SupplySolutionCandidate { get; set; } = null!;
        public RequirementEvaluation? RequirementEvaluation { get; set; }
    }
}
