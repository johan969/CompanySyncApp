using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Använder posten för tidsbegränsad och kvantifierad tillgänglighet på ett supplier offering, eventuellt för en viss lot.
    /// </summary>
    public class SupplierOfferingAvailability : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid SupplierMaterialOfferingId { get; set; }
        public Guid? InventoryLotId { get; set; }
        public SupplyAvailabilityStatus Status { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public decimal? MinimumOrderQuantity { get; set; }
        public int? MinimumOrderUnitId { get; set; }
        public DateTimeOffset AvailableFromUtc { get; set; }
        public DateTimeOffset? AvailableToUtc { get; set; }
        public int? LeadTimeDays { get; set; }
        public Guid? AvailabilityLocationId { get; set; }
        public string? SourceReference { get; set; }
        public DateTimeOffset? LastConfirmedAtUtc { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplierMaterialOffering SupplierMaterialOffering { get; set; } = null!;
        public InventoryLot? InventoryLot { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
        public UnitOfMeasure? MinimumOrderUnit { get; set; }
        public GeographicLocation? AvailabilityLocation { get; set; }
    }
}
