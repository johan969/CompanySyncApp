using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Använder den här modellen som den levande CRM-bilden av vad en köpare antas, indikeras eller bekräftas efterfråga.
    /// En konkret och matchningsbar förfrågan skapas som en BuyerRfq.
    /// </summary>
    public class BuyerDemand : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid BusinessRelationshipId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public BuyerDemandKnowledgeLevel KnowledgeLevel { get; set; }
        public BuyerDemandStatus Status { get; set; }
        public decimal? Confidence { get; set; }
        public DateTimeOffset? ValidFromUtc { get; set; }
        public DateTimeOffset? ValidToUtc { get; set; }
        public DateTimeOffset? LastConfirmedAtUtc { get; set; }
        public Guid? OwnerUserId { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship BusinessRelationship { get; set; } = null!;
        public ICollection<BuyerDemandScope> Scopes { get; set; } = [];
        public ICollection<BuyerDemandEvidence> Evidence { get; set; } = [];
        public ICollection<BuyerDemandStateHistory> StateHistory { get; set; } = [];
    }
}
