namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller data för att avgränsa en supplier process capability till ett biologiskt CRM-claim.
    /// </summary>
    public sealed class CreateSupplierProcessCapabilityBiologicalSourceScopeRequest
    {
        /// <summary>
        /// Identifierar claimet som capabilityn ska avgränsas till.
        /// </summary>
        public Guid BusinessRelationshipBiologicalSourceClaimId { get; init; }

        /// <summary>
        /// Innehåller anteckningar om den biologiska avgränsningen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token för capability-aggregatet.
        /// </summary>
        public uint SupplierProcessCapabilityRowVersion { get; init; }
    }
}
