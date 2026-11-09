using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public abstract class BuyerRfqRequirement : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerRfqRevisionId { get; set; }
        public Guid? BuyerRfqLineId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public RequirementLevel Level { get; set; }
        public decimal Weight { get; set; }
        public string? AlternativeGroupKey { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
        public string? BuyerOriginalText { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerRfqRevision BuyerRfqRevision { get; set; } = null!;
        public BuyerRfqLine? BuyerRfqLine { get; set; }
    }
}
