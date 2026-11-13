using CompanySyncApp.Context;
using CompanySyncApp.Models;
using CompanySyncApp.Models.Enums;
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

        var existingLinks = await _db.CompanySpecies.ToListAsync();

        var knownLinks = existingLinks
            .Select(link => (link.CompanyId, link.SpeciesId))
            .ToHashSet();

        var processedCompanies = new HashSet<(string, string)>();

        var created = 0;
        var existing = 0;
        var unresolved = 0;
        var outdated = 0;

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

            if (!companiesByKey.TryGetValue(companyKey, out var companyMatches)
                || companyMatches.Count != 1)
            {
                Console.Error.WriteLine(
                    $"Rad {row.SourceRowNumber} skippas: företaget saknas eller matchar flera poster.");
                unresolved++;
                continue;
            }

            var company = companyMatches[0];
            var incomingSpeciesIds = new HashSet<Guid>();
            var hasAmbiguousSpecies = false;

            var speciesNames = string.IsNullOrWhiteSpace(row.Species)
                ? new List<string>()
                : ParseSpeciesName(row.Species);

            foreach (var speciesName in speciesNames)
            {
                if (!speciesByName.TryGetValue(
                        Normalize(speciesName), out var speciesMatches))
                {
                    Console.Error.WriteLine(
                        $"Rad {row.SourceRowNumber}: okänd art '{speciesName}', hoppas över.");
                    unresolved++;
                    continue;
                }

                if (speciesMatches.Count != 1)
                {
                    Console.Error.WriteLine(
                        $"Rad {row.SourceRowNumber}: flera arter matchar '{speciesName}'.");
                    hasAmbiguousSpecies = true;
                    unresolved++;
                    continue;
                }

                var matchedSpecies = speciesMatches[0];

                // Även befintliga kopplingar ska räknas som närvarande.
                incomingSpeciesIds.Add(matchedSpecies.Id);

                var linkKey = (company.Id, matchedSpecies.Id);

                if (!knownLinks.Add(linkKey))
                {
                    existing++;
                    continue;
                }

                _db.CompanySpecies.Add(new CompanySpecies
                {
                    CompanyId = company.Id,
                    SpeciesId = matchedSpecies.Id,
                    VerificationStatus = VerificationStatus.ObservedInPublicSource
                });

                created++;
            }

            if (hasAmbiguousSpecies)
            {
                Console.Error.WriteLine(
                    $"Rad {row.SourceRowNumber}: inga gamla kopplingar inaktiveras eftersom artmatchningen är tvetydig.");
                continue;
            }

            outdated += MarkMissingLinksOutdated(
                company.Id,
                incomingSpeciesIds,
                existingLinks);
        }

        await _db.SaveChangesAsync();

        Console.WriteLine(
            $"Artkopplingar skapade: {created}, redan befintliga: {existing}, " +
            $"Outdated: {outdated}, olösta: {unresolved}");
    }

    private static List<string> ParseSpeciesName(string value)
    {
        var names = new List<string>();

        using var reader = CsvReader.FromText(
            value,
            new CsvReaderOptions { HasHeader = false });

        while (reader.Read())
        {
            for (var index = 0; index < reader.FieldCount; index++)
            {
                var name = reader.GetString(index).Trim();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    names.Add(name);
                }
            }
        }

        return names;
    }

    private static int MarkMissingLinksOutdated(
        Guid companyId,
        IReadOnlySet<Guid> incomingSpeciesIds,
        IEnumerable<CompanySpecies> existingLinks)
    {
        var outdatedCount = 0;

        foreach (var link in existingLinks)
        {
            if (link.CompanyId != companyId)
            {
                continue;
            }

            if (link.VerificationStatus != VerificationStatus.Unverified &&
                link.VerificationStatus != VerificationStatus.ObservedInPublicSource)
            {
                continue;
            }

            if (incomingSpeciesIds.Contains(link.SpeciesId))
            {
                continue;
            }

            link.VerificationStatus = VerificationStatus.Outdated;
            outdatedCount++;
        }

        return outdatedCount;
    }
}