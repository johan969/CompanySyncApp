using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class PaymentStatusSnapshot : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid CommercialInvoiceId { get; set; }
        public int? ExternalSystemId { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public DateTimeOffset ObservedAtUtc { get; set; }
        public string? ExternalPaymentReference { get; set; }
        public string? SnapshotJson { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public CommercialInvoice CommercialInvoice { get; set; } = null!;
        public ExternalSystem? ExternalSystem { get; set; }
    }
}
