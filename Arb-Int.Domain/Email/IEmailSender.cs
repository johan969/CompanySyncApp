

namespace Arb_Int.Domain.Email
{
    public sealed record SendEmailResult(bool Success, string? ProviderMessageId, string? Error);

    public interface IEmailSender
    {
        Task<SendEmailResult> SendRenderedAsync(EmailOutboxMessage msg, CancellationToken cancellationToken);
        Task<SendEmailResult> SendProviderTemplateAsync(
            EmailOutboxMessage msg,
            IReadOnlyDictionary<string, string> variables,
            IReadOnlyList<object> personalizations,
            CancellationToken cancellationToken);
    }
}