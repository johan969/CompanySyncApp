namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller detaljerad information om en CRM-aktivitet.
    /// </summary>
    public sealed class CrmActivityResponse
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
        /// Relationstypens kod när en specifik affärsrelation anges.
        /// </summary>
        public string? BusinessRelationshipTypeCode { get; init; }

        /// <summary>
        /// Aktuellt relationssteg när en specifik affärsrelation anges.
        /// </summary>
        public string? BusinessRelationshipStageCode { get; init; }

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
        /// Aktivitetens beskrivning.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Tidpunkt då aktiviteten faktiskt inträffade.
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
        /// Externt system som aktiviteten importerats eller synkroniserats från.
        /// </summary>
        public string? ExternalSystem { get; init; }

        /// <summary>
        /// Aktivitetens identifierare i det externa systemet.
        /// </summary>
        public string? ExternalRecordId { get; init; }

        /// <summary>
        /// Tidpunkt då aktiviteten skapades.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }

        /// <summary>
        /// Deltagare som är kopplade till aktiviteten.
        /// </summary>
        public required IReadOnlyList<CrmActivityParticipantResponse> Participants { get; init; }
    }
}