using Arb_Int.Domain;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Parties.Validation;
using Arb_Int.Domain.User;
using Arb_Int.Domain.Validation;
using CompanySyncApp.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CompanySyncApp;

internal static class Program
{
    private static async Task Main()
    {
        //läs csv för företag
        var filepath = Path.Combine(
            AppContext.BaseDirectory,
            "data",
            "Companies-All companies.csv");
        
        var source = new CsvCompanyDataSource(filepath);
        var companies = await source.GetCompaniesAsync();
        
        foreach (var company in companies)
        {
            Console.WriteLine("Name: " + company.Name + " Headquarters: " + company.Headquarters + ", Species: " + company.Species);
        }

        
        // //Läs csv för alger
        // var filepath = Path.Combine(
        //     AppContext.BaseDirectory,
        //     "data",
        //     "Algae species-All species.csv");
        // var source = new CsvSpeciesDataSource(filepath);
        // var species = await source.ReadSpeciesAsync();
        //
        // foreach (var specie in species)
        // {
        //     Console.WriteLine("Species: " + specie.SpeciesName + ", Producer: " + specie.Producers);
        // }
    }
}
