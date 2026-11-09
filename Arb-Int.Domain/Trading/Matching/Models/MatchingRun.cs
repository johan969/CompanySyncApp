using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Sparar varje matchningskörning mot en bestämd RFQ-revision och regelversion i stället för att skriva över tidigare resultat.
    /// </summary>
    public class MatchingRun : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerRfqRevisionId { get; set; }
        public Guid MatchingRuleSetVersionId { get; set; }
        public MatchingRunStatus Status { get; set; }
        /// <summary>
        /// Tidpunkt då matchningskörningen köades.
        /// </summary>
        public DateTimeOffset QueuedAtUtc { get; set; }
        /// <summary>
        /// Tidpunkt då en worker faktiskt började behandla matchningskörningen.
        /// </summary>
        public DateTimeOffset? StartedAtUtc { get; set; }
        /// <summary>
        /// Tidpunkt då matchningskörningen avslutades.
        /// </summary>
        public DateTimeOffset? CompletedAtUtc { get; set; }
        public Guid? StartedByUserId { get; set; }
        public string? TriggerName { get; set; }
        public string? CorrelationId { get; set; }
        public string? InputSnapshotJson { get; set; }
        public string? InputSnapshotHash { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerRfqRevision BuyerRfqRevision { get; set; } = null!;
        public MatchingRuleSetVersion MatchingRuleSetVersion { get; set; } = null!;
        public ICollection<MatchingRunLine> Lines { get; set; } = [];
        public ICollection<SupplySolutionCandidate> Candidates { get; set; } = [];
    }
}
