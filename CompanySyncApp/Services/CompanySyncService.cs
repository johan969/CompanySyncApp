using CompanySyncApp.Models;

namespace CompanySyncApp.Services;

public sealed class CompanySyncService : ICompanySyncService
{
    public Task<IReadOnlyList<AuditLog>> SynchronizeAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Company synchronization has not been implemented yet.");
    }
}
