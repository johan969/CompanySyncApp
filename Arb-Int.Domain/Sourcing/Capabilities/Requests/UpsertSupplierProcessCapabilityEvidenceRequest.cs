namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en stabil källobservation som ska skapas eller uppdateras för en supplier process capability.
    /// </summary>
    public sealed class UpsertSupplierProcessCapabilityEvidenceRequest
    {
        /// <summary>
        /// Anger typen av sourcing-evidens.
        /// </summary>
        public SourcingEvidenceType EvidenceType { get; init; }

        /// <summary>
        /// Anger källsystemets stabila namn.
        /// <para>Exempel:<br />
        /// supplier-website<br />
        /// supplier-csv<br />
        /// legacy-mariadb</para>
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
        /// Anger när just denna källobservation först observerades.
        /// </summary>
        public DateTimeOffset FirstObservedUtc { get; init; }

        /// <summary>
        /// Anger när just denna källobservation senast observerades.
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

        /// <summary>
        /// Concurrency-token för capability-aggregatet.
        /// </summary>
        public uint SupplierProcessCapabilityRowVersion { get; init; }
    }
}
