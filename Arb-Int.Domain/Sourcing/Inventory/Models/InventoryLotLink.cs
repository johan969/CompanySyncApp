using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    public class InventoryLotLink : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid ParentInventoryLotId { get; set; }
        public Guid ChildInventoryLotId { get; set; }
        public InventoryLotLinkKind Kind { get; set; }
        public decimal? InputQuantity { get; set; }
        public int? QuantityUnitId { get; set; }
        public decimal? YieldPercent { get; set; }
        public Guid? MaterialTransformationId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public InventoryLot ParentInventoryLot { get; set; } = null!;
        public InventoryLot ChildInventoryLot { get; set; } = null!;
        public UnitOfMeasure? QuantityUnit { get; set; }
        public MaterialTransformation? MaterialTransformation { get; set; }
    }
}
