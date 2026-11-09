using CompanySyncApp.Models;

namespace CompanySyncApp.Services;

public interface IHistoryService
{
    Task<IReadOnlyList<AuditLog>> GetHistoryAsync(CancellationToken cancellationToken = default);

    Task SaveHistoryAsync(IReadOnlyCollection<AuditLog> entries, CancellationToken cancellationToken = default);
}
