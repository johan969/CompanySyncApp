using Arb_Int.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Email
{

    public interface IEmailTemplateVariantRepository
    {
        Task<List<EmailTemplateVariant>> GetActiveByTemplateIdAsync(Guid templateId, CancellationToken cancellationToken = default);
        Task<EmailTemplateVariant?> ResolveAsync(Guid templateId, string? culture, CancellationToken cancellationToken = default);
    }

    public class EmailTemplateVariantRepository : IEmailTemplateVariantRepository
    {
        private readonly Arb_IntDbContext dbContext;
        private readonly DbSet<EmailTemplateVariant> dbSet;
        public EmailTemplateVariantRepository(Arb_IntDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<EmailTemplateVariant>();
        }

        public async Task<List<EmailTemplateVariant>> GetActiveByTemplateIdAsync(Guid templateId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(v => v.TemplateId == templateId && v.IsActive)
                .ToListAsync(cancellationToken);
        }
        public async Task<EmailTemplateVariant?> ResolveAsync(Guid templateId, string? culture, CancellationToken cancellationToken = default)
        {
            var candidates = await dbSet
                .Where(v => v.TemplateId == templateId && v.IsActive)
                .ToListAsync(cancellationToken);

            var chain = CultureFallback.GetFallbackChain(culture);

            foreach (var c in chain)
            {
                var match = candidates.FirstOrDefault(v => string.Equals(v.Culture, c, StringComparison.OrdinalIgnoreCase));
                if (match != null) return match;
            }

            // fallback till default-variant om kultur inte matchade
            return candidates.FirstOrDefault(v => v.IsDefault) ?? candidates.FirstOrDefault();
        }
        public async Task SaveAsync(EmailTemplateVariant variant, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(variant, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}