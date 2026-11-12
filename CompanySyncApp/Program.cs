using CompanySyncApp.Context;
using CompanySyncApp.Models;
using CompanySyncApp.Services;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace CompanySyncApp;

internal static class Program
{
    private static async Task Main()
    {
        var filepathSpecies = Path.Combine(                                                                                    
            AppContext.BaseDirectory,                                                                                   
            "data",                                                                                                     
            "Algae species-All species.csv");  
        
        var filepathCompanies  = Path.Combine(                                                                                           
        AppContext.BaseDirectory,                                                                                          
        "data",                                                                                                            
        "Companies-All companies.csv");                                                                                    
   ;                                                                       
   
        var service = new ImportSyncService();
        await service.ImportAsync(filepathCompanies,filepathSpecies);
    }
    }

