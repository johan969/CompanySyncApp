using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    public class BuyerDemandScope : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid BuyerDemandId { get; set; }
        public Guid BusinessRelationshipId { get; set; }
        public Guid? BiologicalSourceClaimId { get; set; }
        public Guid? MaterialId { get; set; }
        public int? MaterialTypeId { get; set; }
        public Guid? IntendedUseCharacteristicId { get; set; }
        public string? DeclaredFormat { get; set; }
        public string? IntendedUseText { get; set; }
        public decimal? TypicalQuantity { get; set; }
        public int? TypicalQuantityUnitId { get; set; }
        public string? RecurrenceDescription { get; set; }
        public int SortOrder { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerDemand BuyerDemand { get; set; } = null!;
        public BusinessRelationshipBiologicalSourceClaim? BiologicalSourceClaim { get; set; }
        public Material? Material { get; set; }
        public MaterialType? MaterialType { get; set; }
        public Characteristic? IntendedUseCharacteristic { get; set; }
        public UnitOfMeasure? TypicalQuantityUnit { get; set; }
    }
}
