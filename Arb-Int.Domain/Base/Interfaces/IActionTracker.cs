namespace Arb_Int.Domain.Base
{
    public interface IActionTracker
    {
        string? ActionName { get; set; }
        DateTimeOffset? ActionDateUtc { get; set; }
        Guid? ActionUserId { get; set; }
    }
}