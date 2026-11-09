namespace Arb_Int.Domain.Worker
{
    public sealed class ScheduledTaskDefinitionResponse
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Recurrence status
        /// </summary>
        public ScheduledTaskDefinitionStatus Status { get; set; }

        /// <summary>
        /// Type of task
        /// </summary>
        public ScheduledTaskType TaskType { get; set; }
        /// <summary>
        /// Type of occurrence trigger
        /// </summary>
        public ScheduleTriggerType TriggerType { get; set; }
        /// <summary>
        /// Priority of the task
        /// </summary>
        public ScheduledTaskPriority Priority { get; set; }
        /// <summary>
        /// How to interpret the Time (UTC or TimeZone)
        /// </summary>
        public ScheduleTimeSemantics TimeSemantics { get; set; }
        /// <summary>
        /// Local wall clock input, for example "UTC" or "Europe/Stockholm"
        /// </summary>
        public string? TimeZoneId { get; set; }

        /// <summary>
        /// Next time the task occurs
        /// </summary>
        public DateTimeOffset NextRunAtUtc { get; set; }
        /// <summary>
        /// End date of occurences (if any)
        /// </summary>
        public DateTimeOffset? EndAtUtc { get; set; }
        /// <summary>
        /// Payload of the task
        /// </summary>
        public string? PayloadJson { get; set; }

        public Guid? UserId { get; set; }
    }
}