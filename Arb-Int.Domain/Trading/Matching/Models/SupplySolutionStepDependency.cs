using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class SupplySolutionStepDependency : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplySolutionCandidateId { get; set; }
        public Guid PredecessorStepId { get; set; }
        public Guid SuccessorStepId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplySolutionStep PredecessorStep { get; set; } = null!;
        public SupplySolutionStep SuccessorStep { get; set; } = null!;
    }
}
