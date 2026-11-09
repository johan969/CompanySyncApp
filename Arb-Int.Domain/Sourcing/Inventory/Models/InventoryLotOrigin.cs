using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    public class InventoryLotOrigin : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid InventoryLotId { get; set; }
        public Guid GeographicLocationId { get; set; }
        public OriginRole Role { get; set; }
        public OriginVerificationStatus VerificationStatus { get; set; }
        public bool IsPrimary { get; set; }
        public decimal? Proportion { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public InventoryLot InventoryLot { get; set; } = null!;
        public GeographicLocation GeographicLocation { get; set; } = null!;
    }
}
