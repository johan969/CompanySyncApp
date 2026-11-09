using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Trading
{
    public class BuyerSalesOrderDelivery : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid BuyerSalesOrderId { get; set; }
        public int DeliveryNumber { get; set; }
        public Guid? DestinationAddressId { get; set; }
        public Guid? DestinationLocationId { get; set; }
        public int? IncotermRuleId { get; set; }
        public string? NamedPlace { get; set; }
        public DateTimeOffset PlannedDeliveryFromUtc { get; set; }
        public DateTimeOffset PlannedDeliveryToUtc { get; set; }
        public DateTimeOffset? ActualDeliveryAtUtc { get; set; }
        public string? DeliveryInstructions { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerSalesOrder BuyerSalesOrder { get; set; } = null!;
        public Address? DestinationAddress { get; set; }
        public GeographicLocation? DestinationLocation { get; set; }
        public IncotermRule? IncotermRule { get; set; }
        public ICollection<BuyerSalesOrderDeliveryLine> Lines { get; set; } = [];
    }
}
