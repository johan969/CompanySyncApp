using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Arb_Int.Domain.Email
{


    public sealed class MailerSendOptions
    {
        public string ApiKey { get; set; } = null!;
        public string BaseUrl { get; set; } = "https://api.mailersend.com";
    }

    public sealed class MailerSendEmailSender : IEmailSender
    {
        private readonly HttpClient _http;
        private readonly MailerSendOptions _opts;

        // Om du vill ha personalization även för Cc/Bcc: sätt true.
        private const bool IncludeCcBccInPersonalization = false;

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public MailerSendEmailSender(HttpClient http, IOptions<MailerSendOptions> opts)
        {
            _http = http;
            _opts = opts.Value;

            _http.BaseAddress = new Uri(_opts.BaseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _opts.ApiKey);
        }

        public Task<SendEmailResult> SendRenderedAsync(EmailOutboxMessage msg, CancellationToken cancellationToken)
        {
            return SendInternalAsync(
                msg,
                templateId: null,
                subject: msg.Subject,
                html: msg.BodyHtml,
                text: msg.BodyText,
                cancellationToken);
        }

        public Task<SendEmailResult> SendProviderTemplateAsync(
            EmailOutboxMessage msg,
            IReadOnlyDictionary<string, string> variables,
            IReadOnlyList<object> personalizations,
            CancellationToken cancellationToken)
        {
            var subject = string.IsNullOrWhiteSpace(msg.Subject) ? null : msg.Subject;

            return SendInternalAsync(
                msg,
                templateId: msg.ProviderTemplateId,
                subject: subject,
                html: null,
                text: null,
                cancellationToken);
        }

        private async Task<SendEmailResult> SendInternalAsync(
            EmailOutboxMessage msg,
            string? templateId,
            string? subject,
            string? html,
            string? text,
            CancellationToken cancellationToken)
        {
            if (msg.Provider != EmailProvider.MailerSend)
                return new SendEmailResult(false, null, $"Unsupported provider: {msg.Provider}");

            if (msg.Recipients is null || msg.Recipients.Count == 0)
                return new SendEmailResult(false, null, "No recipients found on outbox message.");

            if (string.IsNullOrWhiteSpace(msg.FromEmailSnapshot))
                return new SendEmailResult(false, null, "FromEmailSnapshot is required.");

            var to = BuildPeople(msg, EmailRecipientType.To);
            if (to.Count == 0)
                return new SendEmailResult(false, null, "At least one TO recipient is required.");

            var cc = BuildPeople(msg, EmailRecipientType.Cc);
            var bcc = BuildPeople(msg, EmailRecipientType.Bcc);

            // Global variables (gäller alla mottagare om inte recipient override:ar)
            var globalVars = DeserializeDictionaryStrict(msg.VariablesJson, "VariablesJson");

            // Personalization från recipients + global vars (recipient override:ar)
            var personalization = BuildPersonalizationFromRecipientsStrict(msg, globalVars);

            var request = new MailerSendEmailRequest
            {
                From = new MailerSendPerson
                {
                    Email = msg.FromEmailSnapshot.Trim(),
                    Name = NullIfWhiteSpace(msg.FromNameSnapshot)
                },
                ReplyTo = string.IsNullOrWhiteSpace(msg.ReplyToEmailSnapshot)
                    ? null
                    : new MailerSendPerson
                    {
                        Email = msg.ReplyToEmailSnapshot!.Trim(),
                        Name = NullIfWhiteSpace(msg.ReplyToNameSnapshot)
                    },

                To = to,
                Cc = cc.Count == 0 ? null : cc,
                Bcc = bcc.Count == 0 ? null : bcc,

                Subject = subject,
                Html = html,
                Text = text,
                TemplateId = NullIfWhiteSpace(templateId),

                // Skicka bara fältet om vi faktiskt har personalization
                Personalization = personalization.Count == 0 ? null : personalization
            };

            // MailerSend kräver: antingen template_id eller html/text
            if (string.IsNullOrWhiteSpace(request.TemplateId)
                && string.IsNullOrWhiteSpace(request.Html)
                && string.IsNullOrWhiteSpace(request.Text))
            {
                return new SendEmailResult(false, null, "MailerSend requires either template_id or html/text content.");
            }

            var json = JsonSerializer.Serialize(request, JsonOpts);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var resp = await _http.PostAsync("v1/email", content, cancellationToken);

            if (resp.IsSuccessStatusCode)
            {
                resp.Headers.TryGetValues("x-message-id", out var values);
                var providerMessageId = values?.FirstOrDefault();
                return new SendEmailResult(true, providerMessageId, null);
            }

            var body = await resp.Content.ReadAsStringAsync(cancellationToken);
            return new SendEmailResult(false, null,
                $"MailerSend send failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
        }

        private static List<MailerSendPerson> BuildPeople(EmailOutboxMessage msg, EmailRecipientType type)
        {
            // Dedup per email, välj den med NameSnapshot om möjligt
            return msg.Recipients
                .Where(r => r.Type == type)
                .Where(r => !string.IsNullOrWhiteSpace(r.EmailSnapshot))
                .GroupBy(r => r.EmailSnapshot.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    var best = g.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.NameSnapshot)) ?? g.First();
                    return new MailerSendPerson
                    {
                        Email = best.EmailSnapshot.Trim(),
                        Name = NullIfWhiteSpace(best.NameSnapshot)
                    };
                })
                .ToList();
        }

        private static List<MailerSendPersonalization> BuildPersonalizationFromRecipientsStrict(
            EmailOutboxMessage msg,
            IReadOnlyDictionary<string, string> globalVars)
        {
            // Vilka recipient-typer ska få personalization?
            var allowedTypes = IncludeCcBccInPersonalization
                ? new HashSet<EmailRecipientType> { EmailRecipientType.To, EmailRecipientType.Cc, EmailRecipientType.Bcc }
                : new HashSet<EmailRecipientType> { EmailRecipientType.To };

            var perEmail = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var r in msg.Recipients.Where(r => allowedTypes.Contains(r.Type)))
            {
                var email = r.EmailSnapshot?.Trim();
                if (string.IsNullOrWhiteSpace(email))
                    continue;

                // starta med global vars
                var data = new Dictionary<string, string>(globalVars, StringComparer.OrdinalIgnoreCase);

                // merge recipient vars (override)
                var recipientVars = DeserializeDictionaryStrict(r.PersonalizationJson, $"Recipient[{email}].PersonalizationJson");
                foreach (var kv in recipientVars)
                    data[kv.Key] = kv.Value;

                if (data.Count == 0)
                    continue;

                // Om samma email förekommer flera gånger: merge, senaste vinner
                if (!perEmail.TryGetValue(email, out var existing))
                {
                    perEmail[email] = data;
                }
                else
                {
                    foreach (var kv in data)
                        existing[kv.Key] = kv.Value;
                }
            }

            return perEmail
                .Select(kvp => new MailerSendPersonalization
                {
                    Email = kvp.Key,
                    Data = kvp.Value
                })
                .ToList();
        }

        private static Dictionary<string, string> DeserializeDictionaryStrict(string? json, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOpts);
                if (dict is null)
                    return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                // Trimma nycklar/värden lite defensivt
                var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in dict)
                {
                    var key = kv.Key?.Trim();
                    if (string.IsNullOrWhiteSpace(key))
                        continue;

                    result[key] = kv.Value ?? string.Empty;
                }
                return result;
            }
            catch (Exception ex)
            {
                // I worker vill du hellre faila tydligt än att “skippa” personalization tyst
                throw new InvalidOperationException($"Invalid JSON in {fieldName}.", ex);
            }
        }

        private static string? NullIfWhiteSpace(string? s)
            => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        #region MailerSend DTOs

        private sealed class MailerSendEmailRequest
        {
            [JsonPropertyName("from")]
            public MailerSendPerson From { get; set; } = null!;

            [JsonPropertyName("to")]
            public List<MailerSendPerson> To { get; set; } = new();

            [JsonPropertyName("cc")]
            public List<MailerSendPerson>? Cc { get; set; }

            [JsonPropertyName("bcc")]
            public List<MailerSendPerson>? Bcc { get; set; }

            [JsonPropertyName("reply_to")]
            public MailerSendPerson? ReplyTo { get; set; }

            [JsonPropertyName("subject")]
            public string? Subject { get; set; }

            [JsonPropertyName("text")]
            public string? Text { get; set; }

            [JsonPropertyName("html")]
            public string? Html { get; set; }

            [JsonPropertyName("template_id")]
            public string? TemplateId { get; set; }

            [JsonPropertyName("personalization")]
            public List<MailerSendPersonalization>? Personalization { get; set; }
        }

        private sealed class MailerSendPerson
        {
            [JsonPropertyName("email")]
            public string Email { get; set; } = null!;

            [JsonPropertyName("name")]
            public string? Name { get; set; }
        }

        private sealed class MailerSendPersonalization
        {
            [JsonPropertyName("email")]
            public string Email { get; set; } = null!;

            [JsonPropertyName("data")]
            public Dictionary<string, string> Data { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        }

        #endregion
    }
}
