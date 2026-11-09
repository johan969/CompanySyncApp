using Arb_Int.Domain.Email;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Schemalägger kontoinvitationer med samma Identity-tokenmekanism som lösenordsåterställning.
    /// <para>Nuvarande provider-template för password reset återanvänds tills en separat UserInvitation-template finns.</para>
    /// </summary>
    public sealed class UserInvitationEmailService : IUserInvitationEmailService
    {
        private readonly Arb_IntDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSchedulingService _emailSchedulingService;
        private readonly IEmailOutboxBuilder _outboxBuilder;

        /// <summary>
        /// Skapar invitationstjänsten med Identity-, konfigurations- och e-postberoenden.
        /// </summary>
        public UserInvitationEmailService(Arb_IntDbContext db, UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSchedulingService emailSchedulingService, IEmailOutboxBuilder outboxBuilder)
        {
            _db = db;
            _userManager = userManager;
            _configuration = configuration;
            _emailSchedulingService = emailSchedulingService;
            _outboxBuilder = outboxBuilder;
        }

        /// <summary>
        /// Genererar password-reset-token och skickar länken till den kontaktpunkt som administratören uttryckligen valt.
        /// </summary>
        public async Task<Result> QueueInvitationAsync(ApplicationUser user, Guid contactPointId, string clientUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                return Result.Fail("The application user has no email address.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callback = QueryHelpers.AddQueryString(clientUrl, new Dictionary<string, string?>
            {
                ["token"] = token,
                ["email"] = user.Email
            });

            var name = $"{user.FirstName}".Trim();
            if (string.IsNullOrWhiteSpace(name)) name = $"{user.LastName}".Trim();

            var subject = _configuration.GetValue<string>("ArbIntSettings:UserInvitation_Subject")
                ?? _configuration.GetValue<string>("ArbIntSettings:ResetPassword_ResetText")
                ?? "Set up your account";
            var greeting = _configuration.GetValue<string>("ArbIntSettings:Greeting") ?? string.Empty;
            var bodyText = _configuration.GetValue<string>("ArbIntSettings:UserInvitation_BodyText")
                ?? _configuration.GetValue<string>("ArbIntSettings:ResetPassword_BodyText")
                ?? string.Empty;
            var actionText = _configuration.GetValue<string>("ArbIntSettings:UserInvitation_ActionText")
                ?? _configuration.GetValue<string>("ArbIntSettings:ResetPassword_ResetText")
                ?? "Set password";
            var footerText = _configuration.GetValue<string>("ArbIntSettings:UserInvitation_FooterText")
                ?? _configuration.GetValue<string>("ArbIntSettings:ResetPassword_FooterText")
                ?? string.Empty;
            var systemSignature = _configuration.GetValue<string>("ArbIntSettings:SystemSignature") ?? string.Empty;
            var providerTemplateId = _configuration.GetValue<string>("ArbIntSettings:UserInvitation_ProviderTemplateId") ?? "ynrw7gy0v2jl2k8e";

            var request = new EmailOutboxBuildRequest
            {
                Kind = EmailTemplateKind.PasswordReset,
                Provider = EmailProvider.MailerSend,
                ContentMode = EmailContentMode.ProviderTemplate,
                FromUserId = _configuration.GetValue<Guid>("ArbIntSettings:SystemUserId"),
                ToContactPointId = contactPointId,
                SubjectTemplate = subject,
                ProviderTemplateId = providerTemplateId,
                PolicyContext = new EmailPolicyContext(ContactId: null, RequiresPassword: false),
                DomainTokenValues = new Dictionary<string, string>(),
                ProviderVariables = new Dictionary<string, string>
                {
                    [TokenKeys.Name] = name,
                    [TokenKeys.ResetLink] = callback,
                    [TokenKeys.AccountName] = _configuration.GetValue<string>("ArbIntSettings:AccountName") ?? string.Empty,
                    [TokenKeys.SupportUrl] = _configuration.GetValue<string>("ArbIntSettings:SupportUrl") ?? string.Empty,
                    [TokenKeys.SystemOwner] = _configuration.GetValue<string>("ArbIntSettings:SystemOwner") ?? string.Empty,
                    [TokenKeys.Greeting] = greeting,
                    [TokenKeys.BodyText] = bodyText,
                    [TokenKeys.ResetText] = actionText,
                    [TokenKeys.SystemSignature] = systemSignature,
                    [TokenKeys.FooterText] = footerText
                }
            };

            var outbox = await _outboxBuilder.BuildAsync(request, cancellationToken);
            await _emailSchedulingService.CreateOutboxAndScheduleAsync(_db, outbox, DateTimeOffset.UtcNow, cancellationToken);
            return Result.Ok();
        }
    }
}
