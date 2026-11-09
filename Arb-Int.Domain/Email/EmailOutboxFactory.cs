using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Arb_Int.Domain.User;

namespace Arb_Int.Domain.Email
{
    public class EmailOutboxFactory
    {
        private readonly IEmailTemplateRepository _templates;
        private readonly IEmailTemplateVariantRepository _variantRepository;
        private readonly IProviderEmailTemplateMappingRepository _providerMappings;
        private readonly IEmailTemplateRenderer _renderer;
        private readonly IEmailTemplatePolicyRegistry _policies;
        private readonly IEmailOutboxRepository _outbox;
        private readonly IUserSettingsService userService;

        public EmailOutboxFactory(
            IEmailTemplateRepository templates,
            IEmailTemplateVariantRepository variantRepository,
            IProviderEmailTemplateMappingRepository providerMappings,
            IEmailTemplateRenderer renderer,
            IEmailTemplatePolicyRegistry policies,
            IEmailOutboxRepository outbox,
            IUserSettingsService userService)
        {
            _templates = templates;
            _variantRepository = variantRepository;
            _providerMappings = providerMappings;
            _renderer = renderer;
            _policies = policies;
            _outbox = outbox;
            this.userService = userService;
        }
        public async Task FillUserSnapshotsAsync(
    EmailOutboxMessage msg,
            CancellationToken cancellationToken)
        {
            var allUserIds = new HashSet<Guid>
                {
                    msg.FromUserId
                };
            if (msg.ReplyToUserId is Guid r) allUserIds.Add(r);

            foreach (var rec in msg.Recipients)
                allUserIds.Add(rec.UserId);

            var map = await userService.GetByUserIdsAsync(allUserIds, cancellationToken);

            // From
            if (!map.TryGetValue(msg.FromUserId, out var from) || string.IsNullOrWhiteSpace(from.Email))
                throw new ValidationException("FromUserId has no email in UserSettings.");

            msg.FromEmailSnapshot = from.Email!;
            msg.FromNameSnapshot = $"{from.FirstName} {from.LastName}".Trim();

            // ReplyTo
            if (msg.ReplyToUserId is Guid replyId)
            {
                if (!map.TryGetValue(replyId, out var reply) || string.IsNullOrWhiteSpace(reply.Email))
                    throw new ValidationException("ReplyToUserId has no email in UserSettings.");

                msg.ReplyToEmailSnapshot = reply.Email!;
                msg.ReplyToNameSnapshot = $"{reply.FirstName} {reply.LastName}".Trim();
            }

            // Recipients
            foreach (var rec in msg.Recipients)
            {
                if (!map.TryGetValue(rec.UserId, out var us) || string.IsNullOrWhiteSpace(us.Email))
                    throw new ValidationException($"Recipient {rec.UserId} has no email in UserSettings.");

                rec.EmailSnapshot = us.Email!;
                rec.NameSnapshot = $"{us.FirstName} {us.LastName}".Trim();
            }
        }

        public async Task<EmailOutboxMessage> CreateFromTemplateAsync(
            Guid templateId,
            string? culture,
            string to,
            EmailProvider provider,
            IReadOnlyDictionary<string, string> tokens,
            EmailPolicyContext policyContext,
            Guid? organisationId,
            Guid? userId,
            CancellationToken cancellationToken = default)
        {
            var template = await _templates.FindByIdAsync(templateId, cancellationToken)
                ?? throw new InvalidOperationException("Template not found.");

            var variant = await _variantRepository.ResolveAsync(templateId, culture, cancellationToken)
                ?? throw new InvalidOperationException("No active template variant found.");

            var policy = _policies.Get(template.Kind);

            // Validera template (tokens finns)
            var templateValidation = policy.ValidateTemplate(variant.SubjectTemplate, variant.BodyHtmlTemplate, policyContext);
            if (!templateValidation.IsValid)
                throw new ValidationException(string.Join("; ", templateValidation.Errors.Select(e => e.Message)));

            var mapping = provider == EmailProvider.None
            ? null
            : await _providerMappings.GetActiveMappingAsync(variant.Id, provider, cancellationToken);

            EmailOutboxMessage msg;

            msg = new()
            {

            };

            await _outbox.SaveAsync(msg, cancellationToken);

            return msg;
        }

        public async Task<EmailOutboxMessage> CreateOutboxForUserAsync(
        EmailTemplateKind kind,
        Guid fromUserId,
        Guid toContactPointId,
        string subject,
        string html,
        string? text,
        IReadOnlyDictionary<string, string> vars,
        Guid? partyId = null,
        Guid? ownerUserId = null,
        Guid? replyToUserId = null,
        CancellationToken cancellationToken = default)
        {
            // TODO: Det kommer inte att gå att göra på det här sättet då de flesta toContactPointId inte är users utan är Person bara eller t.o.m. ett företag
            // Hämta användare för from + to i en go
            var map = await userService.GetByUserIdsAsync([fromUserId, toContactPointId, replyToUserId ?? Guid.Empty], cancellationToken);

            if (!map.TryGetValue(fromUserId, out var fromSettings) || string.IsNullOrWhiteSpace(fromSettings.Email))
                throw new InvalidOperationException("Missing From user email in UserSettings.");

            if (!map.TryGetValue(toContactPointId, out var toSettings) || string.IsNullOrWhiteSpace(toSettings.Email))
                throw new InvalidOperationException("Missing To user email in UserSettings.");

            string? replyToName = null, replyToEmail = null;
            if (replyToUserId.HasValue)
            {
                if (!map.TryGetValue((Guid)replyToUserId!, out var replyToSettings) || string.IsNullOrWhiteSpace(replyToSettings.Email))
                    throw new InvalidOperationException("Missing ReplyTo user email in UserSettings.");
                replyToName = $"{replyToSettings.FirstName} {replyToSettings.LastName}".Trim();
                replyToEmail = replyToSettings.Email;
            }

            var fromName = string.IsNullOrWhiteSpace($"{fromSettings.FirstName} {fromSettings.LastName}".Trim()) ? "Algaebra" : $"{fromSettings.FirstName} {fromSettings.LastName}".Trim();
            var toName = $"{toSettings.FirstName} {toSettings.LastName}".Trim(); // kan vara null

            var outbox = new EmailOutboxMessage
            {
                PartyId = partyId,
                UserId = ownerUserId ?? fromUserId, // TODO: Ändra så att mottagare är ContactPoint istället för UserId
                Kind = kind,

                FromUserId = fromUserId,
                ReplyToUserId = replyToUserId,

                FromEmailSnapshot = fromSettings.Email!,
                FromNameSnapshot = fromName,

                ReplyToEmailSnapshot = replyToEmail,
                ReplyToNameSnapshot = replyToName,

                Provider = EmailProvider.MailerSend,
                ContentMode = EmailContentMode.RenderedContent,

                Subject = subject,
                BodyHtml = html,
                BodyText = text,

                VariablesJson = JsonSerializer.Serialize(vars),

                Status = EmailOutboxStatus.Draft,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Recipient: To
            outbox.Recipients.Add(new EmailOutboxRecipient
            {
                UserId = toContactPointId, // TODO: Ändra så att mottagare är ContactPoint istället för UserId
                Type = EmailRecipientType.To,
                EmailSnapshot = toSettings.Email!,
                NameSnapshot = toName,
                // Valfritt: per-recipient personalization.
                // Om du vill minimera data kan du sätta null här och låta sendern använda global vars.
                PersonalizationJson = JsonSerializer.Serialize(vars),
                CreatedAt = DateTimeOffset.UtcNow
            });

            return outbox;
        }
    }

}
