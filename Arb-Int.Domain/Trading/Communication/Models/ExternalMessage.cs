using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Trading
{
    public class ExternalMessage : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid ExternalConversationId { get; set; }
        public Guid? InReplyToMessageId { get; set; }
        public ExternalMessageDirection Direction { get; set; }
        public CommunicationChannel Channel { get; set; }
        public Guid? SenderPersonId { get; set; }
        public Guid? RecipientPersonId { get; set; }
        public Guid? DisclosurePackageRevisionId { get; set; }
        public string? ExternalMessageId { get; set; }
        public string? Subject { get; set; }
        public required string Body { get; set; }
        public DateTimeOffset OccurredAtUtc { get; set; }
        public DateTimeOffset? RecordedAtUtc { get; set; }
        public Guid? RecordedByUserId { get; set; }
        public string? ContentHash { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public ExternalConversation ExternalConversation { get; set; } = null!;
        public ExternalMessage? InReplyToMessage { get; set; }
        public Person? SenderPerson { get; set; }
        public Person? RecipientPerson { get; set; }
        public DisclosurePackageRevision? DisclosurePackageRevision { get; set; }
        public ICollection<ExternalMessage> Replies { get; set; } = [];
        public ICollection<ExternalMessageAttachment> Attachments { get; set; } = [];
    }
}
