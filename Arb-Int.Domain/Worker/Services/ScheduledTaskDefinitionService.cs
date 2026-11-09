
using Arb_Int.Domain;
using Arb_Int.Domain.User;

namespace Arb_Int.Repository.System
{
    /// <summary>
    /// Definierar persistence-operationer för återkommande scheduled-task-definitioner.
    /// </summary>
    public interface IScheduledTaskDefinitionRepository
    {

    }

    /// <summary>
    /// Tillhandahåller persistence för scheduled-task-definitioner utan egen affärsloggning.
    /// </summary>
    public class ScheduledTaskDefinitionRepository : IScheduledTaskDefinitionRepository
    {
        private readonly Arb_IntDbContext dbContext;
        private readonly ICurrentUserService currentUserService;

        /// <summary>
        /// Skapar repositoryt med databas- och användarkontext.
        /// </summary>
        public ScheduledTaskDefinitionRepository(Arb_IntDbContext dbContext, ICurrentUserService currentUserService)
        {
            this.dbContext = dbContext;
            this.currentUserService = currentUserService;
        }
    }
}