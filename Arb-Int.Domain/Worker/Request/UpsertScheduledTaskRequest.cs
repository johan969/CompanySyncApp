namespace Arb_Int.Domain.Worker
{
    public sealed class UpsertScheduledTaskRequest
    {
        /// <summary>
        /// null => create, otherwise update
        /// </summary>
        public Guid? Id { get; set; }

        public Guid? UserId { get; set; }

        public ScheduledTaskType? TaskType { get; set; }
        public ScheduleTriggerType? TriggerType { get; set; }
        public ScheduledTaskStatus? Status { get; set; }
        public ScheduledTaskPriority? Priority { get; set; }

        /// <summary>
        /// “When to trigger?” Always UTC.
        /// </summary>
        public DateTimeOffset RunAtUtc { get; set; }

        public string? PayloadJson { get; set; }

        public int? MaxAttempts { get; set; }
    }
}