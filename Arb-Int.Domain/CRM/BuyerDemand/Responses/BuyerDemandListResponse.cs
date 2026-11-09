namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller kompakt information om en BuyerDemand.
    /// </summary>
    public sealed class BuyerDemandListResponse
    {
        /// <summary>
        /// Identifierar efterfrågan.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar köparrelationen.
        /// </summary>
        public Guid BusinessRelationshipId { get; init; }

        /// <summary>
        /// Identifierar köparen eller motparten.
        /// </summary>
        public Guid CounterpartyId { get; init; }

        /// <summary>
        /// Namnet på efterfrågan.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Köparens eller motpartens visningsnamn.
        /// </summary>
        public required string CounterpartyName { get; init; }

        /// <summary>
        /// Anger hur säker kunskapen om efterfrågan är.
        /// </summary>
        public BuyerDemandKnowledgeLevel KnowledgeLevel { get; init; }

        /// <summary>
        /// Efterfrågans aktuella status.
        /// </summary>
        public BuyerDemandStatus Status { get; init; }

        /// <summary>
        /// Confidence mellan 0 och 1.
        /// </summary>
        public decimal? Confidence { get; init; }

        /// <summary>
        /// Tidpunkt då efterfrågan senast bekräftades.
        /// </summary>
        public DateTimeOffset? LastConfirmedAtUtc { get; init; }

        /// <summary>
        /// Identifierar ansvarig användare.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Tidpunkt då efterfrågan skapades.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }

        /// <summary>
        /// Antal scopes på efterfrågan.
        /// </summary>
        public int ScopeCount { get; init; }

        /// <summary>
        /// Antal evidensposter.
        /// </summary>
        public int EvidenceCount { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
