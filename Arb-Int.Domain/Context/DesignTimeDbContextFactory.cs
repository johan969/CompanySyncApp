using Arb_Int.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Arb_Int.Domain
{
    public class DesignTimeArb_IntDbContextFactory : IDesignTimeDbContextFactory<Arb_IntDbContext>
    {
        public DesignTimeArb_IntDbContextFactory() { }
        public Arb_IntDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@Directory.GetCurrentDirectory() + "/../Arb-Int.API/appsettings.json")
#if DEBUG
                .AddJsonFile(@Directory.GetCurrentDirectory() + "/../Arb-Int.API/appsettings.Development.json")
#endif
                .Build();
            var builder = new DbContextOptionsBuilder<Arb_IntDbContext>();
            var connectionString = configuration.GetConnectionString("DBConnectionString");
            builder.UseNpgsql(connectionString);
            return new Arb_IntDbContext(builder.Options);
        }
    }
    //public class DesignTimeIdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    //{
    //    public IdentityDbContext CreateDbContext(string[] args)
    //    {
    //        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "/../CoreAPI/appsettings.json").Build();
    //        var builder = new DbContextOptionsBuilder<IdentityDbContext>();
    //        var connectionString = configuration.GetConnectionString("ApplicationDBConnectionString");
    //        builder.UseSqlServer(connectionString);
    //        return new IdentityDbContext(builder.Options);
    //    }
    //}
}
