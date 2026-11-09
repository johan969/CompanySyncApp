namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en inventory lot med lot-ägda origins och provenance-länkar.
    /// </summary>
    public sealed class InventoryLotResponse
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
        /// Beskriver loten.
        /// </summary>
        public required string Description { get; init; }

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
        /// Identifierar lotens nuvarande geografiska plats.
        /// </summary>
        public Guid? CurrentLocationId { get; init; }

        /// <summary>
        /// Anger warehouse-referens när sådan finns.
        /// </summary>
        public string? WarehouseReference { get; init; }

        /// <summary>
        /// Innehåller interna anteckningar om loten.
        /// </summary>
        public string? InternalNotes { get; init; }

        /// <summary>
        /// Anger när loten skapades i systemet.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }

        /// <summary>
        /// Innehåller geografiska origin-poster för loten.
        /// </summary>
        public required IReadOnlyList<InventoryLotOriginResponse> Origins { get; init; }

        /// <summary>
        /// Innehåller länkar där loten är child eller output.
        /// </summary>
        public required IReadOnlyList<InventoryLotLinkResponse> ParentLinks { get; init; }

        /// <summary>
        /// Innehåller länkar där loten är parent eller input.
        /// </summary>
        public required IReadOnlyList<InventoryLotLinkResponse> ChildLinks { get; init; }

        /// <summary>
        /// Concurrency-token för lot-aggregatet.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
