namespace Arb_Int.Domain.Email
{
    public enum EmailProvider
    {
        None = 0,
        MailerSend = 1,
        SendGrid = 2,
        Smtp = 3
    }
    public enum EmailContentMode
    {
        RenderedContent = 0,   // Subject/BodyHtml används
        ProviderTemplate = 1   // ProviderTemplateId + variables/personalizations används
    }
    public enum EmailOutboxStatus
    {
        Draft = 0,
        Pending = 1,
        Sending = 2,
        Sent = 3,
        Failed = 4,
        Cancelled = 5
    }
    public enum EmailTemplateScope
    {
        System = 0,
        Organisation = 1,
        User = 2
    }
    public enum EmailTemplateKind
    {
        Generic = 0,
        AssessmentInvitation = 1,
        AssessmentReminder = 2,
        TwoStepVerification = 3,
        EmailVerification = 4,
        PasswordReset = 5,
        SupportTicketCreated = 6,
        SupportTicketReply = 7,
        SupportTicketInternalNote = 8,
        SupportTicketStatusChanged = 9,
        SupportTicketEscalated = 10
    }

    public enum EmailRecipientType
    {
        To = 0,
        Cc = 1,
        Bcc = 2
    }
}