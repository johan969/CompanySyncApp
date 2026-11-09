using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class InvoiceSyncAttempt : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid CommercialInvoiceId { get; set; }
        public int ExternalSystemId { get; set; }
        public SyncDirection Direction { get; set; }
        public SyncStatus Status { get; set; }
        public DateTimeOffset StartedAtUtc { get; set; }
        public DateTimeOffset? CompletedAtUtc { get; set; }
        public string? CorrelationId { get; set; }
        public string? RequestPayloadHash { get; set; }
        public string? ResponsePayloadHash { get; set; }
        public string? ExternalEntityId { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryNumber { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public CommercialInvoice CommercialInvoice { get; set; } = null!;
        public ExternalSystem ExternalSystem { get; set; } = null!;
    }
}
