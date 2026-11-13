using CompanySyncApp.Context;
using CompanySyncApp.Models;
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
                                                                                                                    
    var existingNames = await _db.AlgaeSpecies                                                                       
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
                                                                                                                    
        _db.AlgaeSpecies.Add(new AlgaeSpecies                                                                        
        {                                                                                                           
            Id = Guid.NewGuid(),                                                                                    
            SpeciesName = row.SpeciesName.Trim(),                                                                   
            Genus = row.Genus,                                                                                      
            Color = row.Color,                                                                                      
            CommonName = row.CommonName                                                                             
        });                                                                                                         
        created++;                                                                                                  
    }                                                                                                               
                                                                                                                    
    await _db.SaveChangesAsync();                                                                                    
    Console.WriteLine($"Arter skapade: {created}, överhoppade: {skipped}");                                         
}
}