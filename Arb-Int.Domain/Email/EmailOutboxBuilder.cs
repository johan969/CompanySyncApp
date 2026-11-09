using System.Text.Json;

namespace Arb_Int.Domain.Email
{
    public interface IEmailOutboxBuilder
    {
        Task<EmailOutboxMessage> BuildAsync(EmailOutboxBuildRequest req, CancellationToken cancellationToken = default);
    }


    public sealed class EmailOutboxBuilder : IEmailOutboxBuilder
    {
        private readonly EmailOutboxFactory _factory;
        private readonly IEmailTemplatePolicyRegistry _policies;

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public EmailOutboxBuilder(EmailOutboxFactory factory, IEmailTemplatePolicyRegistry policies)
        {
            _factory = factory;
            _policies = policies;
        }

        public async Task<EmailOutboxMessage> BuildAsync(EmailOutboxBuildRequest req, CancellationToken cancellationToken = default)
        {
            var policy = _policies.Get(req.Kind);
            string? subject = string.Empty, html = string.Empty, text = string.Empty;
            if (req.ContentMode == EmailContentMode.RenderedContent)
            {
                // 1) Validate template contains required things (#tokens and/or {{placeholders}})
                var templateRes = policy.ValidateTemplate(req.SubjectTemplate, req.BodyHtmlTemplate, req.PolicyContext);
                if (!templateRes.IsValid)
                    throw new InvalidOperationException(ToMessage(req.Kind, templateRes));

                // 2) Replace domain tokens (#...) with actual values; leave {{...}} placeholders intact for provider
                subject = ReplaceDomainTokens(req.SubjectTemplate, req.DomainTokenValues);
                html = ReplaceDomainTokens(req.BodyHtmlTemplate, req.DomainTokenValues);
                text = req.BodyTextTemplate is null ? null : ReplaceDomainTokens(req.BodyTextTemplate, req.DomainTokenValues);

                // 3) Validate payload content (after domain-token replacement)
                //    IMPORTANT: In your model, policies may still require {{verify_link}} etc to remain.
                //    ValidateRendered checks required tokens/placeholders exist in the outgoing content.
                var renderedRes = policy.ValidateRendered(req.SubjectTemplate, req.BodyHtmlTemplate, req.PolicyContext);
                if (!renderedRes.IsValid)
                    throw new InvalidOperationException(ToMessage(req.Kind, renderedRes));
            }
            else
            {
                subject = ReplaceDomainTokens(req.SubjectTemplate, req.DomainTokenValues);
            }
            // 4) Build outbox + initial recipient(s) via factory (snapshots)
            //    For ProviderTemplate mode, subject/html/text won't be used by provider, but we still keep them sane.
            var outbox = await _factory.CreateOutboxForUserAsync(
                kind: req.Kind,
                fromUserId: req.FromUserId,
                replyToUserId: req.ReplyToUserId,
                toContactPointId: req.ToContactPointId,
                subject: req.ContentMode == EmailContentMode.RenderedContent ? subject : (string.IsNullOrWhiteSpace(subject) ? " " : subject),
                html: req.ContentMode == EmailContentMode.RenderedContent ? html : "<!-- provider template -->",
                text: req.ContentMode == EmailContentMode.RenderedContent ? text : null,
                vars: req.ProviderVariables,
                partyId: req.PartyId,
                ownerUserId: req.FromUserId,
                cancellationToken: cancellationToken);
            outbox.Provider = req.Provider;
            outbox.ContentMode = req.ContentMode;
            outbox.ProviderTemplateId = req.ContentMode == EmailContentMode.ProviderTemplate ? req.ProviderTemplateId : null;

            // 5) Persist global variables consistently
            outbox.VariablesJson = JsonSerializer.Serialize(req.ProviderVariables, JsonOpts);

            // 6) Apply per-recipient personalization JSON (merged global + per-user overrides)
            //    NOTE: outbox.Recipients contains To/Cc/Bcc etc. This works for multiple recipients too.
            foreach (var r in outbox.Recipients)
            {
                var merged = Merge(req.ProviderVariables,
                    req.ProviderPersonalizationByUserId.TryGetValue(r.UserId, out var ov) ? ov : null);

                r.PersonalizationJson = JsonSerializer.Serialize(merged, JsonOpts);
            }

            // 7) Build policy payload list from ALL recipients (snapshots) for strict validation
            var personalizations = BuildProviderPersonalizations(
                globalVars: req.ProviderVariables,
                recipients: outbox.Recipients,
                perUserOverrides: req.ProviderPersonalizationByUserId);

            // 8) Validate provider payload (does provider have required values for {{...}}?)
            //    ProviderTemplateId is only required if you are actually sending provider templates.
            var providerTemplateIdForValidation =
                req.ContentMode == EmailContentMode.ProviderTemplate
                    ? req.ProviderTemplateId
                    : null;

            var providerRes = policy.ValidateProviderPayload(
                req.Provider,
                providerTemplateIdForValidation,
                req.ProviderVariables,
                personalizations,
                req.PolicyContext);

            if (!providerRes.IsValid)
                throw new InvalidOperationException(ToMessage(req.Kind, providerRes));

            return outbox;
        }

        private static string ReplaceDomainTokens(string input, IReadOnlyDictionary<string, string> domainTokenValues)
        {
            var s = input;
            foreach (var kv in domainTokenValues)
            {
                // kv.Key should be "#link" etc.
                s = s.Replace(kv.Key, kv.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            return s;
        }

        private static IReadOnlyDictionary<string, string> Merge(
            IReadOnlyDictionary<string, string> global,
            IReadOnlyDictionary<string, string>? overrides)
        {
            if (overrides is null || overrides.Count == 0)
                return global;

            var d = new Dictionary<string, string>(global, StringComparer.OrdinalIgnoreCase);
            foreach (var kv in overrides)
                d[kv.Key] = kv.Value;

            return d;
        }

        private static IReadOnlyList<ProviderPersonalization> BuildProviderPersonalizations(
            IReadOnlyDictionary<string, string> globalVars,
            IReadOnlyList<EmailOutboxRecipient> recipients,
            IReadOnlyDictionary<Guid, IReadOnlyDictionary<string, string>> perUserOverrides)
        {
            var list = new List<ProviderPersonalization>(recipients.Count);

            foreach (var r in recipients)
            {
                var data = new Dictionary<string, string>(globalVars, StringComparer.OrdinalIgnoreCase);

                if (perUserOverrides.TryGetValue(r.UserId, out var ov))
                {
                    foreach (var kv in ov)
                        data[kv.Key] = kv.Value;
                }

                // If recipient.PersonalizationJson has extra overrides, merge them too (optional).
                // This keeps validation aligned with what you actually persisted.
                if (!string.IsNullOrWhiteSpace(r.PersonalizationJson))
                {
                    try
                    {
                        var extra = JsonSerializer.Deserialize<Dictionary<string, string>>(r.PersonalizationJson!);
                        if (extra is not null)
                        {
                            foreach (var kv in extra)
                                data[kv.Key] = kv.Value;
                        }
                    }
                    catch
                    {
                        // If you want strictness, throw here instead.
                    }
                }

                // IMPORTANT: ProviderPersonalization is a record with required ctor args.
                list.Add(new ProviderPersonalization(
                    Email: r.EmailSnapshot,
                    Data: data));
            }

            return list;
        }

        private static string ToMessage(EmailTemplateKind kind, PolicyValidationResult r)
            => $"Policy validation failed for {kind}: " +
               string.Join("; ", r.Errors.Select(e => $"{e.Code}: {e.Message}"));
    }
}
