namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller verifieringsstatus och eventuell ny verifieringstid för en supplier process capability.
    /// </summary>
    public sealed class SetSupplierProcessCapabilityVerificationRequest
    {
        /// <summary>
        /// Anger capabilityns nya verification-status.
        /// </summary>
        public SourcingVerificationStatus VerificationStatus { get; init; }
        /// <summary>
        /// Anger tidpunkten för en ny verifieringshändelse när en sådan registreras.
        /// <para>Null behåller tidigare LastVerifiedUtc.</para>
        /// </summary>
        public DateTimeOffset? VerifiedAtUtc { get; init; }
        /// <summary>
        /// Concurrency-token från senast lästa capability.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
