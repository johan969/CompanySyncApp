namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar grunddata för ett biologiskt source-claim.
    /// </summary>
    public sealed class UpdateBiologicalSourceClaimRequest
    {
        /// <summary>
        /// Namnet exakt så som det angavs i källan.
        /// </summary>
        public required string DeclaredName { get; init; }

        /// <summary>
        /// Anger relationen mellan motparten och den biologiska källan.
        /// </summary>
        public BiologicalSourceRelationshipKind RelationshipKind { get; init; }

        /// <summary>
        /// Tidpunkt då claimen först observerades.
        /// </summary>
        public DateTimeOffset FirstObservedUtc { get; init; }

        /// <summary>
        /// Tidpunkt då claimen senast observerades.
        /// </summary>
        public DateTimeOffset LastObservedUtc { get; init; }

        /// <summary>
        /// Interna anteckningar om claimen.
        /// </summary>
        public string? Notes { get; init; }
    }
}