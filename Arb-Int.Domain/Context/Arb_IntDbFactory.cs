
using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain
{
    public interface IArb_IntDbContextFactory
    {
        Arb_IntDbContext CreateDbContext();
    }

    public class Arb_IntDbContextFactory : IArb_IntDbContextFactory
    {
        private readonly IDbContextFactory<Arb_IntDbContext> _efFactory;

        public Arb_IntDbContextFactory(IDbContextFactory<Arb_IntDbContext> efFactory)
        {
            _efFactory = efFactory;
        }

        public Arb_IntDbContext CreateDbContext()
        {
            var context = _efFactory.CreateDbContext();


            return context;
        }
    }
}