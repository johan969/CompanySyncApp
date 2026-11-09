namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar metadata för en supplier process capability.
    /// </summary>
    public sealed class UpdateSupplierProcessCapabilityRequest
    {
        /// <summary>
        /// Anger capabilityns visningsnamn.
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// Anger processnamnet exakt så som det observerades eller deklarerades.
        /// </summary>
        public required string DeclaredProcessName { get; init; }
        /// <summary>
        /// Beskriver den kända eller möjliga processkapaciteten.
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// Innehåller interna eller sourcingrelaterade anteckningar.
        /// </summary>
        public string? Notes { get; init; }
        /// <summary>
        /// Anger när capabilityn först observerades.
        /// </summary>
        public DateTimeOffset FirstObservedUtc { get; init; }
        /// <summary>
        /// Anger när capabilityn senast observerades.
        /// </summary>
        public DateTimeOffset LastObservedUtc { get; init; }
        /// <summary>
        /// Concurrency-token från senast lästa capability.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
