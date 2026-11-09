namespace Arb_Int.Domain.Worker
{
    public sealed class UpsertScheduledTaskDefinitionRequest
    {
        /// <summary>
        /// null => create, otherwise update
        /// </summary>
        public Guid? Id { get; set; }

        public Guid? UserId { get; set; }

        /// <summary>
        /// Type of task
        /// </summary>
        public ScheduledTaskType TaskType { get; set; } = ScheduledTaskType.None;
        /// <summary>
        /// Type of occurrence trigger
        /// </summary>
        public ScheduleTriggerType TriggerType { get; set; } = ScheduleTriggerType.Once;
        /// <summary>
        /// Priority of the task
        /// </summary>
        public ScheduledTaskPriority Priority { get; set; } = ScheduledTaskPriority.Normal;
        /// <summary>
        /// How to interpret the Time (UTC or TimeZone)
        /// </summary>
        public ScheduleTimeSemantics TimeSemantics { get; set; } = ScheduleTimeSemantics.UtcFixed;

        /// <summary>
        /// Local wall clock input, for example "UTC" or "Europe/Stockholm"
        /// </summary>
        public string? TimeZoneId { get; set; }
        /// <summary>
        /// Local time i.e. 2026-02-23 09:04 (Kind=Unspecified)
        /// </summary>
        public DateTime? FirstRunAtLocal { get; set; } // 

        /// <summary>
        /// UtcFixed input (to create directly in UTC)
        /// </summary>
        public DateTimeOffset? FirstRunAtUtc { get; set; }
        /// <summary>
        /// End date of occurences (if any)
        /// </summary>
        public DateTimeOffset? EndAtUtc { get; set; }
        /// <summary>
        /// Payload of the task
        /// </summary>
        public string? PayloadJson { get; set; }

        public int? MaxAttempts { get; set; }

        // Optional idempotency - du kan sätta från client eller server-side
        //public string? IdempotencyKey { get; set; }
    }
}