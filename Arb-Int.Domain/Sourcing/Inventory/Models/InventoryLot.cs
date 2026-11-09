using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Använder loten för faktisk kvantitet, analys, certifiering, ursprung och allokering i order- och logistikflödet.
    /// </summary>
    public class InventoryLot : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid SupplierBusinessRelationshipId { get; set; }
        public Guid? SupplierMaterialOfferingId { get; set; }
        public Guid? MaterialId { get; set; }
        public required string LotNumber { get; set; }
        public string? SupplierLotNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public InventoryLotStatus Status { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public DateTimeOffset? HarvestedAtUtc { get; set; }
        public DateTimeOffset? ProducedAtUtc { get; set; }
        public DateTimeOffset? BestBeforeUtc { get; set; }
        public DateTimeOffset? ExpiresAtUtc { get; set; }
        public Guid? CurrentLocationId { get; set; }
        public string? WarehouseReference { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship SupplierBusinessRelationship { get; set; } = null!;
        public SupplierMaterialOffering? SupplierMaterialOffering { get; set; }
        public Material? Material { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
        public GeographicLocation? CurrentLocation { get; set; }
        public ICollection<InventoryLotOrigin> Origins { get; set; } = [];
        public ICollection<InventoryLotLink> ParentLinks { get; set; } = [];
        public ICollection<InventoryLotLink> ChildLinks { get; set; } = [];
        public ICollection<SupplierOfferingAvailability> Availabilities { get; set; } = [];
        public ICollection<SupplyCertification> Certifications { get; set; } = [];
        public ICollection<SupplyDocument> Documents { get; set; } = [];
        public ICollection<SupplyAnalysis> Analyses { get; set; } = [];
    }
}
