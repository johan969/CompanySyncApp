using CompanySyncApp.Context;
using CompanySyncApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanySyncApp.Services;

public sealed class CompanySyncService
{
    private readonly ImportDbContext _db;
    
    public CompanySyncService(ImportDbContext db)
    {
        _db = db;
    }
                                                                                                                      
    public async Task ImportCompaniesAsync(string filepath)
    {
        var source = new CsvCompanyDataSource(filepath);
    var rows = await source.GetCompaniesAsync();   
                                                                                                                           
    var existingCompanies = await _db.Companies.ToListAsync();                                                              
                                                                                                                           
    var processedKeys = new HashSet<(string, string)>();                                                                   
                                                                                                                           
    var created = 0;                                                                                                       
    var skipped = 0;                                                                                                       
    var unchanged = 0;                                                                                                     
    var updated = 0;                                                                                                       
                                                                                                                           
    foreach (var row in rows)                                                                                              
    {                                                                                                                      
        if (string.IsNullOrWhiteSpace(row.Name))                                                                           
        {                                                                                                                  
            Console.Error.WriteLine($"Rad {row.SourceRowNumber} skippas: namn saknas.");                                   
            skipped++;                                                                                                     
            continue;                                                                                                      
        }                                                                                                                  
                                                                                                                           
        var key = (Normalize(row.Name), Normalize(row.Headquarters));                                                      
                                                                                                                           
        if (!processedKeys.Add(key))                                                                                       
        {                                                                                                                  
            Console.Error.WriteLine(                                                                                       
                $"Rad {row.SourceRowNumber} skippas: dubblett namn och land i filen");                                     
            skipped++;                                                                                                     
            continue;                                                                                                      
        }                                                                                                                  
                                                                                                                           
        var matches = FindCompanyMatches(existingCompanies, row);                                                                                                   
                                                                                                                           
        if (matches.Count > 1)                                                                                             
        {                                                                                                                  
            Console.Error.WriteLine($"Rad {row.SourceRowNumber} skippas: flera poster matchar i databasen.");              
            skipped++;                                                                                                     
            continue;                                                                                                      
        }                                                                                                                  
                                                                                                                           
        if (matches.Count == 1)                                                                                            
        {                                                                                                                  
            var existing = matches[0];                                                                                     
                                                                                                                           
            if (!HasCompanyChanges(existing, row))                                                             
            {                                                                                                              
                unchanged++;                                                                                               
            }                                                                                                              
            else                                                                                                           
            {                                                                                                              
                ApplyCompanyChanges(existing,row);                                                                  
                updated++;                                                                                                 
            }                                                                                                              
                                                                                                                           
            continue;                                                                                                      
        }

        _db.Companies.Add(MapToCompany(row));                                                                                       
        created++;                                                                                                         
    }                                                                                                                      
                                                                                                                           
    await _db.SaveChangesAsync();                                                                                           
    Console.WriteLine(                                                                                                     
        $"Skapade: {created}, uppdaterade: {updated}, oförändrade: {unchanged}, överhoppade: {skipped}");                  
}
    
    private static Company MapToCompany(ImportedCompany row)
    {
        return new Company
        {
            Id = Guid.NewGuid(),                                                                                           
            Name = row.Name.Trim(),                                                                                        
            Headquarters = row.Headquarters?.Trim(),                                                                       
            Website = row.Website,                                                                                         
            ValueChain = row.ValueChain,                                                                                   
            ProductionProcess = row.ProductionProcess,                                                                     
            IndustryApplications = row.IndustryApplications,                                                               
            IndustrySubsectorTags = row.IndustrySubsectorTags,                                                             
            Investors = row.Investors,                                                                                     
            AssociationFocus = row.AssociationFocus,                                                                       
            FoundingYear = row.FoundingYear,                                                                               
            StartupScaleupStatus = row.StartupScaleupStatus,                                                               
            DisclosedInvestmentsSince2020 = row.DisclosedInvestmentsSince2020,                                             
            DevelopmentStage = row.DevelopmentStage,                                                                       
            LastModified = row.LastModified     
        };
    }
    private static bool HasCompanyChanges(
        Company existing,
        ImportedCompany incoming)
    {
        var website = string.IsNullOrWhiteSpace(incoming.Website)
            ? null
            : incoming.Website.Trim();
        return existing.Website != website
               || existing.LastModified != incoming.LastModified;
    }

    private static void ApplyCompanyChanges(
        Company existing,
        ImportedCompany incoming)
    {
        existing.Website = string.IsNullOrWhiteSpace(incoming.Website)
            ? null
            : incoming.Website.Trim();

        existing.LastModified = incoming.LastModified;
    }
    private static string Normalize(string? value) =>
        (value ?? "").Trim().ToUpperInvariant();

    private static List<Company> FindCompanyMatches(
        IEnumerable<Company> companies,
        ImportedCompany row)
    {
        var name = Normalize(row.Name);
        var country = Normalize(row.Headquarters);

        return companies
            .Where(company =>
                Normalize(company.Name) == name &&
                Normalize(company.Headquarters) == country)
            .ToList();
    }
    
}