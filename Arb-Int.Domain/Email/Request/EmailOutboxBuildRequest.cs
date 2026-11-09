namespace Arb_Int.Domain.Email
{
    public sealed class EmailOutboxBuildRequest
    {
        public EmailTemplateKind Kind { get; init; }
        public EmailProvider Provider { get; init; }
        public EmailContentMode ContentMode { get; init; }

        public Guid? PartyId { get; init; }
        public Guid FromUserId { get; init; }
        public Guid ToContactPointId { get; init; }
        public Guid? ReplyToUserId { get; init; }

        public string SubjectTemplate { get; init; } = null!;
        public string BodyHtmlTemplate { get; init; } = null!;
        public string? BodyTextTemplate { get; init; }

        public string? ProviderTemplateId { get; init; }
        public Guid? DomainEmailTemplateId { get; init; }

        public EmailPolicyContext PolicyContext { get; init; } = null!;

        // Domän-token values (#...)
        public IReadOnlyDictionary<string, string> DomainTokenValues { get; init; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Provider variables (för {{...}})
        public IReadOnlyDictionary<string, string> ProviderVariables { get; init; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Per-recipient overrides (valfritt)
        public IReadOnlyDictionary<Guid, IReadOnlyDictionary<string, string>> ProviderPersonalizationByUserId { get; init; }
            = new Dictionary<Guid, IReadOnlyDictionary<string, string>>();
    }
}