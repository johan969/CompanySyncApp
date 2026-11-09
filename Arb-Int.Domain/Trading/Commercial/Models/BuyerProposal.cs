using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Detta används som Algaebras modererade erbjudande till köparen. Supplieridentiteter och supplierpriser exponeras inte här.
    /// </summary>
    public class BuyerProposal : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid BuyerBusinessRelationshipId { get; set; }
        public Guid BuyerRfqId { get; set; }
        public required string ProposalNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public BuyerProposalStatus Status { get; set; }
        public int LatestRevisionNumber { get; set; }
        public Guid? OwnerUserId { get; set; }
        public DateTimeOffset? IssuedAtUtc { get; set; }
        public DateTimeOffset? ClosedAtUtc { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship BuyerBusinessRelationship { get; set; } = null!;
        public BuyerRfq BuyerRfq { get; set; } = null!;
        public ICollection<BuyerProposalRevision> Revisions { get; set; } = [];
    }
}
