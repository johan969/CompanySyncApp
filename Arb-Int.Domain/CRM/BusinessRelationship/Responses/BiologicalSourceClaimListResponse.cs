namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller kompakt information om ett biologiskt source-claim.
    /// </summary>
    public sealed class BiologicalSourceClaimListResponse
    {
        /// <summary>
        /// Identifierar claimen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar affärsrelationen.
        /// </summary>
        public Guid BusinessRelationshipId { get; init; }

        /// <summary>
        /// Anger motpartens visningsnamn. Behövs när listan sträcker sig över flera relationer.
        /// </summary>
        public string? CounterpartyName { get; init; }

        /// <summary>
        /// Anger affärsrelationens typkod, exempelvis "supplier" eller "buyer".
        /// </summary>
        public string? BusinessRelationshipTypeCode { get; init; }

        /// <summary>
        /// Namnet exakt så som det deklarerades i källan.
        /// </summary>
        public required string DeclaredName { get; init; }

        /// <summary>
        /// Identifierar eventuell mappad intern biologisk källa.
        /// </summary>
        public Guid? BiologicalSourceId { get; init; }

        /// <summary>
        /// Anger den mappade biologiska källans visningsnamn.
        /// </summary>
        public string? MappedBiologicalSourceName { get; init; }

        /// <summary>
        /// Anger relationen mellan motparten och den biologiska källan.
        /// </summary>
        public BiologicalSourceRelationshipKind RelationshipKind { get; init; }

        /// <summary>
        /// Anger status för mappingen.
        /// </summary>
        public ClaimMappingStatus MappingStatus { get; init; }

        /// <summary>
        /// Anger mappingens confidence mellan 0 och 1.
        /// </summary>
        public decimal? MappingConfidence { get; init; }

        /// <summary>
        /// Anger claimens verifieringsstatus.
        /// </summary>
        public ClaimVerificationStatus VerificationStatus { get; init; }

        /// <summary>
        /// Tidpunkt då claimen senast observerades.
        /// </summary>
        public DateTimeOffset LastObservedUtc { get; init; }

        /// <summary>
        /// Antal evidensposter.
        /// </summary>
        public int EvidenceCount { get; init; }

        /// <summary>
        /// Antal geografiska kopplingar.
        /// </summary>
        public int RegionCount { get; init; }
    }
}