namespace Arb_Int.Domain.Worker
{
    public sealed record JobAccepted(string JobId, string? Message = null);
}