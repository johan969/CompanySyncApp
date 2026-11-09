using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Email
{
    public class EmailTemplateVariant : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid TemplateId { get; set; }
        public EmailTemplate Template { get; set; } = null!;

        // "sv-SE", "sv", "en-US" etc
        public string Culture { get; set; } = null!;

        public string SubjectTemplate { get; set; } = null!;
        public string BodyHtmlTemplate { get; set; } = null!;
        public string? BodyTextTemplate { get; set; }

        // Om du vill ha "default"-variant
        public bool IsDefault { get; set; }

        // Draft vs Active (så du kan spara halvfärdig variant men inte aktivera)
        public bool IsActive { get; set; } = true;
        public ICollection<ProviderEmailTemplateMapping> ProviderMappings { get; set; } = [];
    }
}