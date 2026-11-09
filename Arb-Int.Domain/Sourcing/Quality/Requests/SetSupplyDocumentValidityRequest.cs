namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller ett nytt giltighetsfönster för ett supply document.
    /// </summary>
    public sealed class SetSupplyDocumentValidityRequest
    {
        /// <summary>
        /// Anger när dokumentets giltighet börjar när den är känd.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Anger när dokumentets giltighet slutar när den är känd.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa document.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
