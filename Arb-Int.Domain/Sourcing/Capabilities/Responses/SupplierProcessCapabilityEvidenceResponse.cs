namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en källobservation som stöder en supplier process capability.
    /// </summary>
    public sealed class SupplierProcessCapabilityEvidenceResponse
    {
        /// <summary>
        /// Identifierar evidensposten.
        /// </summary>
        public Guid Id { get; init; }
        /// <summary>
        /// Identifierar capabilityn.
        /// </summary>
        public Guid SupplierProcessCapabilityId { get; init; }
        /// <summary>
        /// Anger typen av sourcing-evidens.
        /// </summary>
        public SourcingEvidenceType EvidenceType { get; init; }
        /// <summary>
        /// Anger källsystemets stabila namn.
        /// </summary>
        public required string SourceSystem { get; init; }
        /// <summary>
        /// Anger den stabila postnyckeln i källsystemet.
        /// </summary>
        public required string SourceRecordKey { get; init; }
        /// <summary>
        /// Anger källans URL när sådan finns.
        /// </summary>
        public string? SourceUrl { get; init; }
        /// <summary>
        /// Anger när evidensposten först observerades.
        /// </summary>
        public DateTimeOffset FirstObservedUtc { get; init; }
        /// <summary>
        /// Anger när evidensposten senast observerades.
        /// </summary>
        public DateTimeOffset LastObservedUtc { get; init; }
        /// <summary>
        /// Innehåller anteckningar om evidensen.
        /// </summary>
        public string? Notes { get; init; }
        /// <summary>
        /// Anger hash av källinnehållet när sådan finns.
        /// </summary>
        public string? ContentHash { get; init; }
    }
}
