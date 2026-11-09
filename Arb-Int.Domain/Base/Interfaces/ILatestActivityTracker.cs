namespace Arb_Int.Domain.Base
{
    public interface ILatestActivityTracker
    {
        DateTimeOffset? LatestActivityUtc { get; set; }
    }
}