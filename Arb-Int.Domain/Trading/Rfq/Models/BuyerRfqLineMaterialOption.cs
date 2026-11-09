using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqLineMaterialOption : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerRfqLineId { get; set; }
        public MaterialOptionKind OptionKind { get; set; }
        public Guid? MaterialId { get; set; }
        public int? MaterialTypeId { get; set; }
        public Guid? BiologicalSourceId { get; set; }
        public Guid? BiologicalSourceClaimId { get; set; }
        public string? BuyerText { get; set; }
        public string? AlternativeGroupKey { get; set; }
        public decimal? MinimumMappingConfidence { get; set; }
        public int SortOrder { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerRfqLine BuyerRfqLine { get; set; } = null!;
        public Material? Material { get; set; }
        public MaterialType? MaterialType { get; set; }
        public BiologicalSource? BiologicalSource { get; set; }
        public BusinessRelationshipBiologicalSourceClaim? BiologicalSourceClaim { get; set; }
    }
}
