namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Anger varför en ScheduledTask är kopplad till en BuyerRfq.
    /// </summary>
    public enum BuyerRfqScheduledTaskPurpose
    {
        None = 0,
        DeadlineReminder = 1,
        Close = 2,
        PostDeadlineProcessing = 3
    }
}