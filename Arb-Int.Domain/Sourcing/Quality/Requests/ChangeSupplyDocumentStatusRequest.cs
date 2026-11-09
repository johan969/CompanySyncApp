namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller den nya statusen för ett supply document.
    /// </summary>
    public sealed class ChangeSupplyDocumentStatusRequest
    {
        /// <summary>
        /// Anger dokumentets nya status.
        /// </summary>
        public DocumentStatus Status { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa document.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
