namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar metadata för en CRM-aktivitet.
    /// </summary>
    public sealed class UpdateCrmActivityRequest
    {
        /// <summary>
        /// Anger aktivitetens typ.
        /// </summary>
        public CrmActivityType ActivityType { get; init; }

        /// <summary>
        /// Anger aktivitetens riktning.
        /// </summary>
        public CrmActivityDirection Direction { get; init; }

        /// <summary>
        /// Aktivitetens rubrik.
        /// </summary>
        public required string Subject { get; init; }

        /// <summary>
        /// Aktivitetens beskrivning.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Tidpunkt då aktiviteten faktiskt inträffade.
        /// </summary>
        public DateTimeOffset? OccurredUtc { get; init; }

        /// <summary>
        /// Tidpunkt då aktiviteten ska vara utförd.
        /// </summary>
        public DateTimeOffset? DueUtc { get; init; }

        /// <summary>
        /// Identifierar ansvarig användare.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Externt system som aktiviteten kommer från.
        /// </summary>
        public string? ExternalSystem { get; init; }

        /// <summary>
        /// Aktivitetens stabila identifierare i det externa systemet.
        /// </summary>
        public string? ExternalRecordId { get; init; }
    }
}