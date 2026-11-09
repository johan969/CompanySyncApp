namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller ett komplett biologiskt source-claim med evidens och geografiska kopplingar.
    /// </summary>
    public sealed class BiologicalSourceClaimResponse
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
        /// Namnet exakt så som det deklarerades i källan.
        /// </summary>
        public required string DeclaredName { get; init; }

        /// <summary>
        /// Identifierar eventuell mappad intern biologisk källa.
        /// </summary>
        public Guid? BiologicalSourceId { get; init; }

        /// <summary>
        /// Anger relationen mellan motparten och den biologiska källan.
        /// </summary>
        public BiologicalSourceRelationshipKind RelationshipKind { get; init; }

        /// <summary>
        /// Anger aktuell mappingstatus.
        /// </summary>
        public ClaimMappingStatus MappingStatus { get; init; }

        /// <summary>
        /// Anger mappingens confidence mellan 0 och 1.
        /// </summary>
        public decimal? MappingConfidence { get; init; }

        /// <summary>
        /// Anger aktuell verifieringsstatus.
        /// </summary>
        public ClaimVerificationStatus VerificationStatus { get; init; }

        /// <summary>
        /// Tidpunkt då claimen först observerades.
        /// </summary>
        public DateTimeOffset FirstObservedUtc { get; init; }

        /// <summary>
        /// Tidpunkt då claimen senast observerades.
        /// </summary>
        public DateTimeOffset LastObservedUtc { get; init; }

        /// <summary>
        /// Tidpunkt då claimen senast verifierades.
        /// </summary>
        public DateTimeOffset? LastVerifiedUtc { get; init; }

        /// <summary>
        /// Interna anteckningar om claimen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Evidens som stödjer dokumentationen av claimen.
        /// </summary>
        public required IReadOnlyList<BiologicalSourceClaimEvidenceResponse> Evidence { get; init; }

        /// <summary>
        /// Geografiska platser som hör till claimen.
        /// </summary>
        public required IReadOnlyList<BiologicalSourceClaimRegionResponse> Regions { get; init; }
    }
}