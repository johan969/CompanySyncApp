using CompanySyncApp.Services;

namespace CompanySyncApp;

internal static class Program
{
    private static async Task Main()
    {
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "data",
            "Companies-All companies.csv");

        var source = new CsvCompanyDataSource(filePath);
        var companies = await source.GetCompaniesAsync();

        Console.WriteLine($"Läste {companies.Count} företag.");

        foreach (var company in companies.Take(3))
        {
            Console.WriteLine($"{company.Name} – {company.Website ?? "Ingen webbadress"}");
        }
    }
}
