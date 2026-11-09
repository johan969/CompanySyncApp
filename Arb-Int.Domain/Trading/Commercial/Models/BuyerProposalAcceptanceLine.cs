using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class BuyerProposalAcceptanceLine : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerProposalAcceptanceId { get; set; }
        public Guid BuyerProposalOptionId { get; set; }
        public Guid BuyerProposalLineId { get; set; }
        public decimal AcceptedQuantity { get; set; }
        public int QuantityUnitId { get; set; }
        public bool QuantityChanged { get; set; }
        public string? BuyerNotes { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerProposalAcceptance BuyerProposalAcceptance { get; set; } = null!;
        public BuyerProposalLine BuyerProposalLine { get; set; } = null!;
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
    }
}
