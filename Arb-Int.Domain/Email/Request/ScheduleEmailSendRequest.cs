namespace Arb_Int.Domain.Email
{
    public record ScheduleEmailSendRequest(
        Guid EmailOutboxMessageId,
        DateTimeOffset RunAt
    );
}