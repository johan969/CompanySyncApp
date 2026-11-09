namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller kompakt information om en CRM-aktivitet.
    /// </summary>
    public sealed class CrmActivityListResponse
    {
        /// <summary>
        /// Identifierar aktiviteten.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar den primära parten som aktiviteten gäller.
        /// </summary>
        public Guid RegardingPartyId { get; init; }

        /// <summary>
        /// Den primära partens visningsnamn.
        /// </summary>
        public required string RegardingPartyName { get; init; }

        /// <summary>
        /// Identifierar eventuell specifik affärsrelation.
        /// </summary>
        public Guid? BusinessRelationshipId { get; init; }

        /// <summary>
        /// Relationstypens kod när aktiviteten är kopplad till en affärsrelation.
        /// </summary>
        public string? BusinessRelationshipTypeCode { get; init; }

        /// <summary>
        /// Anger aktivitetens typ.
        /// </summary>
        public CrmActivityType ActivityType { get; init; }

        /// <summary>
        /// Anger aktivitetens aktuella status.
        /// </summary>
        public CrmActivityStatus Status { get; init; }

        /// <summary>
        /// Anger aktivitetens riktning.
        /// </summary>
        public CrmActivityDirection Direction { get; init; }

        /// <summary>
        /// Aktivitetens rubrik.
        /// </summary>
        public required string Subject { get; init; }

        /// <summary>
        /// Tidpunkt då aktiviteten inträffade.
        /// </summary>
        public DateTimeOffset? OccurredUtc { get; init; }

        /// <summary>
        /// Tidpunkt då aktiviteten ska vara utförd.
        /// </summary>
        public DateTimeOffset? DueUtc { get; init; }

        /// <summary>
        /// Tidpunkt då aktiviteten slutfördes.
        /// </summary>
        public DateTimeOffset? CompletedUtc { get; init; }

        /// <summary>
        /// Identifierar ansvarig användare.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Antal deltagare i aktiviteten.
        /// </summary>
        public int ParticipantCount { get; init; }
    }
}