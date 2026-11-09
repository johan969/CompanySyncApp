namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en biologisk avgränsning för en supplier process capability.
    /// </summary>
    public sealed class SupplierProcessCapabilityBiologicalSourceScopeResponse
    {
        /// <summary>
        /// Identifierar scope-länken.
        /// </summary>
        public Guid Id { get; init; }
        /// <summary>
        /// Identifierar supplier-relationen som både capability och claim tillhör.
        /// </summary>
        public Guid BusinessRelationshipId { get; init; }
        /// <summary>
        /// Identifierar capabilityn.
        /// </summary>
        public Guid SupplierProcessCapabilityId { get; init; }
        /// <summary>
        /// Identifierar det biologiska CRM-claim som avgränsar capabilityn.
        /// </summary>
        public Guid BusinessRelationshipBiologicalSourceClaimId { get; init; }
        /// <summary>
        /// Innehåller anteckningar om den biologiska avgränsningen.
        /// </summary>
        public string? Notes { get; init; }
    }
}
