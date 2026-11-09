using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Email
{
    public class EmailOutboxRecipient : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid OutboxMessageId { get; set; }
        public EmailOutboxMessage OutboxMessage { get; set; } = null!;

        // Referens till user (UserSettings.Id / Identity.User.Id)
        // TODO: Ändra så att mottagare är ContactPoint istället för UserId
        public Guid UserId { get; set; }

        public EmailRecipientType Type { get; set; }

        // Snapshot (rekommenderas)
        public string EmailSnapshot { get; set; } = null!;
        public string? NameSnapshot { get; set; }

        // Provider-personalization per mottagare (valfritt men du ville ha det)
        public string? PersonalizationJson { get; set; }

        // Auditable (om du använder)
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}