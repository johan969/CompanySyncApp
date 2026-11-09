

namespace Arb_Int.Domain.Email
{
    public record RenderedEmail(string Subject, string BodyHtml, string? BodyText);
    public sealed record ProviderPersonalization(
        string Email,
        Dictionary<string, string> Data);


    public interface IEmailTemplateRenderer
    {
        RenderedEmail Render(EmailTemplateVariant variant, IReadOnlyDictionary<string, string> tokens);
    }

    public class SimpleTokenEmailTemplateRenderer : IEmailTemplateRenderer
    {
        public RenderedEmail Render(EmailTemplateVariant variant, IReadOnlyDictionary<string, string> tokens)
        {
            string Apply(string input)
            {
                var output = input;
                foreach (var kv in tokens)
                    output = output.Replace(kv.Key, kv.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                return output;
            }

            var subject = Apply(variant.SubjectTemplate);
            var html = Apply(variant.BodyHtmlTemplate);
            var text = variant.BodyTextTemplate is null ? null : Apply(variant.BodyTextTemplate);

            return new RenderedEmail(subject, html, text);
        }
    }
}
