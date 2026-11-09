namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller den nya statusen för en supply certification.
    /// </summary>
    public sealed class ChangeSupplyCertificationStatusRequest
    {
        /// <summary>
        /// Anger certificationens nya status.
        /// </summary>
        public CertificationStatus Status { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa certification.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
