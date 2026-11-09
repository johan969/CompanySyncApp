using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqDelivery : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerRfqRevisionId { get; set; }
        public int DeliveryNumber { get; set; }
        public Guid? DestinationAddressId { get; set; }
        public Guid? DestinationLocationId { get; set; }
        public DateTimeOffset RequestedDeliveryFromUtc { get; set; }
        public DateTimeOffset RequestedDeliveryToUtc { get; set; }
        public int? IncotermRuleId { get; set; }
        public string? NamedPlace { get; set; }
        public decimal? MinimumTemperatureCelsius { get; set; }
        public decimal? MaximumTemperatureCelsius { get; set; }
        public string? PackagingNotes { get; set; }
        public string? DeliveryNotes { get; set; }
        public int SortOrder { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerRfqRevision BuyerRfqRevision { get; set; } = null!;
        public Address? DestinationAddress { get; set; }
        public GeographicLocation? DestinationLocation { get; set; }
        public IncotermRule? IncotermRule { get; set; }
        public ICollection<BuyerRfqDeliveryLineAllocation> LineAllocations { get; set; } = [];
    }
}
