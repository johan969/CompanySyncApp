using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CompanySyncApp.Context;

public sealed class ImportDbContextFactory : IDesignTimeDbContextFactory<ImportDbContext>
{
    public ImportDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("COMPANYSYNC_DB_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Sätt COMPANYSYNC_DB_CONNECTION först");
        }

        var options = new DbContextOptionsBuilder<ImportDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new ImportDbContext(options);
    }
}