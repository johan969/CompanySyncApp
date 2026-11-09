namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar information för en geografisk claim-koppling.
    /// </summary>
    public sealed class UpdateBiologicalSourceClaimRegionRequest
    {
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