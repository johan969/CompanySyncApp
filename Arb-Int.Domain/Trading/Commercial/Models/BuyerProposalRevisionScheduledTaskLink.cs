using Arb_Int.Domain.Worker;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Länkar en konkret ScheduledTask till en BuyerProposalRevision och beskriver uppgiftens syfte.
    /// </summary>
    public sealed class BuyerProposalRevisionScheduledTaskLink
    {
        /// <summary>
        /// Proposal-revision som uppgiften hör till.
        /// </summary>
        public Guid BuyerProposalRevisionId { get; set; }

        /// <summary>
        /// Proposal-revision som uppgiften hör till.
        /// </summary>
        public BuyerProposalRevision BuyerProposalRevision { get; set; } = null!;

        /// <summary>
        /// Konkret worker-uppgift som ska köras.
        /// </summary>
        public Guid ScheduledTaskId { get; set; }

        /// <summary>
        /// Konkret worker-uppgift som ska köras.
        /// </summary>
        public ScheduledTask ScheduledTask { get; set; } = null!;

        /// <summary>
        /// Beskriver vilken funktion uppgiften har för revisionen.
        /// </summary>
        public BuyerProposalRevisionScheduledTaskPurpose Purpose { get; set; } = BuyerProposalRevisionScheduledTaskPurpose.None;
    }
}