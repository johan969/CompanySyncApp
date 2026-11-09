using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Sparar de affärsmässiga växlingarna mellan antagen, indikerad, bekräftad och ogiltigförklarad efterfrågan.
    /// Den tekniska radversioneringen kompletterar, men ersätter inte, den här historiken.
    /// </summary>
    public class BuyerDemandStateHistory : IEntity<Guid>, ICreateTracker, IActionTracker
    {
        public Guid Id { get; set; }
        public Guid BuyerDemandId { get; set; }
        public BuyerDemandKnowledgeLevel? FromKnowledgeLevel { get; set; }
        public BuyerDemandKnowledgeLevel ToKnowledgeLevel { get; set; }
        public BuyerDemandStatus? FromStatus { get; set; }
        public BuyerDemandStatus ToStatus { get; set; }
        public Guid? BuyerDemandEvidenceId { get; set; }
        public decimal? Confidence { get; set; }
        public DateTimeOffset ChangedAtUtc { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public BuyerDemand BuyerDemand { get; set; } = null!;
        public BuyerDemandEvidence? BuyerDemandEvidence { get; set; }
    }
}
