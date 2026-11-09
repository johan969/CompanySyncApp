using CompanySyncApp.Models;

namespace CompanySyncApp.Services;

public interface ICompanySyncService
{
    Task<IReadOnlyList<AuditLog>> SynchronizeAsync(CancellationToken cancellationToken = default);
}
