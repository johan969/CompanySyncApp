using System.Text.Json;
using Arb_Int.Domain.Worker;
using Microsoft.Extensions.Logging;

namespace Arb_Int.Domain.Email
{
    /// <summary>
    /// Schemalägger e-post genom att koppla outbox-meddelanden till ScheduledTask.
    /// </summary>
    public interface IEmailSchedulingService
    {
        // Task<Guid> CreateOutboxAndScheduleAsync(
        //     EmailOutboxMessage outbox,
        //     DateTimeOffset runAt,
        //     CancellationToken cancellationToken = default);
        /// <summary>
        /// Sparar ett outbox-meddelande och skapar dess schemalagda e-postuppgift.
        /// </summary>
        Task CreateOutboxAndScheduleAsync(
            Arb_IntDbContext dbContext,
            EmailOutboxMessage outbox,
            DateTimeOffset runAt,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Skapar outbox-meddelanden och ScheduledTask-poster för e-postutskick.
    /// </summary>
    public sealed class EmailSchedulingService : IEmailSchedulingService
    {
        private readonly IEmailOutboxRepository _outboxRepo;
        private readonly Arb_IntDbContext dbContext;
        private readonly ILogger<EmailSchedulingService> _logger;

        /// <summary>
        /// Skapar e-postschemaläggaren med repository-, databas- och loggingberoenden.
        /// </summary>
        public EmailSchedulingService(
            IEmailOutboxRepository outboxRepo,
            Arb_IntDbContext dbContext,
            ILogger<EmailSchedulingService> logger)
        {
            _outboxRepo = outboxRepo;
            this.dbContext = dbContext;
            _logger = logger;
        }

        // public async Task<Guid> CreateOutboxAndScheduleAsync(
        //     EmailOutboxMessage outbox,
        //     DateTimeOffset runAt,
        //     CancellationToken cancellationToken = default)
        // {

        //     var task = new ScheduledTask
        //     {
        //         OrganisationId = outbox.OrganisationId,
        //         UserId = outbox.UserId,
        //         TaskType = ScheduledTaskType.EmailMessage,
        //         TriggerType = ScheduleTriggerType.Once,
        //         Status = ScheduledTaskStatus.Pending,
        //         Priority = ScheduledTaskPriority.High,
        //         RunAtUtc = runAt,
        //         Attempts = 0,
        //         MaxAttempts = 5,
        //     };

        //     await dbContext.AddAsync(task, cancellationToken);
        //     await dbContext.SaveChangesAsync(cancellationToken);

        //     // 2) koppla outbox 1–1
        //     outbox.ScheduledTaskId = task.Id;
        //     outbox.Status = EmailOutboxStatus.Pending;Task<Guid> CreateOutboxAndScheduleAsync(
        //     EmailOutboxMessage outbox,
        //     DateTimeOffset runAt,
        //     CancellationToken cancellationToken = default);

        //     await _outboxRepo.SaveAsync(outbox, cancellationToken);

        //     return task.Id;
        // }
        /// <summary>
        /// Sparar outbox-meddelandet och skapar en pending engångsuppgift för angiven tid.
        /// </summary>
        public async Task CreateOutboxAndScheduleAsync(
            Arb_IntDbContext dbContext,
            EmailOutboxMessage outbox,
            DateTimeOffset runAt,
            CancellationToken cancellationToken = default)
        {
            dbContext.Set<EmailOutboxMessage>().Add(outbox);
            var task = new ScheduledTask
            {
                Id = Guid.CreateVersion7(),
                PartyId = outbox.PartyId,
                UserId = outbox.UserId,
                TaskType = ScheduledTaskType.EmailMessage,
                TriggerType = ScheduleTriggerType.Once,
                Status = ScheduledTaskStatus.Pending,
                Priority = ScheduledTaskPriority.Normal,
                PayloadJson = JsonSerializer.Serialize(new { OutboxId = outbox.Id }),
                RunAtUtc = runAt,
                Attempts = 0,
                MaxAttempts = 5,
            };
            dbContext.ScheduledTasks.Add(task);
            outbox.ScheduledTaskId = task.Id;
            outbox.ScheduledTask = task;
            outbox.Status = EmailOutboxStatus.Pending;

            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Email outbox message {EmailOutboxMessageId} scheduled as task {ScheduledTaskId} for {RunAtUtc}. PartyId={PartyId}, UserId={UserId}",
                outbox.Id,
                task.Id,
                task.RunAtUtc,
                outbox.PartyId,
                outbox.UserId);
        }
    }
}
