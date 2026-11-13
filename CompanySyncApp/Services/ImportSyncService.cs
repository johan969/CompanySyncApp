using Arb_Int.Domain;
using Arb_Int.Domain.Parties;
using CompanySyncApp.Models;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using CompanySyncApp.Context;
using CsvHelper;

namespace CompanySyncApp.Services;

public sealed class ImportSyncService
{
    public async Task ImportAsync(string filepathCompany, string filePathSpecies)
    {
        var connectionString = Environment.GetEnvironmentVariable("COMPANYSYNC_DB_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Sätt COMPANYSYNC_DB_CONNECTION först.");
        }

        var options = new DbContextOptionsBuilder<ImportDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var db = new ImportDbContext(options);

        var companyService = new CompanySyncService(db);
        var speciesService = new SpeciesSyncService(db);
        var companySpeciesService = new CompanySpeciesSyncService(db);

        await companyService.ImportCompaniesAsync(filepathCompany);
        await speciesService.ImportSpeciesAsync(filePathSpecies);
        await companySpeciesService.ImportCompanySpeciesAsync(filepathCompany);
    }
}