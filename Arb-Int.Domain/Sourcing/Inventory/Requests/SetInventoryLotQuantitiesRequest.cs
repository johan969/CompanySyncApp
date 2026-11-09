namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller den aktuella kvantitetsbilden för en inventory lot.
    /// </summary>
    public sealed class SetInventoryLotQuantitiesRequest
    {
        /// <summary>
        /// Anger lotens totala kvantitet.
        /// </summary>
        public decimal TotalQuantity { get; init; }

        /// <summary>
        /// Anger lotens tillgängliga kvantitet.
        /// </summary>
        public decimal AvailableQuantity { get; init; }

        /// <summary>
        /// Anger lotens reserverade kvantitet.
        /// </summary>
        public decimal ReservedQuantity { get; init; }

        /// <summary>
        /// Identifierar måttenheten för samtliga kvantiteter.
        /// </summary>
        public int QuantityUnitId { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa lot.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
