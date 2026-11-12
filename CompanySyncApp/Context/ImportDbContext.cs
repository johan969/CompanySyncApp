using Microsoft.EntityFrameworkCore;

namespace CompanySyncApp.Context;


public sealed class ImportDbContext : DbContext
{
    public ImportDbContext(
        DbContextOptions<ImportDbContext> options)
        : base(options)
    {
        
    }
}