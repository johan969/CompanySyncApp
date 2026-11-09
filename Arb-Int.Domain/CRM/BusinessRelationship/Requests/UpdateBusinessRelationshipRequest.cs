namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar metadata för en affärsrelation.
    /// </summary>
    public sealed class UpdateBusinessRelationshipRequest
    {
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