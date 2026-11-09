using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqDeliveryLineAllocation : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerRfqRevisionId { get; set; }
        public Guid BuyerRfqDeliveryId { get; set; }
        public Guid BuyerRfqLineId { get; set; }
        public decimal Quantity { get; set; }
        public int QuantityUnitId { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerRfqDelivery BuyerRfqDelivery { get; set; } = null!;
        public BuyerRfqLine BuyerRfqLine { get; set; } = null!;
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
    }
}
