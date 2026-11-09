namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller detaljerad information om en BuyerDemand.
    /// </summary>
    public sealed class BuyerDemandResponse
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
        /// Identifierar relationens motpart.
        /// </summary>
        public Guid CounterpartyId { get; init; }

        /// <summary>
        /// Motpartens visningsnamn.
        /// </summary>
        public required string CounterpartyName { get; init; }

        /// <summary>
        /// Efterfrågans namn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Efterfrågans beskrivning.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Anger kunskapsnivån för efterfrågan.
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
        /// Tidpunkt då efterfrågan börjar gälla.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Tidpunkt då efterfrågan slutar gälla.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Tidpunkt då efterfrågan senast bekräftades.
        /// </summary>
        public DateTimeOffset? LastConfirmedAtUtc { get; init; }

        /// <summary>
        /// Identifierar ansvarig användare.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Interna anteckningar.
        /// </summary>
        public string? InternalNotes { get; init; }

        /// <summary>
        /// Tidpunkt då efterfrågan skapades.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }

        /// <summary>
        /// Matchningsbara delområden i efterfrågan.
        /// </summary>
        public required IReadOnlyList<BuyerDemandScopeResponse> Scopes { get; init; }

        /// <summary>
        /// Evidens kopplad till efterfrågan.
        /// </summary>
        public required IReadOnlyList<BuyerDemandEvidenceResponse> Evidence { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}