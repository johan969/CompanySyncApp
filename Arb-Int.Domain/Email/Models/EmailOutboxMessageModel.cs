using Arb_Int.Domain.Base;
using Arb_Int.Domain.Worker;

namespace Arb_Int.Domain.Email
{
        /** <summary>
            <para>Email message model<br />
            <strong>Flowchart for EmailOutbox + EmailTemplate + ScheduledTask:</strong><br /><br /><code><c>
            [EmailTemplate + Variants] <br />
                    | <br />
                    | (API: Create outbox from template + tokens + culture fallback) <br />
                    v <br />
            [EmailOutboxMessage]  (Draft) <br />
            - Subject (rendered) <br />
            - BodyHtml (rendered) <br />
            - Kind (policy) <br />
            - To/Cc/Bcc <br />
                    | <br />
                    | (API: user edits subject/body/recipients) <br />
                    v <br />
            [EmailOutboxMessage] (Draft/Pending) <br />
                    | <br />
                    | (API: Schedule send) <br />
                    v <br />
            [ScheduledTask] (Pending, RunAt=...) <br />
                    | <br />
                    | (Worker pump claims task) <br />
                    v v
            [WorkerService] <br />
            - Load outbox by ScheduledTaskId <br />
            - Validate rendered tokens resolved <br />
            - Send via provider <br />
            - Update statuses + retry on failure <br />
                    | <br />
                    v <br />
            [EmailOutboxMessage] Sent/Failed  +  [ScheduledTask] Succeeded/Failed/Dead </c></code></para>
        </summary> **/
        public class EmailOutboxMessage : IEntity<Guid>
        {
                public Guid Id { get; set; }

                public Guid? PartyId { get; set; }
                public Guid? UserId { get; set; } // owner/skapare (valfritt)

                public EmailTemplateKind Kind { get; set; }

                // From/ReplyTo = referenser
                public Guid FromUserId { get; set; }
                public Guid? ReplyToUserId { get; set; }

                // Snapshots (stabilitet)
                public string FromEmailSnapshot { get; set; } = null!;
                public string? FromNameSnapshot { get; set; }
                public string? ReplyToEmailSnapshot { get; set; }
                public string? ReplyToNameSnapshot { get; set; }

                // Recipients
                public List<EmailOutboxRecipient> Recipients { get; set; } = [];

                // Content/provider
                public EmailContentMode ContentMode { get; set; }
                public EmailProvider Provider { get; set; }
                public string? ProviderTemplateId { get; set; }
                public string? VariablesJson { get; set; }
                public string? PersonalizationsJson { get; set; } // om du vill ha global list också
                public string Subject { get; set; } = null!;
                public string BodyHtml { get; set; } = null!;
                public string? BodyText { get; set; }

                public EmailOutboxStatus Status { get; set; }

                public Guid? ScheduledTaskId { get; set; }
                public ScheduledTask? ScheduledTask { get; set; }

                public DateTimeOffset CreatedAt { get; set; }
                public DateTimeOffset? UpdatedAt { get; set; }
        }
}