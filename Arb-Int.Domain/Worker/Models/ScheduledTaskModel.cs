
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.User;

namespace Arb_Int.Domain.Worker
{


    public class ScheduledTask : IEntity<Guid>, IActionTracker, ICreateTracker, IRowVersion
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }
        public ApplicationUser? User { get; set; }



        public Guid? PartyId { get; set; }
        public Party? Party { get; set; }

        public Guid? ContactPointId { get; set; }
        public PartyContactPoint? ContactPoint { get; set; }


        public ScheduledTaskType? TaskType { get; set; }
        public ScheduleTriggerType? TriggerType { get; set; }
        public ScheduledTaskStatus? Status { get; set; }
        public ScheduledTaskPriority? Priority { get; set; }

        // master link
        public Guid? DefinitionId { get; set; }
        public ScheduledTaskDefinition? Definition { get; set; }

        /// <summary>
        /// “When to trigger?” Always UTC.
        /// </summary>
        public DateTimeOffset RunAtUtc { get; set; }

        public DateTimeOffset? StartedAtUtc { get; set; }
        public DateTimeOffset? CompletedAtUtc { get; set; }

        public string? PayloadJson { get; set; }

        public DateTimeOffset? LockedUntilUtc { get; set; }
        public string? LockedBy { get; set; }

        public int Attempts { get; set; }
        public int MaxAttempts { get; set; } = 10;

        public string? IdempotencyKey { get; set; }

        public string? LastError { get; set; }
        public DateTimeOffset? LastErrorAtUtc { get; set; }

        public string? HangfireJobId { get; set; }

        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
    }

}
