namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att skapa ett biologiskt source-claim.
    /// </summary>
    public sealed class CreateBiologicalSourceClaimRequest
    {
        /// <summary>
        /// Namnet exakt så som det angavs i källan.
        /// </summary>
        public required string DeclaredName { get; init; }

        /// <summary>
        /// Identifierar eventuell redan känd intern biologisk källa.
        /// </summary>
        public Guid? BiologicalSourceId { get; init; }

        /// <summary>
        /// Anger relationen mellan motparten och den biologiska källan.
        /// </summary>
        public BiologicalSourceRelationshipKind RelationshipKind { get; init; }

        /// <summary>
        /// Anger initial mappingstatus.
        /// </summary>
        public ClaimMappingStatus MappingStatus { get; init; } = ClaimMappingStatus.Unmapped;

        /// <summary>
        /// Anger initial mapping-confidence mellan 0 och 1.
        /// </summary>
        public decimal? MappingConfidence { get; init; }

        /// <summary>
        /// Anger initial verifieringsstatus.
        /// </summary>
        public ClaimVerificationStatus VerificationStatus { get; init; } = ClaimVerificationStatus.Unverified;

        /// <summary>
        /// Tidpunkt då claimen först observerades.
        /// </summary>
        public DateTimeOffset? FirstObservedUtc { get; init; }

        /// <summary>
        /// Tidpunkt då claimen senast observerades.
        /// </summary>
        public DateTimeOffset? LastObservedUtc { get; init; }

        /// <summary>
        /// Tidpunkt då claimen senast verifierades.
        /// </summary>
        public DateTimeOffset? LastVerifiedUtc { get; init; }

        /// <summary>
        /// Interna anteckningar om claimen.
        /// </summary>
        public string? Notes { get; init; }
    }
}