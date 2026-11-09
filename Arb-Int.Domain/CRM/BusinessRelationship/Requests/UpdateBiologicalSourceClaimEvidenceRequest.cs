namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar metadata för ett claim-evidens.
    /// </summary>
    public sealed class UpdateBiologicalSourceClaimEvidenceRequest
    {
        /// <summary>
        /// Anger typen av evidens.
        /// </summary>
        public ClaimEvidenceType EvidenceType { get; init; }

        /// <summary>
        /// Eventuell URL till källmaterialet.
        /// </summary>
        public string? SourceUrl { get; init; }

        /// <summary>
        /// Tidpunkt då evidensen observerades.
        /// </summary>
        public DateTimeOffset ObservedUtc { get; init; }

        /// <summary>
        /// Interna anteckningar om evidensen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Eventuell hash av källmaterialets innehåll.
        /// </summary>
        public string? ContentHash { get; init; }
    }
}