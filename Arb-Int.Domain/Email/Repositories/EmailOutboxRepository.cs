using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Email
{

    public interface IEmailOutboxRepository
    {
        Task<bool> IsScheduledAsync(Guid outboxId, CancellationToken cancellationToken = default);
        Task<EmailOutboxMessage?> GetFromScheduledTaskId(Guid taskId, CancellationToken cancellationToken = default);

        Task SaveAsync(EmailOutboxMessage message, CancellationToken cancellationToken = default);
    }

    public class EmailOutboxRepository : IEmailOutboxRepository
    {
        private readonly Arb_IntDbContext dbContext;
        private readonly DbSet<EmailOutboxMessage> dbSet;
        public EmailOutboxRepository(Arb_IntDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<EmailOutboxMessage>();
        }

        public async Task<bool> IsScheduledAsync(Guid outboxId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(x => x.Id == outboxId)
                .AnyAsync(x => x.ScheduledTaskId != null, cancellationToken);
        }
        public async Task<EmailOutboxMessage?> GetFromScheduledTaskId(Guid taskId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Include(eom => eom.Recipients)
                .FirstOrDefaultAsync(x => x.ScheduledTaskId == taskId, cancellationToken);
        }

        public async Task SaveAsync(EmailOutboxMessage message, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(message, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}