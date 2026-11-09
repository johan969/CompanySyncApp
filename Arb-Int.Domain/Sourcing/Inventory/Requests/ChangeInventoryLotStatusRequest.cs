namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller den nya statusen för en inventory lot.
    /// </summary>
    public sealed class ChangeInventoryLotStatusRequest
    {
        /// <summary>
        /// Anger lotens nya status.
        /// </summary>
        public InventoryLotStatus Status { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa lot.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
