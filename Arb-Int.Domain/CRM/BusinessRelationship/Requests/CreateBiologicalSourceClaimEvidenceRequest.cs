namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att lägga till evidens på ett biologiskt source-claim.
    /// </summary>
    public sealed class CreateBiologicalSourceClaimEvidenceRequest
    {
        /// <summary>
        /// Anger typen av evidens.
        /// </summary>
        public ClaimEvidenceType EvidenceType { get; init; }

        /// <summary>
        /// Källsystemets stabila namn.
        /// </summary>
        public required string SourceSystem { get; init; }

        /// <summary>
        /// Postens stabila identifierare inom källsystemet.
        /// </summary>
        public required string SourceRecordKey { get; init; }

        /// <summary>
        /// Eventuell URL till källmaterialet.
        /// </summary>
        public string? SourceUrl { get; init; }

        /// <summary>
        /// Tidpunkt då evidensen observerades. Aktuell UTC-tid används om värdet saknas.
        /// </summary>
        public DateTimeOffset? ObservedUtc { get; init; }

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