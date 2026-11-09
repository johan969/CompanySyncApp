namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar metadata för en biologisk avgränsning på en supplier process capability.
    /// </summary>
    public sealed class UpdateSupplierProcessCapabilityBiologicalSourceScopeRequest
    {
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
