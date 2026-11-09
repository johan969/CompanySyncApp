namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Anger varför en ScheduledTask är kopplad till en CrmActivity.
    /// </summary>
    public enum CrmActivityScheduledTaskPurpose
    {
        None = 0,
        Reminder = 1,
        FollowUp = 2,
        Escalation = 3
    }
}