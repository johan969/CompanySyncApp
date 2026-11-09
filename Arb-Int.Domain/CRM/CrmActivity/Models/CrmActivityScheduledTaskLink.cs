using Arb_Int.Domain.Worker;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Länkar en konkret ScheduledTask till en CrmActivity och beskriver uppgiftens syfte.
    /// </summary>
    public sealed class CrmActivityScheduledTaskLink
    {
        /// <summary>
        /// CRM-aktivitet som uppgiften hör till.
        /// </summary>
        public Guid CrmActivityId { get; set; }

        /// <summary>
        /// CRM-aktivitet som uppgiften hör till.
        /// </summary>
        public CrmActivity CrmActivity { get; set; } = null!;

        /// <summary>
        /// Konkret worker-uppgift som ska köras.
        /// </summary>
        public Guid ScheduledTaskId { get; set; }

        /// <summary>
        /// Konkret worker-uppgift som ska köras.
        /// </summary>
        public ScheduledTask ScheduledTask { get; set; } = null!;

        /// <summary>
        /// Beskriver vilken funktion uppgiften har för CRM-aktiviteten.
        /// </summary>
        public CrmActivityScheduledTaskPurpose Purpose { get; set; } = CrmActivityScheduledTaskPurpose.None;
    }
}