namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar metadata för en inventory lot.
    /// </summary>
    public sealed class UpdateInventoryLotRequest
    {
        /// <summary>
        /// Anger leverantörens lotnummer när det är känt.
        /// </summary>
        public string? SupplierLotNumber { get; init; }

        /// <summary>
        /// Anger lotens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Beskriver loten.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Anger skördetid när den är känd.
        /// </summary>
        public DateTimeOffset? HarvestedAtUtc { get; init; }

        /// <summary>
        /// Anger produktionstid när den är känd.
        /// </summary>
        public DateTimeOffset? ProducedAtUtc { get; init; }

        /// <summary>
        /// Anger best-before-tid när den är känd.
        /// </summary>
        public DateTimeOffset? BestBeforeUtc { get; init; }

        /// <summary>
        /// Anger expiration-tid när den är känd.
        /// </summary>
        public DateTimeOffset? ExpiresAtUtc { get; init; }

        /// <summary>
        /// Identifierar lotens nuvarande geografiska plats när den är känd.
        /// </summary>
        public Guid? CurrentLocationId { get; init; }

        /// <summary>
        /// Anger lager- eller warehouse-referens när sådan finns.
        /// </summary>
        public string? WarehouseReference { get; init; }

        /// <summary>
        /// Innehåller interna anteckningar om loten.
        /// </summary>
        public string? InternalNotes { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa lot.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
