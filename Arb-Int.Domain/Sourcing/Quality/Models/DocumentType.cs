using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    public class DocumentType : NamedReferenceEntity, IEntity<int>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public int Id { get; set; }
        public DocumentCategory Category { get; set; }
        public bool IsLotSpecificByDefault { get; set; }
        public bool CanExpire { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
    }
}
