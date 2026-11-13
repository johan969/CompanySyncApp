using CompanySyncApp.Context;
using CompanySyncApp.Models;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace CompanySyncApp.Services;

public class CompanySpeciesSyncService
{
    private readonly ImportDbContext _db;
    public CompanySpeciesSyncService(ImportDbContext db)
    {
        _db = db;
    }
    public async Task ImportCompanySpeciesAsync(string filepath)
    {
        var source = new CsvCompanyDataSource(filepath);
        var rows = await source.GetCompaniesAsync();

        static string Normalize(string? value) =>
            (value ?? "").Trim().ToUpperInvariant();

        var companies = await _db.Companies.AsNoTracking().ToListAsync();
        var species = await _db.AlgaeSpecies.AsNoTracking().ToListAsync();

        var companiesByKey = companies
            .GroupBy(company => (
                Normalize(company.Name),
                Normalize(company.Headquarters)))
            .ToDictionary(group => group.Key, group => group.ToList());

        var speciesByName = species
            .GroupBy(item => Normalize(item.SpeciesName))
            .ToDictionary(group => group.Key, group => group.ToList());

        var existingLinks = await _db.CompanySpecies
            .AsNoTracking()
            .Select(link => new { link.CompanyId, link.SpeciesId })
            .ToListAsync();

        var knownLinks = existingLinks
            .Select(link => (link.CompanyId, link.SpeciesId))
            .ToHashSet();

        var processedCompanies = new HashSet<(string, string)>();
        var created = 0;
        var existing = 0;
        var unresolved = 0;

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Name))
            {
                Console.Error.WriteLine(
                    $"Rad {row.SourceRowNumber} skippas: namn saknas.");
                continue;
            }

            var companyKey = (Normalize(row.Name), Normalize(row.Headquarters));

            if (!processedCompanies.Add(companyKey))
            {
                Console.Error.WriteLine(
                    $"Rad {row.SourceRowNumber} skippas: dubbelt företag i CSV-filen.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(row.Species))
            {
                continue;
            }

            if (!companiesByKey.TryGetValue(companyKey, out var companyMatches)
                || companyMatches.Count != 1)
            {
                Console.Error.WriteLine(
                    $"Rad {row.SourceRowNumber} skippas: företaget saknas eller matchar flera poster.");
                unresolved++;
                continue;
            }

            var company = companyMatches[0];

            using var reader = CsvReader.FromText(
                row.Species,
                new CsvReaderOptions { HasHeader = false });

            while (reader.Read())
            {
                for (var index = 0; index < reader.FieldCount; index++)
                {
                    var speciesName = reader.GetString(index).Trim();

                    if (string.IsNullOrWhiteSpace(speciesName))
                    {
                        continue;
                    }

                    if (!speciesByName.TryGetValue(
                            Normalize(speciesName), out var speciesMatches)
                        || speciesMatches.Count != 1)
                    {
                        Console.Error.WriteLine(
                            $"Rad {row.SourceRowNumber}: arten '{speciesName}' saknas eller är tvetydig. Kopplingen skippas.");
                        unresolved++;
                        continue;
                    }

                    var matchedSpecies = speciesMatches[0];
                    var linkKey = (company.Id, matchedSpecies.Id);

                    if (!knownLinks.Add(linkKey))
                    {
                        existing++;
                        continue;
                    }

                    _db.CompanySpecies.Add(new CompanySpecies
                    {
                        CompanyId = company.Id,
                        SpeciesId = matchedSpecies.Id
                    });

                    created++;
                }
            }
        }

        await _db.SaveChangesAsync();

        Console.WriteLine(
            $"Artkopplingar skapade: {created}, redan befintliga: {existing}, olösta: {unresolved}");
    }
}