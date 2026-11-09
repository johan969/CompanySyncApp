using CompanySyncApp.Models;

namespace CompanySyncApp.Services;

public sealed class CsvCompanyDataSource : ICompanyDataSource
{
    public Task<IReadOnlyList<Company>> GetCompaniesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("CSV loading has not been implemented yet.");
    }
}
