namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller grunddata för en ny inventory lot.
    /// </summary>
    public sealed class CreateInventoryLotRequest
    {
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
        /// Identifierar offeringet när loten redan kan kopplas till ett supplier material offering.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Identifierar kanoniskt material när det redan är känt.
        /// </summary>
        public Guid? MaterialId { get; init; }

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
        /// Identifierar måttenheten för lotens kvantiteter.
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
    }
}
