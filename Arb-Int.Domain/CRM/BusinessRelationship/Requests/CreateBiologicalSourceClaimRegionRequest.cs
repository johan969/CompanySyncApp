namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att lägga till en geografisk plats på ett claim.
    /// </summary>
    public sealed class CreateBiologicalSourceClaimRegionRequest
    {
        /// <summary>
        /// Identifierar den geografiska platsen.
        /// </summary>
        public Guid GeographicLocationId { get; init; }

        /// <summary>
        /// Anger platsens funktion för claimen.
        /// </summary>
        public BiologicalSourceClaimLocationRole Role { get; init; }

        /// <summary>
        /// Anger om platsen ska vara claimens primära region.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Interna anteckningar om platskopplingen.
        /// </summary>
        public string? Notes { get; init; }
    }
}