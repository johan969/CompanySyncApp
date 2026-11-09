using Arb_Int.Domain.Base;
using Arb_Int.Domain.Economy;

namespace Arb_Int.Domain.Trading
{
    public class BuyerProposalOption : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerProposalRevisionId { get; set; }
        public Guid SupplySolutionCandidateId { get; set; }
        public int OptionNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public BuyerProposalOptionStatus Status { get; set; }
        public decimal TotalNetAmount { get; set; }
        public decimal? TotalTaxAmount { get; set; }
        public decimal TotalGrossAmount { get; set; }
        public int CurrencyId { get; set; }
        public int? EstimatedLeadTimeDays { get; set; }
        public string? BuyerFacingSummary { get; set; }
        public string? InternalMarginSnapshotJson { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerProposalRevision BuyerProposalRevision { get; set; } = null!;
        public SupplySolutionCandidate SupplySolutionCandidate { get; set; } = null!;
        public Currency Currency { get; set; } = null!;
        public ICollection<BuyerProposalLine> Lines { get; set; } = [];
    }
}
