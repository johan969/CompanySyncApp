using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqLine : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerRfqRevisionId { get; set; }
        public Guid? BuyerDemandScopeId { get; set; }
        public int LineNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string? BuyerLineReference { get; set; }
        public string? BuyerOriginalText { get; set; }
        public Guid? MaterialId { get; set; }
        public int? MaterialTypeId { get; set; }
        public Guid? IntendedUseCharacteristicId { get; set; }
        public decimal RequiredQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public decimal? LowerQuantityTolerancePercent { get; set; }
        public decimal? UpperQuantityTolerancePercent { get; set; }
        public bool AllowPartialFulfillment { get; set; }
        public bool AllowMultipleSuppliers { get; set; }
        public bool AllowMultipleOrigins { get; set; }
        public bool AllowMultipleLots { get; set; }
        public bool RequireSingleSupplier { get; set; }
        public bool RequireSingleOrigin { get; set; }
        public bool RequireSingleLot { get; set; }
        public bool RequireUniformCertification { get; set; }
        public bool RequireUniformSpecification { get; set; }
        public int? MaximumSupplierCount { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerRfqRevision BuyerRfqRevision { get; set; } = null!;
        public BuyerDemandScope? BuyerDemandScope { get; set; }
        public Material? Material { get; set; }
        public MaterialType? MaterialType { get; set; }
        public Characteristic? IntendedUseCharacteristic { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
        public ICollection<BuyerRfqLineMaterialOption> MaterialOptions { get; set; } = [];
        public ICollection<BuyerRfqRequirement> Requirements { get; set; } = [];
        public ICollection<BuyerRfqDeliveryLineAllocation> DeliveryAllocations { get; set; } = [];
    }
}
