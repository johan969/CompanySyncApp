namespace Arb_Int.Domain.Email
{
    public record EmailPolicyContext(
        Guid? ContactId,
        bool RequiresPassword);

    public record PolicyValidationError(string Code, string Message);

    public class PolicyValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<PolicyValidationError> Errors { get; } = new();

        public static PolicyValidationResult Ok() => new();
        public static PolicyValidationResult Fail(params PolicyValidationError[] errors)
        {
            var r = new PolicyValidationResult();
            r.Errors.AddRange(errors);
            return r;
        }
    }

    public interface IEmailTemplatePolicy
    {
        EmailTemplateKind Kind { get; }

        PolicyValidationResult ValidateTemplate(string subjectTemplate, string bodyHtmlTemplate, EmailPolicyContext context);
        PolicyValidationResult ValidateRendered(string subject, string bodyHtml, EmailPolicyContext context);

        PolicyValidationResult ValidateProviderPayload(
            EmailProvider provider,
            string? providerTemplateId,
            IReadOnlyDictionary<string, string> variables,
            IReadOnlyList<ProviderPersonalization> personalizations,
            EmailPolicyContext context);
    }
    public abstract class TokenEmailPolicyBase : IEmailTemplatePolicy
    {
        public abstract EmailTemplateKind Kind { get; }

        protected virtual IReadOnlyList<string> RequiredDomainTokens(EmailPolicyContext context) => Array.Empty<string>();
        protected virtual IReadOnlyList<string> RequiredProviderPlaceholders(EmailPolicyContext context) => Array.Empty<string>();
        protected virtual IReadOnlyList<string> RequiredProviderKeys(EmailPolicyContext context) => Array.Empty<string>();

        public PolicyValidationResult ValidateTemplate(string subjectTemplate, string bodyHtmlTemplate, EmailPolicyContext context)
            => ValidateContains(subjectTemplate, bodyHtmlTemplate,
                RequiredDomainTokens(context),
                RequiredProviderPlaceholders(context));

        public PolicyValidationResult ValidateRendered(string subject, string bodyHtml, EmailPolicyContext context)
            => ValidateContains(subject, bodyHtml,
                RequiredDomainTokens(context),
                RequiredProviderPlaceholders(context));

        public PolicyValidationResult ValidateProviderPayload(
            EmailProvider provider,
            string? providerTemplateId,
            IReadOnlyDictionary<string, string> variables,
            IReadOnlyList<ProviderPersonalization> personalizations,
            EmailPolicyContext context)
        {
            var errors = new List<PolicyValidationError>();

            // Här validerar vi bara att "data finns" för placeholders som kräver data.
            foreach (var key in RequiredProviderKeys(context))
            {
                var hasGlobal = variables.TryGetValue(key, out var gv) && !string.IsNullOrWhiteSpace(gv);
                var hasAnyRecipient = personalizations.Any(p => p.Data.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v));

                if (!hasGlobal && !hasAnyRecipient)
                    errors.Add(new("required_provider_value_missing", $"Required provider key '{key}' is missing in variables/personalizations."));
            }

            return errors.Count == 0 ? PolicyValidationResult.Ok() : PolicyValidationResult.Fail(errors.ToArray());
        }

        protected static PolicyValidationResult ValidateContains(
            string subject,
            string bodyHtml,
            IReadOnlyList<string> requiredDomainTokens,
            IReadOnlyList<string> requiredProviderPlaceholders)
        {
            var errors = new List<PolicyValidationError>();

            foreach (var t in requiredDomainTokens)
            {
                if (!Contains(subject, t) && !Contains(bodyHtml, t))
                    errors.Add(new("required_domain_token_missing", $"Required domain token '{t}' is missing in subject/body."));
            }

            foreach (var p in requiredProviderPlaceholders)
            {
                if (!Contains(subject, p) && !Contains(bodyHtml, p))
                    errors.Add(new("required_provider_placeholder_missing", $"Required provider placeholder '{p}' is missing in subject/body."));
            }

            return errors.Count == 0 ? PolicyValidationResult.Ok() : PolicyValidationResult.Fail(errors.ToArray());
        }

        protected static bool Contains(string input, string tokenOrPlaceholder)
            => input?.IndexOf(tokenOrPlaceholder, StringComparison.OrdinalIgnoreCase) >= 0;
    }
    public interface IEmailTemplatePolicyRegistry
    {
        IEmailTemplatePolicy Get(EmailTemplateKind kind);
    }

    public class EmailTemplatePolicyRegistry : IEmailTemplatePolicyRegistry
    {
        private readonly IReadOnlyDictionary<EmailTemplateKind, IEmailTemplatePolicy> _policies;

        public EmailTemplatePolicyRegistry(IEnumerable<IEmailTemplatePolicy> policies)
            => _policies = policies.ToDictionary(p => p.Kind);

        public IEmailTemplatePolicy Get(EmailTemplateKind kind)
            => _policies.TryGetValue(kind, out var p)
                ? p
                : throw new InvalidOperationException($"No policy registered for {kind}.");
    }

    public sealed class TwoStepVerificationEmailPolicy : TokenEmailPolicyBase
    {
        public override EmailTemplateKind Kind => EmailTemplateKind.TwoStepVerification;

        protected override IReadOnlyList<string> RequiredProviderPlaceholders(EmailPolicyContext context)
            => new[] { ProviderPlaceholders.Otp };

        protected override IReadOnlyList<string> RequiredProviderKeys(EmailPolicyContext context)
            => new[] { TokenKeys.Otp };
    }

    public sealed class EmailVerificationEmailPolicy : TokenEmailPolicyBase
    {
        public override EmailTemplateKind Kind => EmailTemplateKind.EmailVerification;

        protected override IReadOnlyList<string> RequiredProviderPlaceholders(EmailPolicyContext context)
            => new[] { ProviderPlaceholders.VerifyLink };

        protected override IReadOnlyList<string> RequiredProviderKeys(EmailPolicyContext context)
            => new[] { TokenKeys.VerifyLink };
    }

    public sealed class PasswordResetEmailPolicy : TokenEmailPolicyBase
    {
        public override EmailTemplateKind Kind => EmailTemplateKind.PasswordReset;

        protected override IReadOnlyList<string> RequiredProviderPlaceholders(EmailPolicyContext context)
            => new[] { ProviderPlaceholders.ResetLink };

        protected override IReadOnlyList<string> RequiredProviderKeys(EmailPolicyContext context)
            => new[] { TokenKeys.ResetLink };
    }
}
