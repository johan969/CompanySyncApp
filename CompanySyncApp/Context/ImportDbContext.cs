using Microsoft.EntityFrameworkCore;
using CompanySyncApp.Models;
using CompanySyncApp.Models.Enums;

namespace CompanySyncApp.Context;


public sealed class ImportDbContext : DbContext
{
    public ImportDbContext(
        DbContextOptions<ImportDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<AlgaeSpecies> AlgaeSpecies => Set<AlgaeSpecies>();
    public DbSet<CompanySpecies> CompanySpecies => Set<CompanySpecies>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("companysync");

        modelBuilder.Entity<CompanySpecies>()
            .HasKey(link => new { link.CompanyId, link.SpeciesId });

        modelBuilder.Entity<CompanySpecies>()
            .Property(link => link.VerificationStatus)
            .HasDefaultValue(VerificationStatus.ObservedInPublicSource);

        modelBuilder.Entity<CompanySpecies>()
            .HasOne(link => link.Company)
            .WithMany(company => company.SpeciesLinks)
            .HasForeignKey(link => link.CompanyId);

        modelBuilder.Entity<CompanySpecies>()
            .HasOne(link => link.Species)
            .WithMany(species => species.CompanyLinks)
            .HasForeignKey(link => link.SpeciesId);
    }
}
