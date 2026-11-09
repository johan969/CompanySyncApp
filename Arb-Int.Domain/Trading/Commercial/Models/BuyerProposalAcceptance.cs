using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class BuyerProposalAcceptance : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerProposalRevisionId { get; set; }
        public Guid BuyerProposalOptionId { get; set; }
        public BuyerProposalAcceptanceStatus Status { get; set; }
        public string? BuyerAcceptanceReference { get; set; }
        public DateTimeOffset AcceptedAtUtc { get; set; }
        public Guid? RecordedByUserId { get; set; }
        public bool IsPartialAcceptance { get; set; }
        public DateTimeOffset? InternallyApprovedAtUtc { get; set; }
        public Guid? InternallyApprovedByUserId { get; set; }
        public string? InternalApprovalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerProposalRevision BuyerProposalRevision { get; set; } = null!;
        public BuyerProposalOption BuyerProposalOption { get; set; } = null!;
        public ICollection<BuyerProposalAcceptanceLine> Lines { get; set; } = [];
        public ICollection<BuyerSalesOrder> BuyerSalesOrders { get; set; } = [];
    }
}
