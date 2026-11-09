using CompanySyncApp.Models;

namespace CompanySyncApp.Services;

public sealed class JsonHistoryService : IHistoryService
{
    public Task<IReadOnlyList<AuditLog>> GetHistoryAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("JSON history loading has not been implemented yet.");
    }

    public Task SaveHistoryAsync(
        IReadOnlyCollection<AuditLog> entries,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("JSON history saving has not been implemented yet.");
    }
}
