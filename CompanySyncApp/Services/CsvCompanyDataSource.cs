using System.Globalization;
using CompanySyncApp.Models;
using CsvHelper;

namespace CompanySyncApp.Services;

public sealed class CsvCompanyDataSource
{
    private readonly string _filepath;

    public CsvCompanyDataSource(string filePath)
    {
        _filepath = filePath;
    }

    public async Task<IReadOnlyList<ImportedCompany>> GetCompaniesAsync(CancellationToken cancellationToken = default)
    {
        using var reader = CsvReader.FromFile(_filepath);

        var companies = new List<ImportedCompany>();

        await reader.ReadHeaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var name = reader.GetString("Name").Trim();
            var headquarters = reader.GetString("Headquarters").Trim();
            var species = reader.GetString("Species").Trim();
            var website = reader.GetString("Website").Trim();
            var dateText = reader.GetString("Last Modified").Trim();

            if (!DateOnly.TryParseExact(
                    dateText,
                    "d MMMM yyyy",
                    CultureInfo.GetCultureInfo("sv-SE"),
                    DateTimeStyles.None,
                    out var LastModified))
            {
                Console.Error.WriteLine($"Rad {reader.RowNumber} skippas: Last Modified saknas eller är ogiltigt.");
                continue;
            }
            
            
            

            var company = new ImportedCompany
            {
                SourceRowNumber = reader.RowNumber,
                Name = name,
                Headquarters = headquarters,
                Species = species,
                Website = website,
                LastModified = LastModified
            };

            companies.Add(company);
        }
        return companies;
    }


}