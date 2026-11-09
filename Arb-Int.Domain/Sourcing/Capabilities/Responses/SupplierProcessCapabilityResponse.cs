namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en supplier process capability med capability-ägda child-data.
    /// </summary>
    public sealed class SupplierProcessCapabilityResponse
    {
        /// <summary>
        /// Identifierar capabilityn.
        /// </summary>
        public Guid Id { get; init; }
        /// <summary>
        /// Identifierar supplier-relationen.
        /// </summary>
        public Guid BusinessRelationshipId { get; init; }
        /// <summary>
        /// Anger capabilityns stabila nyckel inom supplier-relationen.
        /// </summary>
        public required string CapabilityKey { get; init; }
        /// <summary>
        /// Anger capabilityns visningsnamn.
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// Anger processnamnet så som det deklarerades i källan.
        /// </summary>
        public required string DeclaredProcessName { get; init; }
        /// <summary>
        /// Identifierar normaliserad process när den är känd.
        /// </summary>
        public int? ProcessTypeId { get; init; }
        /// <summary>
        /// Identifierar accepterad input-materialtyp när den är känd.
        /// </summary>
        public int? InputMaterialTypeId { get; init; }
        /// <summary>
        /// Identifierar normal output-materialtyp när den är känd.
        /// </summary>
        public int? OutputMaterialTypeId { get; init; }
        /// <summary>
        /// Beskriver den kända eller möjliga processkapaciteten.
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// Innehåller interna eller sourcingrelaterade anteckningar.
        /// </summary>
        public string? Notes { get; init; }
        /// <summary>
        /// Anger mappingens status.
        /// </summary>
        public SourcingMappingStatus MappingStatus { get; init; }
        /// <summary>
        /// Anger mappingens confidence mellan 0 och 1.
        /// </summary>
        public decimal? MappingConfidence { get; init; }
        /// <summary>
        /// Anger capabilityns sourcing-status.
        /// </summary>
        public SupplierProcessCapabilityStatus Status { get; init; }
        /// <summary>
        /// Anger capabilityns verification-status.
        /// </summary>
        public SourcingVerificationStatus VerificationStatus { get; init; }
        /// <summary>
        /// Anger när capabilityn först observerades.
        /// </summary>
        public DateTimeOffset FirstObservedUtc { get; init; }
        /// <summary>
        /// Anger när capabilityn senast observerades.
        /// </summary>
        public DateTimeOffset LastObservedUtc { get; init; }
        /// <summary>
        /// Anger när capabilityn senast verifierades.
        /// </summary>
        public DateTimeOffset? LastVerifiedUtc { get; init; }
        /// <summary>
        /// Anger när capabilityn skapades i systemet.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }
        /// <summary>
        /// Innehåller biologiska avgränsningar för capabilityn.
        /// </summary>
        public required IReadOnlyList<SupplierProcessCapabilityBiologicalSourceScopeResponse> BiologicalSourceScopes { get; init; }
        /// <summary>
        /// Innehåller evidens som stöder capabilityn.
        /// </summary>
        public required IReadOnlyList<SupplierProcessCapabilityEvidenceResponse> Evidence { get; init; }
        /// <summary>
        /// Concurrency-token för capability-aggregatet.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
