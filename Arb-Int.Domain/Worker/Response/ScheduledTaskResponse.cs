namespace Arb_Int.Domain.Worker
{
    public sealed class ScheduledTaskResponse
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public ScheduledTaskType? TaskType { get; set; }
        public ScheduleTriggerType? TriggerType { get; set; }
        public ScheduledTaskStatus? Status { get; set; }
        public ScheduledTaskPriority? Priority { get; set; }

        // master link if recurring task
        public Guid? DefinitionId { get; set; }

        /// <summary>
        /// “When to trigger?” Always UTC.
        /// </summary>
        public DateTimeOffset RunAtUtc { get; set; }

        public DateTimeOffset? StartedAtUtc { get; set; }
        public DateTimeOffset? CompletedAtUtc { get; set; }

        public string? PayloadJson { get; set; }


        public int Attempts { get; set; }
        public int MaxAttempts { get; set; }
    }
}