using CompanySyncApp.Models;

namespace CompanySyncApp.Services;

public interface ICompanyDataSource
{
    Task<IReadOnlyList<ImportedCompany>> GetCompaniesAsync(CancellationToken cancellationToken = default);
}
