namespace Arb_Int.Domain.Worker
{
    public sealed record JobStatus(string JobId, string Status, string? Detail = null);
}