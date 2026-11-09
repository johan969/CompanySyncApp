using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Email
{
    public interface IEmailTemplateRepository
    {
        Task<EmailTemplate?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<EmailTemplate?> FindByKeyAsync(
            EmailTemplateScope scope,
            string key,
            Guid? organisationId,
            Guid? userId,
            CancellationToken cancellationToken = default);
    }

    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly Arb_IntDbContext dbContext;
        private readonly DbSet<EmailTemplate> dbSet;
        public EmailTemplateRepository(Arb_IntDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<EmailTemplate>();
        }

        public async Task<EmailTemplate?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<EmailTemplate?> FindByKeyAsync(EmailTemplateScope scope, string key, Guid? organisationId, Guid? userId, CancellationToken cancellationToken = default)
        {
            IQueryable<EmailTemplate> query = dbSet.AsNoTracking()
                .Where(t => t.Scope == scope && t.Key == key && t.IsActive);

            query = scope switch
            {
                EmailTemplateScope.System =>
                    query.Where(t => t.OrganisationId == null && t.UserId == null),

                EmailTemplateScope.Organisation =>
                    query.Where(t => t.OrganisationId == organisationId && t.UserId == null),

                EmailTemplateScope.User =>
                    query.Where(t => t.UserId == userId),

                _ => query.Where(_ => false)
            };

            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        public async Task SaveAsync(EmailTemplate template, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(template, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
