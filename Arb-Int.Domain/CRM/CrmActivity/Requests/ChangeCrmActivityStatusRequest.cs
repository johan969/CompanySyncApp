namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att ändra status på en CRM-aktivitet.
    /// </summary>
    public sealed class ChangeCrmActivityStatusRequest
    {
        /// <summary>
        /// Aktivitetens nya status.
        /// </summary>
        public CrmActivityStatus Status { get; init; }

        /// <summary>
        /// Tidpunkt då aktiviteten slutfördes när den nya statusen är Completed.
        /// </summary>
        public DateTimeOffset? CompletedUtc { get; init; }
    }
}