namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller nytt validity-fönster för en supply certification.
    /// </summary>
    public sealed class SetSupplyCertificationValidityRequest
    {
        /// <summary>
        /// Anger när certificationens giltighet börjar när den är känd.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Anger när certificationens giltighet slutar när den är känd.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa certification.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
