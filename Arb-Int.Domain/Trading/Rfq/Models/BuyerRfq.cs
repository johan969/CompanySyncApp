using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Använder RFQ-roten som ärendet mot köparen. Det matchningsbara innehållet ligger i låsta revisioner.
    /// </summary>
    public class BuyerRfq : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid BuyerBusinessRelationshipId { get; set; }
        public Guid? BuyerDemandId { get; set; }
        public required string RfqNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public BuyerRfqStatus Status { get; set; }
        public BuyerRfqPriority Priority { get; set; }
        public InformationVisibility Visibility { get; set; } = InformationVisibility.InternalOnly;
        public string? BuyerReference { get; set; }
        public Guid? OwnerUserId { get; set; }
        public int LatestRevisionNumber { get; set; }
        public DateTimeOffset? OpenedAtUtc { get; set; }
        public DateTimeOffset? ClosedAtUtc { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship BuyerBusinessRelationship { get; set; } = null!;
        public BuyerDemand? BuyerDemand { get; set; }
        public ICollection<BuyerRfqRevision> Revisions { get; set; } = [];
        /// <summary>
        /// Schemalagda worker-uppgifter som hör till RFQ:n.
        /// </summary>
        public List<BuyerRfqScheduledTaskLink> ScheduledTaskLinks { get; set; } = [];
    }
}
