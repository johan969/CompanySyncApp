using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class SupplierQuoteLine : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplierQuoteRevisionId { get; set; }
        public Guid SupplierBusinessRelationshipId { get; set; }
        public Guid? BuyerRfqLineId { get; set; }
        public SupplierLineKind Kind { get; set; }
        public Guid? SupplierMaterialOfferingId { get; set; }
        public Guid? SupplierProcessCapabilityId { get; set; }
        public Guid? SupplySolutionStepId { get; set; }
        public Guid? InventoryLotId { get; set; }
        public int LineNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal OfferedQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? FixedFee { get; set; }
        public decimal? MinimumOrderQuantity { get; set; }
        public DateTimeOffset? AvailableFromUtc { get; set; }
        public DateTimeOffset? AvailableToUtc { get; set; }
        public int? IncotermRuleId { get; set; }
        public string? NamedPlace { get; set; }
        public string? PriceBasis { get; set; }
        public string? SupplierLineReference { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplierQuoteRevision SupplierQuoteRevision { get; set; } = null!;
        public BuyerRfqLine? BuyerRfqLine { get; set; }
        public SupplierMaterialOffering? SupplierMaterialOffering { get; set; }
        public SupplierProcessCapability? SupplierProcessCapability { get; set; }
        public SupplySolutionStep? SupplySolutionStep { get; set; }
        public InventoryLot? InventoryLot { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
        public IncotermRule? IncotermRule { get; set; }
    }
}
