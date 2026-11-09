using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Email
{
    public class ProviderEmailTemplateMapping : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid TemplateVariantId { get; set; }
        public EmailTemplateVariant TemplateVariant { get; set; } = null!;

        public EmailProvider Provider { get; set; }

        /// <summary>
        /// Provider template id (t.ex. MailerSend template id)
        /// </summary>
        public string ProviderTemplateId { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTimeOffset? LastSyncedAt { get; set; }
        public string? MetadataJson { get; set; }
    }
}