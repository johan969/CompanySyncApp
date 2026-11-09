using Arb_Int.Domain.Worker;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Länkar en konkret ScheduledTask till en BuyerRfq och beskriver uppgiftens syfte.
    /// </summary>
    public sealed class BuyerRfqScheduledTaskLink
    {
        /// <summary>
        /// RFQ som uppgiften hör till.
        /// </summary>
        public Guid BuyerRfqId { get; set; }

        /// <summary>
        /// RFQ som uppgiften hör till.
        /// </summary>
        public BuyerRfq BuyerRfq { get; set; } = null!;

        /// <summary>
        /// Konkret worker-uppgift som ska köras.
        /// </summary>
        public Guid ScheduledTaskId { get; set; }

        /// <summary>
        /// Konkret worker-uppgift som ska köras.
        /// </summary>
        public ScheduledTask ScheduledTask { get; set; } = null!;

        /// <summary>
        /// Beskriver vilken funktion uppgiften har för RFQ:n.
        /// </summary>
        public BuyerRfqScheduledTaskPurpose Purpose { get; set; } = BuyerRfqScheduledTaskPurpose.None;
    }
}