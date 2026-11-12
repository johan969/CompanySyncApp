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

       await companyService.ImportCompaniesAsync(filepathCompany);                                                                                 
       await ImportSpeciesAsync(db, filePathSpecies);                                                                                   
       await ImportCompanySpeciesAsync(db, filepathCompany);                                                                            
   }             
       
    private static async Task ImportSpeciesAsync(ImportDbContext db, string filepath)                                                     
{                                                                                                                   
                                                                             
    var source = new CsvSpeciesDataSource(filepath);                                                                
    var rows = await source.ReadSpeciesAsync();                                                                     
                                                                                                                    
    var existingNames = await db.AlgaeSpecies                                                                        
        .Select(species => species.SpeciesName)                                                                      
        .ToListAsync();                                                                                              
                                                                                                                    
    var knownNames = existingNames                                                                                   
        .Select(name => name.Trim())                                                                                 
        .ToHashSet(StringComparer.OrdinalIgnoreCase);                                                               
                                                                                                                    
    var created = 0;                                                                                                
    var skipped = 0;                                                                                                
                                                                                                                    
    foreach (var row in rows)                                                                                       
    {                                                                                                               
        if (!knownNames.Add(row.SpeciesName.Trim()))                                                                
        {                                                                                                           
            Console.Error.WriteLine($"Rad {row.SourceRowNumber} skippad: dubblett hittad.");                        
            skipped++;                                                                                              
            continue;                                                                                               
        }                                                                                                           
                                                                                                                    
        db.AlgaeSpecies.Add(new AlgaeSpecies                                                                        
        {                                                                                                           
            Id = Guid.NewGuid(),                                                                                    
            SpeciesName = row.SpeciesName.Trim(),                                                                   
            Genus = row.Genus,                                                                                      
            Color = row.Color,                                                                                      
            CommonName = row.CommonName                                                                             
        });                                                                                                         
        created++;                                                                                                  
    }                                                                                                               
                                                                                                                    
    await db.SaveChangesAsync();                                                                                    
    Console.WriteLine($"Arter skapade: {created}, överhoppade: {skipped}");                                         
}

    private static async Task ImportCompanySpeciesAsync(ImportDbContext db, string filepath)
    {
        var source = new CsvCompanyDataSource(filepath);
        var rows = await source.GetCompaniesAsync();

        static string Normalize(string? value) =>
            (value ?? "").Trim().ToUpperInvariant();

        var companies = await db.Companies.AsNoTracking().ToListAsync();
        var species = await db.AlgaeSpecies.AsNoTracking().ToListAsync();

        var companiesByKey = companies
            .GroupBy(company => (
                Normalize(company.Name),
                Normalize(company.Headquarters)))
            .ToDictionary(group => group.Key, group => group.ToList());

        var speciesByName = species
            .GroupBy(item => Normalize(item.SpeciesName))
            .ToDictionary(group => group.Key, group => group.ToList());

        var existingLinks = await db.CompanySpecies
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

                    db.CompanySpecies.Add(new CompanySpecies
                    {
                        CompanyId = company.Id,
                        SpeciesId = matchedSpecies.Id
                    });

                    created++;
                }
            }
        }

        await db.SaveChangesAsync();

        Console.WriteLine(
            $"Artkopplingar skapade: {created}, redan befintliga: {existing}, olösta: {unresolved}");
    }
}
