namespace CompanySyncApp.Models;

public sealed class AuditLog
{
    public DateTimeOffset Timestamp { get; set; }

    public string OrgNumber { get; set; } = string.Empty;

    public string Change { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;
}
