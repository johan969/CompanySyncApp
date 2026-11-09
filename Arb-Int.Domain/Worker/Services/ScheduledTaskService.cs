using System.Formats.Tar;
using Arb_Int.Domain;
using Arb_Int.Domain.User;
using Arb_Int.Domain.Worker;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Repository.System
{
    /// <summary>
    /// Läser schemalagda uppgifter från databasen.
    /// </summary>
    public interface IScheduledTaskService
    {
        /// <summary>
        /// Hämtar en schemalagd uppgift utan change tracking.
        /// </summary>
        Task<ScheduledTask?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Läser schemalagda uppgifter utan att duplicera servicelagrets affärsloggning.
    /// </summary>
    public class ScheduledTaskService : IScheduledTaskService
    {
        private readonly Arb_IntDbContext dbContext;
        private readonly ICurrentUserService currentUserService;

        /// <summary>
        /// Skapar repositoryt med databas- och användarkontext.
        /// </summary>
        public ScheduledTaskService(Arb_IntDbContext dbContext, ICurrentUserService currentUserService)
        {
            this.dbContext = dbContext;
            this.currentUserService = currentUserService;
        }

        /// <summary>
        /// Hämtar en schemalagd uppgift utan change tracking.
        /// </summary>
        public async Task<ScheduledTask?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<ScheduledTask>().AsNoTracking().FirstOrDefaultAsync(st => st.Id == id, cancellationToken);

        }
    }
}