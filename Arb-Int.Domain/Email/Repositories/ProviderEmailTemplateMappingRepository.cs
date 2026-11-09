
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Email
{
    public interface IProviderEmailTemplateMappingRepository
    {
        Task<ProviderEmailTemplateMapping?> GetActiveMappingAsync(
            Guid templateVariantId,
            EmailProvider provider,
            CancellationToken cancellationToken = default);
    }

    public sealed class ProviderEmailTemplateMappingRepository : IProviderEmailTemplateMappingRepository
    {
        private readonly Arb_IntDbContext dbContext;
        private readonly DbSet<ProviderEmailTemplateMapping> dbSet;
        public ProviderEmailTemplateMappingRepository(Arb_IntDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<ProviderEmailTemplateMapping>();
        }

        public Task<ProviderEmailTemplateMapping?> GetActiveMappingAsync(
            Guid templateVariantId,
            EmailProvider provider,
            CancellationToken cancellationToken = default)
        {
            return dbSet.AsNoTracking()
                .Where(x => x.TemplateVariantId == templateVariantId
                            && x.Provider == provider
                            && x.IsActive)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task SaveAsync(ProviderEmailTemplateMapping mapping, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(mapping, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}