using CompanySyncApp.Context;
using CompanySyncApp.Models;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace CompanySyncApp.Services;

public sealed class SpeciesSyncService
{
    private readonly ImportDbContext _db;

    public SpeciesSyncService(ImportDbContext db)
    {
        _db = db;
    }

    public async Task ImportSpeciesAsync(string filepath)
    {
        var source = new CsvSpeciesDataSource(filepath);
        var rows = await source.ReadSpeciesAsync();

        // Hämta hela objekten med tracking, så EF kan spara ändringar.
        var existingSpecies = await _db.AlgaeSpecies.ToListAsync();

        var processedNames = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        var created = 0;
        var updated = 0;
        var unchanged = 0;
        var skipped = 0;

        foreach (var row in rows)
        {
            var name = row.SpeciesName.Trim();

            if (!processedNames.Add(name))
            {
                Console.Error.WriteLine(
                    $"Rad {row.SourceRowNumber} skippas: dubbelt artnamn i filen.");
                skipped++;
                continue;
            }

            var matches = existingSpecies
                .Where(species => string.Equals(
                    species.SpeciesName.Trim(),
                    name,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matches.Count > 1)
            {
                Console.Error.WriteLine(
                    $"Rad {row.SourceRowNumber} skippas: flera arter matchar i databasen.");
                skipped++;
                continue;
            }

            if (matches.Count == 0)
            {
                _db.AlgaeSpecies.Add(MapToSpecies(row));
                created++;
                continue;
            }

            var existing = matches[0];

            if (!HasSpeciesChanges(existing, row))
            {
                unchanged++;
                continue;
            }

            ApplySpeciesChanges(existing, row);
            updated++;
        }

        await _db.SaveChangesAsync();

        Console.WriteLine(
            $"Arter skapade: {created}, uppdaterade: {updated}, " +
            $"oförändrade: {unchanged}, överhoppade: {skipped}");
    }

    private static AlgaeSpecies MapToSpecies(ImportedAlgaeSpecies row)
    {
        return new AlgaeSpecies
        {
            Id = Guid.NewGuid(),
            SpeciesName = row.SpeciesName.Trim(),
            Genus = row.Genus,
            Color = row.Color,
            CommonName = row.CommonName
        };
    }

    private static bool HasSpeciesChanges(
        AlgaeSpecies existing,
        ImportedAlgaeSpecies incoming)
    {
        return existing.Genus != incoming.Genus
               || existing.Color != incoming.Color
               || existing.CommonName != incoming.CommonName;
    }

    private static void ApplySpeciesChanges(
        AlgaeSpecies existing,
        ImportedAlgaeSpecies incoming)
    {
        existing.Genus = incoming.Genus;
        existing.Color = incoming.Color;
        existing.CommonName = incoming.CommonName;
    }
}