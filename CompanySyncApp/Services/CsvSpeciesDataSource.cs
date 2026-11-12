using CompanySyncApp.Models;
using CsvHelper;

namespace CompanySyncApp.Services;

public sealed class CsvSpeciesDataSource
{
    private readonly string _filepath;

    public CsvSpeciesDataSource(string filepath)
    {
        _filepath = filepath;
    }

    public async Task<List<ImportedAlgaeSpecies>> ReadSpeciesAsync(CancellationToken cancellationToken = default)
    {
        using var reader = CsvReader.FromFile(_filepath);

        var species = new List<ImportedAlgaeSpecies>();
        await reader.ReadHeaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            var algae = reader.GetString("Algae species").Trim();
            var producers = reader.GetString("Producers").Trim();
            if (string.IsNullOrWhiteSpace(algae))
            {
                Console.Error.WriteLine($"Rad {reader.RowNumber} hoppades över: Alg art saknas");
                continue;
            }

            var specie = new ImportedAlgaeSpecies
            {
                SourceRowNumber = reader.RowNumber,
                SpeciesName = algae,
                Producers = producers,
                Genus = reader.GetString("Genus").Trim(),
                Color = reader.GetString("Color").Trim(),
                CommonName = reader.GetString("Common name").Trim()
            };

            species.Add(specie);
        }
        return species;
    }
}