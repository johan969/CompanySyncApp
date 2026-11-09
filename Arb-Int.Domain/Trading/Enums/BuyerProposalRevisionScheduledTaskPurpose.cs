namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Anger varför en ScheduledTask är kopplad till en BuyerProposalRevision.
    /// </summary>
    public enum BuyerProposalRevisionScheduledTaskPurpose
    {
        None = 0,
        ExpiryReminder = 1,
        Expire = 2,
        FollowUp = 3
    }
}