using Arb_Int.Domain.Base;
using Arb_Int.Domain.Economy;

namespace Arb_Int.Domain.Trading
{
    public class BuyerProposalRevision : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerProposalId { get; set; }
        public Guid BuyerRfqId { get; set; }
        public Guid BuyerRfqRevisionId { get; set; }
        public int RevisionNumber { get; set; }
        public BuyerProposalRevisionStatus Status { get; set; }
        public int CurrencyId { get; set; }
        public DateTimeOffset? ValidFromUtc { get; set; }
        public DateTimeOffset? ValidToUtc { get; set; }
        public string? CommercialTerms { get; set; }
        public string? BuyerFacingNotes { get; set; }
        public string? InternalNotes { get; set; }
        public Guid? DisclosurePackageRevisionId { get; set; }
        public DateTimeOffset? IssuedAtUtc { get; set; }
        public Guid? IssuedByUserId { get; set; }
        public string? SnapshotJson { get; set; }
        public string? SnapshotHash { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerProposal BuyerProposal { get; set; } = null!;
        public BuyerRfqRevision BuyerRfqRevision { get; set; } = null!;
        public Currency Currency { get; set; } = null!;
        public DisclosurePackageRevision? DisclosurePackageRevision { get; set; }
        public ICollection<BuyerProposalOption> Options { get; set; } = [];
        public ICollection<BuyerProposalAcceptance> Acceptances { get; set; } = [];
        /// <summary>
        /// Schemalagda worker-uppgifter som hör till revisionen.
        /// </summary>
        public List<BuyerProposalRevisionScheduledTaskLink> ScheduledTaskLinks { get; set; } = [];
    }
}
