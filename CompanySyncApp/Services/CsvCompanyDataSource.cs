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
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var company = new ImportedCompany
            {
                Name = name,
                Headquarters = headquarters
            };

            companies.Add(company);
        }
        return companies;
    }


}