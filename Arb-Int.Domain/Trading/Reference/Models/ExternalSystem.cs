using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class ExternalSystem : NamedReferenceEntity, IEntity<int>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public int Id { get; set; }
        public ExternalSystemKind Kind { get; set; }
        public string? BaseUrl { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
    }
}
