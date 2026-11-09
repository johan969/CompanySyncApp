namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller kompakt information om en affärsrelation för listor och sökresultat.
    /// </summary>
    public sealed class BusinessRelationshipListResponse
    {
        /// <summary>
        /// Identifierar affärsrelationen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar den interna organisation som äger relationen.
        /// </summary>
        public Guid OwningOrganizationId { get; init; }

        /// <summary>
        /// Identifierar relationens motpart.
        /// </summary>
        public Guid CounterpartyId { get; init; }

        /// <summary>
        /// Motpartens visningsnamn.
        /// </summary>
        public required string CounterpartyName { get; init; }

        /// <summary>
        /// Relationens typ.
        /// </summary>
        public required BusinessRelationshipTypeResponse RelationshipType { get; init; }

        /// <summary>
        /// Relationens aktuella steg.
        /// </summary>
        public required BusinessRelationshipStageResponse Stage { get; init; }

        /// <summary>
        /// Relationens prioritet.
        /// </summary>
        public RelationshipPriority? Priority { get; init; }

        /// <summary>
        /// Valfri CRM-poäng mellan 0 och 100.
        /// </summary>
        public int? Score { get; init; }

        /// <summary>
        /// Identifierar användaren som äger relationen.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Tidpunkt då motparten senast kontaktades.
        /// </summary>
        public DateTimeOffset? LastContactedUtc { get; init; }

        /// <summary>
        /// Tidpunkt för nästa planerade åtgärd.
        /// </summary>
        public DateTimeOffset? NextActionUtc { get; init; }

        /// <summary>
        /// Tidpunkt då relationen stängdes.
        /// </summary>
        public DateTimeOffset? ClosedUtc { get; init; }
    }
}