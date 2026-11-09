namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller kompakt information om en supplier process capability i en lista.
    /// </summary>
    public sealed class SupplierProcessCapabilityListResponse
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
        /// Anger motpartens visningsnamn. Behövs när listan sträcker sig över flera relationer.
        /// </summary>
        public string? SupplierName { get; init; }
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
        /// Anger den normaliserade processens visningsnamn.
        /// </summary>
        public string? ProcessTypeName { get; init; }
        /// <summary>
        /// Identifierar accepterad input-materialtyp när den är känd.
        /// </summary>
        public int? InputMaterialTypeId { get; init; }

        /// <summary>
        /// Anger input-materialtypens visningsnamn.
        /// </summary>
        public string? InputMaterialTypeName { get; init; }
        /// <summary>
        /// Identifierar normal output-materialtyp när den är känd.
        /// </summary>
        public int? OutputMaterialTypeId { get; init; }

        /// <summary>
        /// Anger output-materialtypens visningsnamn.
        /// </summary>
        public string? OutputMaterialTypeName { get; init; }
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
        /// Anger senaste kända observationen.
        /// </summary>
        public DateTimeOffset LastObservedUtc { get; init; }
        /// <summary>
        /// Anger senaste verifieringstid när en sådan finns.
        /// </summary>
        public DateTimeOffset? LastVerifiedUtc { get; init; }
        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
