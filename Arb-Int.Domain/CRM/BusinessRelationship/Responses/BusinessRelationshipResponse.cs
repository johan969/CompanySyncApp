namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller detaljerad information om en affärsrelation.
    /// </summary>
    public sealed class BusinessRelationshipResponse
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
        /// Den interna organisationens visningsnamn.
        /// </summary>
        public required string OwningOrganizationName { get; init; }

        /// <summary>
        /// Identifierar relationens motpart.
        /// </summary>
        public Guid CounterpartyId { get; init; }

        /// <summary>
        /// Motpartens visningsnamn.
        /// </summary>
        public required string CounterpartyName { get; init; }

        /// <summary>
        /// Anger om motparten är aktiv.
        /// </summary>
        public bool CounterpartyIsActive { get; init; }

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
        /// Anger hur relationen ursprungligen upptäcktes.
        /// </summary>
        public string? LeadSource { get; init; }

        /// <summary>
        /// Identifierar användaren som äger relationen.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Tidpunkt då relationen först observerades.
        /// </summary>
        public DateTimeOffset FirstObservedUtc { get; init; }

        /// <summary>
        /// Tidpunkt då motparten först kontaktades.
        /// </summary>
        public DateTimeOffset? FirstContactedUtc { get; init; }

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

        /// <summary>
        /// Interna anteckningar om relationen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Tidpunkt då relationen skapades.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }

        /// <summary>
        /// Antal CRM-kontakter kopplade till relationen.
        /// </summary>
        public int ContactCount { get; init; }

        /// <summary>
        /// Antal biologiska source-claims kopplade till relationen.
        /// </summary>
        public int BiologicalSourceClaimCount { get; init; }

        /// <summary>
        /// Antal CRM-aktiviteter kopplade till relationen.
        /// </summary>
        public int ActivityCount { get; init; }

        /// <summary>
        /// Antal BuyerDemand-poster kopplade till relationen.
        /// </summary>
        public int BuyerDemandCount { get; init; }
    }
}