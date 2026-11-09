
using Arb_Int.Domain;
using Arb_Int.Domain.Email;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.User;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using NetDevPack.Security.Jwt.Core.Jwa;
using NetDevPack.Security.Jwt.Core.Model;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Arb_Int.Domain.User
{
    public interface IUserService
    {
        Task<Result<AuthJwtResponse>> GenerateAccessTokenAsync(ApplicationUser user, string ipAddress, CancellationToken cancellationToken = default);
        Task<Result<RefreshToken>> GenerateRefreshTokenAsync(string ipAddress, CancellationToken cancellationToken = default);
        //Task<AuthJwtResponse> GetPrincipalFromExpiredToken(string token);
        Task<Result<ClaimsPrincipal>> ValidateJwtTokenAsync(string token, bool validateLifeTime = true, CancellationToken cancellationToken = default);
        Task<Result<bool>> IsJwtTokenValidAsync(string token, bool validateLifeTime = true, CancellationToken cancellationToken = default);
        Task<Result<ClaimsPrincipal>> GetIdentityFromUserIdAsync(string userId, bool extended = false, CancellationToken cancellationToken = default);
        Task<Result<ApplicationUser>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result<UserDetailsResponse>> GetUserDetailsAsync(CancellationToken cancellationToken = default);
        Task<Result<UserDetailsResponse>> GetUserDetailsByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Result<UserDetailsResponse>> UpdateUserDetailsAsync(UpdateUserDetailRequest request, CancellationToken cancellationToken = default);
        Task<Result<UserDetailsResponse>> UpdateUserDetailsByIdAsync(Guid userId, UpdateUserDetailRequest request, CancellationToken cancellationToken = default);
        Task<Result<AuthJwtResponse>> UseRefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken = default);
        //Task<Result> RevokeToken(string token, string ipAddress);

        Task<Microsoft.AspNetCore.Identity.SignInResult> TestSignIn(UserLogin user);
        Task<Result<AuthJwtResponse>> SignInAsync(UserLogin user, CancellationToken cancellationToken = default);
        Task<Result<AuthJwtResponse>> SendEmailConfirmationAsync(string email, string clientUrl, CancellationToken cancellationToken = default);
        Task<Result> EmailConfirmationAsync(string email, string token, CancellationToken cancellationToken = default);
        Task<Result<AuthJwtResponse>> TwoStepVerificationAsync(TwoFactorRequest twoFactorRequest, CancellationToken cancellationToken = default);
        Task<Result> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest, CancellationToken cancellationToken = default);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken = default);
        Task<Result> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken = default);
        Task<Result> LogoutAsync(CancellationToken cancellationToken = default);
        Task<Result<AuthJwtResponse>> RefreshTokenAsync(CancellationToken cancellationToken = default);
        Task<Result<AuthJwtResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<Result<string>> RevokeTokenAsync(string tokenRequest, CancellationToken cancellationToken = default);
    }
    public class UserService : IUserService
    {

        private readonly IJwtService jwtService;
        private readonly Arb_IntDbContext dbContext;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly IEmailSender emailSender;
        private readonly ILogger<UserService> logger;
        private readonly ICurrentUserService currentUserService;
        private readonly IUserSettingsService userSettingsService;
        private readonly IValidationService validationService;
        private readonly IEmailSchedulingService emailSchedulingService;
        private readonly IEmailOutboxBuilder outboxBuilder;
        private readonly IEmailOutboxRepository outboxRepo;
        private readonly IUserAuthContactPointResolver userAuthContactPointResolver;
        private readonly IUserIdentitySynchronizationService userIdentitySynchronizationService;
        private readonly HttpContext? httpContext;
        private Guid? OnBehalfOfUserId { get; set; }

        public UserService(
            Arb_IntDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration,
            IJwtService jwtService,
            IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager,
            ICurrentUserService currentUserService,
            ILogger<UserService> logger,
            IHttpContextAccessor httpContext,
            IUserSettingsService userSettingsService,
            IValidationService validationService,
            IEmailSchedulingService emailSchedulingService,
            IEmailOutboxBuilder outboxBuilder,
            IEmailOutboxRepository outboxRepo,
            IUserAuthContactPointResolver userAuthContactPointResolver,
            IUserIdentitySynchronizationService userIdentitySynchronizationService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.jwtService = jwtService;
            this.emailSender = emailSender;
            this.signInManager = signInManager;
            this.currentUserService = currentUserService;
            this.logger = logger;
            this.httpContext = httpContext.HttpContext;
            this.userSettingsService = userSettingsService;
            this.validationService = validationService;
            this.emailSchedulingService = emailSchedulingService;
            this.outboxBuilder = outboxBuilder;
            this.outboxRepo = outboxRepo;
            this.userAuthContactPointResolver = userAuthContactPointResolver;
            this.userIdentitySynchronizationService = userIdentitySynchronizationService;
        }

        public async Task<Result<AuthJwtResponse>> GenerateAccessTokenAsync(ApplicationUser user, string ipAddress, CancellationToken cancellationToken = default)
        {
            return await GenerateJWT(user, ipAddress, "LoginCredentials", cancellationToken);
        }

        public async Task<Result<RefreshToken>> GenerateRefreshTokenAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            int refreshTokenTTL = 7;
            try
            {
                refreshTokenTTL = configuration.GetValue<int>("AppJwtSettings:RefreshTokenExpirationInDays");
            }
            catch { }

            var refreshToken = new RefreshToken
            {
                Token = await getUniqueToken().ConfigureAwait(false),
                // token is valid for x number of days
                ExpiresAtUtc = DateTimeOffset.UtcNow.AddDays(refreshTokenTTL),
                CreatedAtUtc = DateTimeOffset.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;

            async Task<string> getUniqueToken()
            {
                // token is a cryptographically strong random sequence of values
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                // ensure token is unique by checking against db
                var tokenIsUnique = !await userManager.Users.AnyAsync(u => u.RefreshTokens.Any(t => t.Token == token), cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!tokenIsUnique)
                    return await getUniqueToken();

                return token;
            }
        }

        public async Task<Result<AuthJwtResponse>> UseRefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken = default)
        {
            var user = await GetUserByRefreshToken(token);
            if (user.IsInactivated)
                return Result.Fail(new TokenError("[Error.InvalidToken] Invalid token"));
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                await userManager.UpdateAsync(user).ConfigureAwait(false);
            }

            if (!refreshToken.IsActive)
                return Result.Fail(new TokenError("[Error.InvalidToken] Invalid token"));

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = await RotateRefreshToken(refreshToken, ipAddress).ConfigureAwait(false);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);

            // save changes to db
            await userManager.UpdateAsync(user).ConfigureAwait(false);

            // generate new jwt
            var jwtToken = await GenerateJWT(user, ipAddress, cancellationToken: cancellationToken).ConfigureAwait(false);

            return jwtToken;
        }


        public async Task<Result<ClaimsPrincipal>> ValidateJwtTokenAsync(string token, bool validateLifeTime = true, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(token))
                return Result.Fail(new NullReferenceError($"[Error.NullReference] {nameof(token)} is null"));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = await jwtService.GetCurrentSecurityKey();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidateLifetime = validateLifeTime,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration.GetValue<string>("AppJwtSettings:Issuer"),
                ValidAudience = configuration.GetValue<string>("AppJwtSettings:Audience"),
                IssuerSigningKey = key
            };
            JwtSecurityToken? jwtSecurityToken;
            ClaimsPrincipal? principal;
            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                jwtSecurityToken = securityToken as JwtSecurityToken;
            }
            catch
            {
                return Result.Fail(new TokenError("[Error.InvalidToken] Invalid token"));
            }
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.EcdsaSha512, StringComparison.InvariantCultureIgnoreCase))
                return Result.Fail(new TokenError("[Error.InvalidToken] Invalid token"));
            try
            {
                var userId = principal?.Identities.FirstOrDefault()?.Claims?.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                if (principal != null && !string.IsNullOrEmpty(userId))
                    return Result.Ok(principal);
                return Result.Fail(new NotFoundError(nameof(ApplicationUser), userId ?? "N/A"));
            }
            catch
            {
                return Result.Fail(new ServerError());
            }
        }
        public async Task<Result<bool>> IsJwtTokenValidAsync(string token, bool validateLifeTime = true, CancellationToken cancellationToken = default)
        {
            var result = await ValidateJwtTokenAsync(token, validateLifeTime, cancellationToken);
            if (result.IsSuccess)
                return Result.Ok(true);
            return Result.Fail(result.Errors);
        }
        public async Task<Result<ApplicationUser>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
                return Result.Ok(user);
            return Result.Fail(new NotFoundError(nameof(ApplicationUser), userId));
        }
        public async Task<Result<UserDetailsResponse>> GetUserDetailsAsync(CancellationToken cancellationToken = default)
        {
            if (!currentUserService.UserId.HasValue)
                return Result.Fail(new NotFoundError("UserId", "NULL"));
            return await GetUserDetailsByIdAsync((Guid)currentUserService.UserId!, cancellationToken);
        }
        /// <summary>
        /// Hämtar Identity-profilen tillsammans med eventuell domänperson och aktiva organisationsmedlemskap.
        /// </summary>
        public async Task<Result<UserDetailsResponse>> GetUserDetailsByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization in administrative callers so another user can only be read within permitted organization/person scopes.
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.Fail(new NotFoundError(nameof(ApplicationUser), userId));

            var personContext = await dbContext.Set<PersonUserAccount>()
                .AsNoTracking()
                .Where(x => x.ApplicationUserId == userId)
                .Select(x => new
                {
                    AccountId = x.Id,
                    x.PersonId,
                    x.AuthEmailContactPointId,
                    x.Person.GivenName,
                    x.Person.FamilyName,
                    x.Status,
                    x.IsExternalUser,
                    x.Person.PreferredLanguageCode,
                    Organizations = x.Person.OrganizationLinks
                        .Where(link => link.ValidToUtc == null)
                        .OrderByDescending(link => link.Organization.IsInternal == true)
                        .ThenBy(link => link.Organization.Name)
                        .Select(link => new UserOrganizationMembershipResponse
                        {
                            OrganizationPersonId = link.Id,
                            OrganizationId = link.OrganizationId,
                            OrganizationName = link.Organization.Name,
                            IsInternal = link.Organization.IsInternal,
                            RelationshipType = link.RelationshipType,
                            JobTitle = link.JobTitle,
                            Department = link.Department,
                            IsPrimaryContact = link.IsPrimaryContact,
                            ValidFromUtc = link.ValidFromUtc
                        })
                        .ToList()
                })
                .SingleOrDefaultAsync(cancellationToken);

            var claimsPrincipal = await GetIdentityFromUserIdAsync(userId.ToString(), true, cancellationToken);
            var response = new UserDetailsResponse
            {
                Id = user.Id,
                FirstName = personContext?.GivenName ?? user.FirstName,
                LastName = personContext?.FamilyName ?? user.LastName,
                UserName = user.UserName ?? "N/A",
                Email = user.Email ?? "N/A",
                AuthEmailContactPointId = personContext?.AuthEmailContactPointId,
                Is2StepVerificationRequired = user.TwoFactorEnabled,
                CanToggle2StepVerification = false,
                IsEmailConfirmed = user.EmailConfirmed,
                IsPasswordChangeRequired = user.IsPasswordChangeRequired,
                IsInactivated = user.IsInactivated,
                PersonId = personContext?.PersonId,
                PersonUserAccountId = personContext?.AccountId,
                PersonUserAccountStatus = personContext?.Status,
                IsExternalUser = personContext?.IsExternalUser,
                PreferredLanguageCode = personContext?.PreferredLanguageCode,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                PhoneNumber = user.PhoneNumber,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
                AccessFailedCount = user.AccessFailedCount,
                Organizations = personContext?.Organizations ?? [],
                Claims = claimsPrincipal.IsSuccess
                    ? [.. claimsPrincipal.Value.Claims.Select(x => new UserClaim { Type = x.Type, Value = x.Value })]
                    : []
            };

            return Result.Ok(response);
        }
        public async Task<Result<UserDetailsResponse>> UpdateUserDetailsAsync(UpdateUserDetailRequest request, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Updating user {userId} with {request}", currentUserService.UserId, request);
            return await UpdateUserDetailsByIdAsync((Guid)currentUserService.UserId!, request, cancellationToken);
        }
        /// <summary>
        /// Uppdaterar Identity-profilen, duplicerade personnamen och eventuell vald auth-e-postkontaktpunkt.
        /// </summary>
        public async Task<Result<UserDetailsResponse>> UpdateUserDetailsByIdAsync(Guid userId, UpdateUserDetailRequest request, CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization in administrative callers so another user can only be updated within permitted organization/person scopes.
            logger.LogInformation("Updating user {userId} with synchronized profile details", userId);

            var validationResult = await validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed)
                return Result.Fail<UserDetailsResponse>(validationResult.Errors);

            var synchronizationResult = await userIdentitySynchronizationService.UpdateProfileAsync(userId, request, cancellationToken);
            if (synchronizationResult.IsFailed)
                return Result.Fail<UserDetailsResponse>(synchronizationResult.Errors);

            return await GetUserDetailsByIdAsync(userId, cancellationToken);
        }

        public async Task<Result<ClaimsPrincipal>> GetIdentityFromUserIdAsync(string userId, bool extended = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var userResult = await GetUserByIdAsync(userId, cancellationToken);
                if (userResult.IsSuccess)
                {
                    var user = userResult.Value;
                    var claims = await userManager.GetClaimsAsync(user);
                    var identityClaims = extended ? await GetExtendedUserClaims(claims, user, authenticationType: "apiKey", cancellationToken: cancellationToken) : await GetUserClaims(claims, user, authenticationType: "apiKey", cancellationToken: cancellationToken);
                    if (identityClaims.IsSuccess)
                        return Result.Ok(new ClaimsPrincipal(identityClaims.Value));
                    return Result.Fail(identityClaims.Errors);
                }
                return Result.Fail(userResult.Errors);
            }
            catch
            {
                return Result.Fail(new ServerError());
            }
        }

        public async Task<Result<ClaimsIdentity>> GetExtendedUserClaims(ICollection<Claim> claims, ApplicationUser user, string authenticationType, CancellationToken cancellationToken = default)
        {
            var userRoles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            IList<Claim> roleClaims = [];
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
                ApplicationRole? identityRole = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == userRole, cancellationToken: cancellationToken);
                if (identityRole != null)
                    foreach (var item in await roleManager.GetClaimsAsync(identityRole))
                    {
                        roleClaims.Add(item);
                    }
                ;
            }
            foreach (var claim in roleClaims)
            {
                claims.Add(claim);
            }
            var identityClaims = new ClaimsIdentity(claims, authenticationType: authenticationType ?? "JWT", nameType: Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.UniqueName, roleType: null);
            return Result.Ok(identityClaims);
        }

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> TestSignIn(UserLogin user)
        {
            return await signInManager.PasswordSignInAsync(user.Email, user.Password, false, true).ConfigureAwait(false);
        }
        public async Task<Result<AuthJwtResponse>> SignInAsync(UserLogin user, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Trying to sign in user {user} with ip={ipAddress} and userAgent={userAgent}", user.Email, IpAddress(), UserAgent());
            var result = await TestSignIn(user);
            if (result == null)
                return Result.Fail(new LoginError());
            var ApplicationUser = await userManager.FindByEmailAsync(user.Email);
            if (ApplicationUser == null)
                return Result.Fail(new LoginError());
            if (ApplicationUser.IsInactivated)
                return Result.Fail(new LoginError("[Error.LoginInactivated] Account is inactive"));
            if (result.Succeeded)
            {
                if (!await userManager.IsEmailConfirmedAsync(ApplicationUser))
                    return Result.Ok(new AuthJwtResponse { IsEmailConfirmationNeeded = true });
                if (await userManager.GetTwoFactorEnabledAsync(ApplicationUser))
                    return await GenerateOTPFor2StepVerification(ApplicationUser, cancellationToken);
                // TODO: Check if there's an agreement pending for this user and if so, create a signAgreementToken that can be used on the following signAgreement request
                try
                {
                    var response = await GenerateAccessTokenAsync(ApplicationUser, IpAddress() ?? "N/A", cancellationToken).ConfigureAwait(false);
                    if (response.IsSuccess)
                    {
                        if (response.Value.RefreshToken != null)
                            SetTokenCookie(response.Value.RefreshToken);
                        return Result.Ok(response.Value);
                    }
                    return Result.Fail(response.Errors);
                }
                catch (Exception ex)
                {
                    return Result.Fail(new LoginError($"[Error.LoginException] {ex.Message}"));
                }
                //return Ok(await GenerateJWT(ApplicationUser, true));
            }
            else if (result.IsLockedOut)
                return Result.Fail(new LoginError("[Error.LoginLockedOut] Account locked"));
            else if (result.IsNotAllowed)
            {
                if (ApplicationUser != null && !ApplicationUser.EmailConfirmed)
                {
                    return Result.Ok(new AuthJwtResponse { IsEmailConfirmationNeeded = true });
                }
                return Result.Fail(new LoginError("[Error.LoginError] Login was not allowed"));
            }
            else if (result.RequiresTwoFactor)
            {
                return await GenerateOTPFor2StepVerification(ApplicationUser);
            }
            return Result.Fail(new LoginError());
        }

        public async Task<Result<AuthJwtResponse>> SendEmailConfirmationAsync(string email, string clientUrl, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user != null)
                return await GenerateOTPForEmailVerification(user, clientUrl);
            return Result.Ok(new AuthJwtResponse { IsEmailConfirmationNeeded = true }); // Sends a true statement instead of NotFound to avoid single out
        }

        public async Task<Result> EmailConfirmationAsync(string email, string token, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Fail(new ProcessError("[Error.InvalidEmailConfirmationRequest] Invalid Email Confirmation Request"));

            var confirmResult = await userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
                return Result.Fail(new ProcessError("[Error.InvalidEmailConfirmationRequest] Invalid Email Confirmation Request"));

            return Result.Ok();
        }

        public async Task<Result<AuthJwtResponse>> TwoStepVerificationAsync(TwoFactorRequest twoFactorRequest, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(twoFactorRequest.Email);
            if (user is null)
                return Result.Fail(new BadRequestError());

            var validVerification = await userManager.VerifyTwoFactorTokenAsync(user, twoFactorRequest.Provider, twoFactorRequest.Token);
            if (!validVerification)
                return Result.Fail(new BadRequestError());

            var response = await GenerateAccessTokenAsync(user, IpAddress() ?? "N/A", cancellationToken);
            if (response.IsSuccess)
            {
                if (response.Value.RefreshToken != null)
                    SetTokenCookie(response.Value.RefreshToken);
                return Result.Ok(response.Value);
            }
            return Result.Fail(new LoginError());
        }
        /// <summary>
        /// <para>Variables:<br />
        /// {{greeting}} - Hälsning<br />
        /// {{name}} - Mottagarens namn/identifierare eller tomt<br />
        /// {{body_text}} - Instruktion<br />
        /// {{reset_link}} - Länk till verifikation<br />
        /// {{reset_text}} - Text på verifikationsknappen<br />
        /// {{system_signature}} - Systemsignatur<br />
        /// {{footer_text}} - Text i foten (support)<br />
        /// {{current_year}} - Nuvarande år<br />
        /// {{system_owner}} - Systemägaren (default = Algaebra)<br />
        /// {{all_rights_reserved}} - Copyright (All rights reserved)</para>
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email ?? "N/A");
            if (user is null)
                return Result.Fail(new BadRequestError());

            var authContactPointResult = await userAuthContactPointResolver.ResolveAsync(user.Id, cancellationToken);
            if (authContactPointResult.IsFailed)
                return Result.Fail(authContactPointResult.Errors);

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

#if DEBUG
            Console.WriteLine($"Reset password token för {user.Email}: {token}");
#endif

            var callback = QueryHelpers.AddQueryString(request.ClientUrl, new Dictionary<string, string?>
            {
                ["token"] = token,
                ["email"] = request.Email
            });

            var name = $"{user.FirstName}".Trim();
            if (string.IsNullOrWhiteSpace(name)) name = $"{user.LastName}".Trim();

            string systemSignature = configuration.GetValue<string>("ArbIntSettings:SystemSignature")!;

            string greeting = configuration.GetValue<string>("ArbIntSettings:Greeting")!;

            string bodyText = configuration.GetValue<string>("ArbIntSettings:ResetPassword_BodyText")!;

            string footerText = configuration.GetValue<string>("ArbIntSettings:ResetPassword_FooterText")!;

            string resetText = configuration.GetValue<string>("ArbIntSettings:ResetPassword_ResetText")!;
            // Bygg outbox (policy kräver {{reset_link}})
            var req = new EmailOutboxBuildRequest
            {
                Kind = EmailTemplateKind.PasswordReset,
                Provider = EmailProvider.MailerSend,
                ContentMode = EmailContentMode.ProviderTemplate, // eller ProviderTemplate

                FromUserId = configuration.GetValue<Guid>("ArbIntSettings:SystemUserId")!,
                ToContactPointId = authContactPointResult.Value.ContactPointId,

                SubjectTemplate = resetText,


                ProviderTemplateId = "ynrw7gy0v2jl2k8e",

                PolicyContext = new EmailPolicyContext(ContactId: null, RequiresPassword: false),

                // domain tokens: inga i detta mail
                DomainTokenValues = new Dictionary<string, string>(),

                // provider vars för {{...}}
                ProviderVariables = new Dictionary<string, string>
                {
                    [TokenKeys.Name] = name,
                    [TokenKeys.ResetLink] = callback,
                    [TokenKeys.AccountName] = configuration.GetValue<string>("ArbIntSettings:AccountName")!,
                    [TokenKeys.SupportUrl] = configuration.GetValue<string>("ArbIntSettings:SupportUrl")!,
                    [TokenKeys.SystemOwner] = configuration.GetValue<string>("ArbIntSettings:SystemOwner")!,
                    [TokenKeys.Greeting] = greeting,
                    [TokenKeys.BodyText] = bodyText,
                    [TokenKeys.ResetText] = resetText,
                    [TokenKeys.SystemSignature] = systemSignature,
                    [TokenKeys.FooterText] = footerText,
                }
            };

            var outbox = await outboxBuilder.BuildAsync(req, cancellationToken);

            // schemalägg (skapar ScheduledTask och kopplar ScheduledTaskId på outbox)
            await emailSchedulingService.CreateOutboxAndScheduleAsync(dbContext, outbox, runAt: DateTimeOffset.UtcNow, cancellationToken);

            return Result.Ok();
        }

        /// <summary>
        /// Återställer lösenord och slutför automatiskt onboarding när användaren fortfarande är inviterad.
        /// </summary>
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user == null)
                return Result.Fail(new BadRequestError());

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var resetPassResult = await userManager.ResetPasswordAsync(user, resetPasswordRequest.Token, resetPasswordRequest.Password);
            if (!resetPassResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail(resetPassResult.Errors.Select(e => e.Description));
            }

            user.IsPasswordChangeRequired = false;

            var invitedAccount = await dbContext.Set<PersonUserAccount>()
                .SingleOrDefaultAsync(x => x.ApplicationUserId == user.Id && x.Status == PersonUserAccountStatus.Invited, cancellationToken);

            if (invitedAccount is not null)
            {
                user.EmailConfirmed = true;
                user.IsInactivated = false;
                user.InactivatedTime = null;
                user.InactivatedByUserId = null;
                user.InactivatedByOnBehalfOfUserId = null;

                invitedAccount.Status = PersonUserAccountStatus.Active;
                invitedAccount.ActivatedAtUtc = DateTimeOffset.UtcNow;
                invitedAccount.RevokedAtUtc = null;
            }

            var updateUserResult = await userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail(updateUserResult.Errors.Select(e => e.Description));
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await userManager.SetLockoutEndDateAsync(user, new DateTime(2000, 1, 1));
            await transaction.CommitAsync(cancellationToken);
            return Result.Ok();
        }

        public async Task<Result> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(changePasswordRequest.Email);
            if (user == null)
                return Result.Fail(new BadRequestError());

            var changePassResult = await userManager.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);
            if (!changePassResult.Succeeded)
            {
                var errors = changePassResult.Errors.Select(e => e.Description);

                return Result.Fail(errors);
            }

            user.IsPasswordChangeRequired = false;
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result.Fail(updateResult.Errors.Select(e => e.Description));

            await userManager.SetLockoutEndDateAsync(user, new DateTime(2000, 1, 1));

            return Result.Ok();
        }

        public async Task<Result> LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (!currentUserService.IsAuthenticated)
                return Result.Fail(new BadRequestError("[Error.NotLoggedIn] Not logged in"));
            var userId = currentUserService?.UserId ?? Guid.Empty;
            if (userId != Guid.Empty)
            {
                var user = await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
                if (user == null)
                    return Result.Fail(new NotFoundError(nameof(user), userId));
                await userManager.RemoveAuthenticationTokenAsync(user, configuration.GetValue<string>("AppJwtSettings:Issuer") ?? "www.algaebra.eu", "Bearer");
                await signInManager.SignOutAsync();
                return Result.Ok();
            }
            return Result.Fail(new BadRequestError());
        }
        public async Task<Result<AuthJwtResponse>> RefreshTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
        {
            return await RefreshToken(refreshTokenValue, fromCookie: false, cancellationToken);
        }
        public async Task<Result<AuthJwtResponse>> RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            var refreshToken = httpContext?.Request.Cookies["refreshToken"] ?? string.Empty;
            return await RefreshToken(refreshToken!, fromCookie: true, cancellationToken);
        }

        private async Task<Result<AuthJwtResponse>> RefreshToken(string tokenValue, bool fromCookie = true, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(tokenValue))
                return Result.Fail(new BadRequestError("[Error.NoRefreshTokenError] No refresh token found"));

            var user = await GetUserByRefreshToken(tokenValue);
            if (user.IsInactivated)
                return Result.Fail(new TokenError("[Error.InvalidToken] Invalid token"));
            var refreshToken = user.RefreshTokens.Single(x => x.Token == tokenValue);
            var ipAddress = IpAddress() ?? string.Empty;
            if (string.IsNullOrEmpty(ipAddress))
                return Result.Fail(new BadRequestError("[Error.EmptyIpAddress] Could not retrieve ip address"));
            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {tokenValue}");
                await userManager.UpdateAsync(user).ConfigureAwait(false);
            }

            if (!refreshToken.IsActive)
                return Result.Fail(new TokenError("[Error.InvalidToken] Invalid token"));


            logger.LogInformation("Auth token refreshed for {userId} (isRevoked={isRevoked}, isActive={isActive}, isExpired={isExpired}) with ip={ipAddress} and userAgent={userAgent}",
                user.Id, refreshToken.IsRevoked, refreshToken.IsActive, refreshToken.IsExpired, IpAddress(), UserAgent());

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = await RotateRefreshToken(refreshToken, ipAddress).ConfigureAwait(false);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);

            // save changes to db
            await userManager.UpdateAsync(user).ConfigureAwait(false);

            // Test för onBehalfOf
            var oboUserId = httpContext?.Request.Cookies["oboUserId"] ?? httpContext?.Request.Headers.Cookie ?? string.Empty;
            if (!string.IsNullOrEmpty(oboUserId))
            {
                if (Guid.TryParse(oboUserId, out var userId))
                    OnBehalfOfUserId = userId;
            }

            // generate new jwt
            var jwtToken = await GenerateJWT(user, ipAddress, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (jwtToken.IsSuccess)
            {
                if (jwtToken.Value.RefreshToken != null)
                    SetTokenCookie(jwtToken.Value.RefreshToken);
                return Result.Ok(jwtToken.Value);
            }
            return Result.Fail(new LoginError("[Error.LoginRefreshToken] Could not log in using refresh token"));
        }

        public async Task<Result<string>> RevokeTokenAsync(string tokenRequest, CancellationToken cancellationToken = default)
        {
            var token = tokenRequest ?? httpContext?.Request.Cookies["refreshToken"] ?? string.Empty;

            if (string.IsNullOrEmpty(token))
                return Result.Fail(new BadRequestError("[Error.EmptyToken] Token is required"));

            var user = await GetUserByRefreshToken(token);

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            var ipAddress = IpAddress() ?? string.Empty;
            if (string.IsNullOrEmpty(ipAddress))
                return Result.Fail(new BadRequestError("[Error.EmptyIpAddress] Could not retrieve ip address"));
            if (!refreshToken.IsActive)
                return Result.Fail(new TokenError("[Error.InvalidToken] Invalid token"));

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            await userManager.UpdateAsync(user);


            return Result.Ok("Token revoked");
        }



        private async Task<Result<AuthJwtResponse>> GenerateJWT(ApplicationUser user, string ipAddress, string provider = "RefreshToken", CancellationToken cancellationToken = default)
        {
            if (user is null)
                throw new NotFoundException("User not found");
            int jsonWebTokenTTL = 5; // Default to 5 minutes
            try
            {
                jsonWebTokenTTL = configuration.GetValue<int>("AppJwtSettings:ExpirationInMiutes"); // Reads the value from Arb-Int.API.appsettings.json
            }
            catch { }
            var refreshToken = await GenerateRefreshTokenAsync(ipAddress, cancellationToken).ConfigureAwait(false);
            if (refreshToken.IsSuccess)
            {
                user.RefreshTokens.Add(refreshToken.Value);
                RemoveOldRefreshTokens(user);
                await dbContext.SaveChangesAsync(cancellationToken);

                var claims = await userManager.GetClaimsAsync(user);
                if (OnBehalfOfUserId.HasValue)
                    claims.Add(new Claim("obo_sub", OnBehalfOfUserId.ToString()!));

                var identityClaims = await GetUserClaims(claims, user, cancellationToken: cancellationToken);
                var encodedToken = await EncodeToken(identityClaims);
                var expiresInSeconds = TimeSpan.FromMinutes(jsonWebTokenTTL).TotalSeconds;
                AuthJwtResponse jwt = GetTokenResponse(encodedToken, user, claims, expiresInSeconds);
                jwt.RefreshToken = refreshToken.Value.Token;
                jwt.ExpiresIn = expiresInSeconds;
                jwt.Provider = provider;
                return Result.Ok(jwt);
            }
            return Result.Fail(refreshToken.Errors);
        }
        private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, ApplicationUser user, string? authenticationType = null, CancellationToken cancellationToken = default)
        {
            var userRoles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            IList<Claim> roleClaims = [];
            var issuerValue = configuration.GetValue<string>("AppJwtSettings:Issuer");
            var audience = configuration.GetValue<string>("AppJwtSettings:Audience");
            var jwtTTL = Convert.ToUInt32(configuration.GetValue<string>("AppJwtSettings:ExpirationInMiutes") ?? "5");
            var now = DateTime.UtcNow;
            var now2 = DateTimeOffset.Now;
            claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub.ToString(), user.Id.ToString()));
            claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.UniqueName, user.Id.ToString()));
            claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Nonce, Guid.CreateVersion7().ToString()));
            if (!string.IsNullOrEmpty(issuerValue))
                claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Iss, issuerValue));
            if (!string.IsNullOrEmpty(audience))
                claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Aud, audience));
            claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Exp, ToUnixEpochDate(now.AddMinutes(jwtTTL)).ToString()));
            claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64));


            var normalizedUserRoles = userRoles
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.ToUpperInvariant())
                .ToList();

            var roles = await roleManager.Roles
                .AsNoTracking()
                .Where(r => r.NormalizedName != null &&
                            normalizedUserRoles.Contains(r.NormalizedName))
                .Select(r => new
                {
                    RoleName = r.Name!,
                    Claims = r.RoleClaims
                        .Where(c => c.ClaimType != null && c.ClaimValue != null)
                        .Select(c => new
                        {
                            c.ClaimType,
                            c.ClaimValue
                        })
                        .ToList()
                })
                .ToListAsync();

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role.RoleName));

                foreach (var roleClaim in role.Claims)
                {
                    roleClaims.Add(new Claim(roleClaim.ClaimType!, roleClaim.ClaimValue!));
                }
            }
            //foreach (var claim in roleClaims)
            //{
            //    claims.Add(claim);
            //}
            var identityClaims = new ClaimsIdentity(claims, authenticationType: authenticationType ?? "JWT", nameType: Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.UniqueName, roleType: null);
            return identityClaims;
        }
        private async Task<string> EncodeToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var currentIssuer = configuration.GetValue<string>("AppJwtSettings:Issuer");

            var key = await jwtService.GetCurrentSigningCredentials();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = currentIssuer,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToUInt32(configuration.GetValue<string>("AppJwtSettings:ExpirationInMiutes") ?? "5")),
                SigningCredentials = key
            });


            return tokenHandler.WriteToken(token);
        }
        private static AuthJwtResponse GetTokenResponse(string encodedToken, ApplicationUser user, IEnumerable<Claim> claims, double expiresInSeconds)
        {
            return new AuthJwtResponse
            {
                IsAuthSuccessful = true,
                IsPasswordChangeRequired = user.IsPasswordChangeRequired,
                AccessToken = encodedToken,
                ExpiresIn = expiresInSeconds,
                UserData = new UserData
                {
                    Id = user.Id.ToString(),
                    Email = user.Email ?? "N/A",
                    Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
                }
            };
        }
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        private static long ToUnixEpochDate(DateTimeOffset date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private async Task<ApplicationUser> GetUserByRefreshToken(string token)
        {
            var user = await userManager
                .Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            return user ?? throw new InvalidTokenException("[Error.InvalidToken] Invalid token");
        }
        private async Task<RefreshToken> RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = await GenerateRefreshTokenAsync(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Value.Token);
            return newRefreshToken.Value;
        }

        private void RemoveOldRefreshTokens(ApplicationUser user)
        {
            // remove old inactive refresh tokens from user based on TTL in app settings
            int refreshTokenTTL = 7;
            try
            {
                refreshTokenTTL = configuration.GetValue<int>("AppJwtSettings:RefreshTokenExpirationInDays");
            }
            catch { }
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.CreatedAtUtc.AddDays(refreshTokenTTL) <= DateTimeOffset.UtcNow);
        }

        private static void RevokeDescendantRefreshTokens(RefreshToken refreshToken, ApplicationUser user, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken != null)
                    if (childToken.IsActive)
                        RevokeRefreshToken(childToken, ipAddress, reason);
                    else
                        RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
            }
        }

        private static void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = "N/A", string replacedByToken = "N/As")
        {
            token.RevokedAtUtc = DateTimeOffset.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        private void SetTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var refreshTokenExpirationInDays = Convert.ToUInt32(configuration.GetValue<string>("AppJwtSettings:RefreshTokenExpirationInDays") ?? "30");
            httpContext?.Response.Cookies.Append("refreshToken", token, GetCookieOptions(refreshTokenExpirationInDays));
        }
        private CookieOptions GetCookieOptions(uint days, bool httpOnly = true)
        {
            return new CookieOptions
            {
                HttpOnly = httpOnly,
                Expires = DateTimeOffset.UtcNow.AddDays(days),
                SameSite = SameSiteMode.Lax,
                Secure = true,
                IsEssential = true,
                Path = "/"
            };
        }

        private string? IpAddress()
        {
            // get source ip address for the current request
            if (httpContext?.Request.Headers.TryGetValue("X-Forwarded-For", out Microsoft.Extensions.Primitives.StringValues value) ?? false)
                return value;
            else
                return httpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString();
        }
        private string? UserAgent()
        {
            return httpContext?.Request.Headers.UserAgent.ToString();
        }


        /// <summary>
        /// <para>Variables:<br />
        /// {{greeting}} - Hälsning<br />
        /// {{name}} - Mottagarens namn/identifierare eller tomt<br />
        /// {{body_text}} - Instruktion<br />
        /// {{otp}} - Länk till verifikation<br />
        /// {{system_signature}} - Systemsignatur<br />
        /// {{footer_text}} - Text i foten (support)<br />
        /// {{current_year}} - Nuvarande år<br />
        /// {{system_owner}} - Systemägaren (default = Algaebra)<br />
        /// {{all_rights_reserved}} - Copyright (All rights reserved)</para>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Result<AuthJwtResponse>> GenerateOTPFor2StepVerification(
            ApplicationUser user,
            CancellationToken cancellationToken = default)
        {
            var providers = await userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
            {
                return Result.Fail(new ProcessError("[Error.InvalidTwoStepProvider] Invalid 2-Step Verification Provider."));
            }

            var authContactPointResult = await userAuthContactPointResolver.ResolveAsync(user.Id, cancellationToken);
            if (authContactPointResult.IsFailed)
                return Result.Fail<AuthJwtResponse>(authContactPointResult.Errors);

            var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
#if DEBUG
            Console.WriteLine($"Email to {user.Email}: Authentication token: {token}");
#endif
            var name = $"{user.FirstName}".Trim();
            if (string.IsNullOrWhiteSpace(name)) name = $"{user.LastName}".Trim();


            string systemSignature = configuration.GetValue<string>("ArbIntSettings:SystemSignature")!;

            string greeting = configuration.GetValue<string>("ArbIntSettings:Greeting")!;

            string bodyText = configuration.GetValue<string>("ArbIntSettings:TwoStepAuthentication_BodyText")!;

            string footerText = configuration.GetValue<string>("ArbIntSettings:TwoStepAuthentication_FooterText")!;

            string subject = configuration.GetValue<string>("ArbIntSettings:TwoStepAuthentication_Subject")!;
            // Template använder provider placeholders (MailerSend)



            EmailOutboxMessage outbox;
            try
            {
                var req = new EmailOutboxBuildRequest
                {
                    Kind = EmailTemplateKind.TwoStepVerification,
                    Provider = EmailProvider.MailerSend,
                    ContentMode = EmailContentMode.ProviderTemplate,

                    FromUserId = configuration.GetValue<Guid>("ArbIntSettings:SystemUserId")!,
                    ToContactPointId = authContactPointResult.Value.ContactPointId,

                    SubjectTemplate = subject,
                    ProviderTemplateId = "jpzkmgq0d2y4059v",

                    // TwoStepVerification har inga #tokens
                    DomainTokenValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),

                    // Provider vars för {{...}}
                    ProviderVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        [TokenKeys.Name] = name,
                        [TokenKeys.Otp] = token,
                        [TokenKeys.AccountName] = configuration.GetValue<string>("ArbIntSettings:AccountName")!,
                        [TokenKeys.SystemOwner] = configuration.GetValue<string>("ArbIntSettings:SystemOwner")!,
                        [TokenKeys.Greeting] = greeting,
                        [TokenKeys.BodyText] = bodyText,
                        [TokenKeys.SystemSignature] = systemSignature,
                        [TokenKeys.FooterText] = footerText.Replace("{{support_url}}", configuration.GetValue<string>("ArbIntSettings:SupportUrl")!),
                    },

                    PolicyContext = new EmailPolicyContext(ContactId: null, RequiresPassword: false),

                    // Om du vill ha per-recipient overrides (inte nödvändigt här)
                    ProviderPersonalizationByUserId = new Dictionary<Guid, IReadOnlyDictionary<string, string>>()
                };

                outbox = await outboxBuilder.BuildAsync(req, cancellationToken);
            }
            catch
            {
                return Result.Fail(new BadRequestError("Error.TwoStepUnsuccessfulAddress"));
            }

            await emailSchedulingService.CreateOutboxAndScheduleAsync(
                dbContext,
                outbox,
                runAt: DateTimeOffset.UtcNow,
                cancellationToken: cancellationToken);

            return Result.Ok(new AuthJwtResponse
            {
                Is2StepVerificationRequired = true,
                Provider = "Email"
            });
        }

        /// <summary>
        /// <para>Variables:<br />
        /// {{greeting}} - Hälsning<br />
        /// {{name}} - Mottagarens namn/identifierare eller tomt<br />
        /// {{body_text}} - Instruktion<br />
        /// {{verify_link}} - Länk till verifikation<br />
        /// {{verify_text}} - Text på verifikationsknappen<br />
        /// {{system_signature}} - Systemsignatur<br />
        /// {{footer_text}} - Text i foten (support)<br />
        /// {{current_year}} - Nuvarande år<br />
        /// {{system_owner}} - Systemägaren (default = Algaebra)<br />
        /// {{all_rights_reserved}} - Copyright (All rights reserved)</para>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clientUrl"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Result<AuthJwtResponse>> GenerateOTPForEmailVerification(
            ApplicationUser user,
            string clientUrl,
            CancellationToken cancellationToken = default)
        {
            if (user.EmailConfirmed)
                return Result.Ok(new AuthJwtResponse { IsEmailConfirmationNeeded = false });

            var authContactPointResult = await userAuthContactPointResolver.ResolveAsync(user.Id, cancellationToken);
            if (authContactPointResult.IsFailed)
                return Result.Fail<AuthJwtResponse>(authContactPointResult.Errors);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
#if DEBUG
            Console.WriteLine($"Email to {user.Email}: Email verification token: {token}");
#endif
            var param = new Dictionary<string, string?>
                {
                    { "email", user.Email },
                    { "token", token }
                };

            var callback = QueryHelpers.AddQueryString(clientUrl, param);

            var name = $"{user.FirstName}".Trim();
            if (string.IsNullOrWhiteSpace(name)) name = $"{user.LastName}".Trim();

            string systemSignature = configuration.GetValue<string>("ArbIntSettings:SystemSignature")!;

            string greeting = configuration.GetValue<string>("ArbIntSettings:Greeting")!;

            string bodyText = configuration.GetValue<string>("ArbIntSettings:EmailVerification_BodyText")!;

            string footerText = configuration.GetValue<string>("ArbIntSettings:EmailVerification_FooterText")!;

            string verifyText = configuration.GetValue<string>("ArbIntSettings:EmailVerification_VerifyText")!;

            EmailOutboxMessage outbox;
            try
            {
                var req = new EmailOutboxBuildRequest
                {
                    Kind = EmailTemplateKind.EmailVerification,
                    Provider = EmailProvider.MailerSend,
                    ContentMode = EmailContentMode.ProviderTemplate,

                    FromUserId = configuration.GetValue<Guid>("ArbIntSettings:SystemUserId")!,
                    ToContactPointId = authContactPointResult.Value.ContactPointId,

                    SubjectTemplate = verifyText,
                    ProviderTemplateId = "pr9084zxe2jlw63d",

                    DomainTokenValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),

                    ProviderVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        [TokenKeys.Name] = name,
                        [TokenKeys.VerifyLink] = callback,
                        [TokenKeys.AccountName] = configuration.GetValue<string>("ArbIntSettings:AccountName")!,
                        [TokenKeys.SupportUrl] = configuration.GetValue<string>("ArbIntSettings:SupportUrl")!,
                        [TokenKeys.SystemOwner] = configuration.GetValue<string>("ArbIntSettings:SystemOwner")!,
                        [TokenKeys.Greeting] = greeting,
                        [TokenKeys.BodyText] = bodyText,
                        [TokenKeys.VerifyText] = verifyText,
                        [TokenKeys.SystemSignature] = systemSignature,
                        [TokenKeys.FooterText] = footerText,
                    },

                    PolicyContext = new EmailPolicyContext(ContactId: null, RequiresPassword: false),

                    ProviderPersonalizationByUserId = new Dictionary<Guid, IReadOnlyDictionary<string, string>>()
                };

                outbox = await outboxBuilder.BuildAsync(req, cancellationToken);
            }
            catch
            {
                return Result.Fail(new BadRequestError("[Error.EmailVerificationUnsuccessfulAddress] Failed to send email verification message."));
            }

            await emailSchedulingService.CreateOutboxAndScheduleAsync(
                dbContext,
                outbox,
                runAt: DateTimeOffset.UtcNow,
                cancellationToken: cancellationToken);

            return Result.Ok(new AuthJwtResponse
            {
                IsEmailConfirmationNeeded = true
            });
        }


        /// <summary>
        /// Trimmar text och ersätter tomma värden med null.
        /// </summary>
        private static string? TrimToNull(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }


    }
    /// <summary>
    /// Tom implementation av IJwtService
    /// </summary>
    public class NoopJwtService : IJwtService
    {
        public Task<SecurityKey> GenerateKey(JwtKeyType jwtKeyType = JwtKeyType.Jws)
        {
            throw new NotSupportedException();
        }

        public Task<SecurityKey> GenerateNewKey(JwtKeyType jwtKeyType = JwtKeyType.Jws)
        {
            throw new NotSupportedException();
        }

        public Task<EncryptingCredentials> GetCurrentEncryptingCredentials()
        {
            throw new NotSupportedException();
        }

        public Task<SecurityKey> GetCurrentSecurityKey(JwtKeyType jwtKeyType = JwtKeyType.Jws)
        {
            throw new NotSupportedException();
        }

        public Task<SigningCredentials> GetCurrentSigningCredentials()
        {
            throw new NotSupportedException();
        }

        public Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int i, JwtKeyType jwtKeyType)
        {
            throw new NotSupportedException();
        }

        public Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int? i = null)
        {
            throw new NotSupportedException();
        }

        public Task RevokeKey(string keyId, string reason = null!)
        {
            throw new NotSupportedException();
        }
    }

}
