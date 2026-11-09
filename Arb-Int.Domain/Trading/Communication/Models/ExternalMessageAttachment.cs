using Arb_Int.Domain.Base;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class ExternalMessageAttachment : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid ExternalMessageId { get; set; }
        public Guid? SupplyDocumentId { get; set; }
        public required string FileName { get; set; }
        public string? ContentType { get; set; }
        public string? StorageProvider { get; set; }
        public string? StorageKey { get; set; }
        public string? ContentHash { get; set; }
        public long? FileSizeBytes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public ExternalMessage ExternalMessage { get; set; } = null!;
        public SupplyDocument? SupplyDocument { get; set; }
    }
}
