namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller den nya sourcing-statusen för en supplier process capability.
    /// </summary>
    public sealed class ChangeSupplierProcessCapabilityStatusRequest
    {
        /// <summary>
        /// Anger den nya capability-statusen.
        /// </summary>
        public SupplierProcessCapabilityStatus Status { get; init; }
        /// <summary>
        /// Concurrency-token från senast lästa capability.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
