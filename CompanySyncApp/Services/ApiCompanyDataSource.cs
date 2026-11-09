using CompanySyncApp.Models;

namespace CompanySyncApp.Services;

public sealed class ApiCompanyDataSource : ICompanyDataSource
{
    public Task<IReadOnlyList<Company>> GetCompaniesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("API loading has not been implemented yet.");
    }
}
