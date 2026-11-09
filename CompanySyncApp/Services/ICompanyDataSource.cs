using CompanySyncApp.Models;

namespace CompanySyncApp.Services;

public interface ICompanyDataSource
{
    Task<IReadOnlyList<Company>> GetCompaniesAsync(CancellationToken cancellationToken = default);
}
