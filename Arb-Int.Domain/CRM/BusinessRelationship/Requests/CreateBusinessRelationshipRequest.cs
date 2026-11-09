namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att skapa en affärsrelation.
    /// </summary>
    public sealed class CreateBusinessRelationshipRequest
    {
        /// <summary>
        /// Identifierar relationens motpart.
        /// </summary>
        public Guid CounterpartyId { get; init; }

        /// <summary>
        /// Identifierar relationstypen.
        /// </summary>
        public int BusinessRelationshipTypeId { get; init; }

        /// <summary>
        /// Identifierar relationens ursprungliga steg.
        /// </summary>
        public int BusinessRelationshipStageId { get; init; }

        /// <summary>
        /// Relationens prioritet.
        /// </summary>
        public RelationshipPriority? Priority { get; init; }

        /// <summary>
        /// Valfri CRM-poäng mellan 0 och 100.
        /// </summary>
        public int? Score { get; init; }

        /// <summary>
        /// Anger hur relationen upptäcktes.
        /// </summary>
        public string? LeadSource { get; init; }

        /// <summary>
        /// Identifierar användaren som äger relationen.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Tidpunkt då relationen först observerades. Aktuell UTC-tid används om värdet saknas.
        /// </summary>
        public DateTimeOffset? FirstObservedUtc { get; init; }

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
        /// Interna anteckningar om relationen.
        /// </summary>
        public string? Notes { get; init; }
    }
}