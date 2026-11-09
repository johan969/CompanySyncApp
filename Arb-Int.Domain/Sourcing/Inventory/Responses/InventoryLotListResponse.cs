namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller kompakt information om en inventory lot i en lista.
    /// </summary>
    public sealed class InventoryLotListResponse
    {
        /// <summary>
        /// Identifierar loten.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar supplier-relationen.
        /// </summary>
        public Guid SupplierBusinessRelationshipId { get; init; }

        /// <summary>
        /// Identifierar supplier material offering när loten är mappad.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Identifierar kanoniskt material när loten är mappad.
        /// </summary>
        public Guid? MaterialId { get; init; }

        /// <summary>
        /// Anger det internt unika lotnumret.
        /// </summary>
        public required string LotNumber { get; init; }

        /// <summary>
        /// Anger leverantörens lotnummer när det är känt.
        /// </summary>
        public string? SupplierLotNumber { get; init; }

        /// <summary>
        /// Anger lotens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Anger lotens status.
        /// </summary>
        public InventoryLotStatus Status { get; init; }

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
        /// Identifierar måttenheten för kvantiteterna.
        /// </summary>
        public int QuantityUnitId { get; init; }

        /// <summary>
        /// Anger best-before-tid när den är känd.
        /// </summary>
        public DateTimeOffset? BestBeforeUtc { get; init; }

        /// <summary>
        /// Anger expiration-tid när den är känd.
        /// </summary>
        public DateTimeOffset? ExpiresAtUtc { get; init; }

        /// <summary>
        /// Identifierar lotens nuvarande geografiska plats.
        /// </summary>
        public Guid? CurrentLocationId { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
