using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.User;

namespace Arb_Int.Domain.Worker
{
    public class ScheduledTaskDefinition : IEntity<Guid>, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }

        // scope
        public Guid? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public Guid? PartyId { get; set; }
        public Party? Party { get; set; }

        public Guid? ContactPointId { get; set; }
        public PartyContactPoint? ContactPoint { get; set; }


        public ScheduledTaskType TaskType { get; set; } = ScheduledTaskType.None;
        public ScheduleTriggerType TriggerType { get; set; } = ScheduleTriggerType.Once;
        public ScheduledTaskPriority Priority { get; set; } = ScheduledTaskPriority.Normal;

        // semantics
        public ScheduleTimeSemantics TimeSemantics { get; set; } = ScheduleTimeSemantics.UtcFixed;

        /// <summary>
        /// Required when TimeSemantics = UserLocalWallClock.
        /// Example: "Europe/Stockholm"
        /// </summary>
        public string? TimeZoneId { get; set; }

        /// <summary>
        /// Next time to generate an occurrence (UTC).
        /// </summary>
        public DateTimeOffset NextRunAtUtc { get; set; }
        /// <summary>
        /// When (if at all) the occurrences should stop
        /// </summary>
        public DateTimeOffset? EndAtUtc { get; set; }

        /// <summary>
        /// Template payload copied to occurrences.
        /// </summary>
        public string? PayloadJson { get; set; }

        public int MaxAttempts { get; set; } = 10;

        public ScheduledTaskDefinitionStatus Status { get; set; } = ScheduledTaskDefinitionStatus.Enabled;

        // multi-node lease
        public DateTimeOffset? LockedUntilUtc { get; set; }
        public string? LockedBy { get; set; }

        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public string? IdempotencyKey { get; set; }
        public List<ScheduledTask> Occurrences { get; set; } = [];
        public uint RowVersion { get; set; }
    }
}