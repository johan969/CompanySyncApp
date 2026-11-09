namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller en geografisk koppling för ett biologiskt source-claim.
    /// </summary>
    public sealed class BiologicalSourceClaimRegionResponse
    {
        /// <summary>
        /// Identifierar regionkopplingen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar claimen.
        /// </summary>
        public Guid BusinessRelationshipBiologicalSourceClaimId { get; init; }

        /// <summary>
        /// Identifierar den geografiska platsen.
        /// </summary>
        public Guid GeographicLocationId { get; init; }

        /// <summary>
        /// Anger vilken funktion platsen har för claimen.
        /// </summary>
        public BiologicalSourceClaimLocationRole Role { get; init; }

        /// <summary>
        /// Anger om detta är claimens primära geografiska plats.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Interna anteckningar om platskopplingen.
        /// </summary>
        public string? Notes { get; init; }
    }
}