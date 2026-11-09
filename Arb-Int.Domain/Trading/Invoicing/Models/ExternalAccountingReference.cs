using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class ExternalAccountingReference : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid CommercialInvoiceId { get; set; }
        public int ExternalSystemId { get; set; }
        public required string ExternalEntityType { get; set; }
        public required string ExternalEntityId { get; set; }
        public string? ExternalEntityNumber { get; set; }
        public string? ExternalUrl { get; set; }
        public string? ExternalVersionToken { get; set; }
        public DateTimeOffset? LastPushedAtUtc { get; set; }
        public DateTimeOffset? LastPulledAtUtc { get; set; }
        public SyncStatus SyncStatus { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public CommercialInvoice CommercialInvoice { get; set; } = null!;
        public ExternalSystem ExternalSystem { get; set; } = null!;
    }
}
