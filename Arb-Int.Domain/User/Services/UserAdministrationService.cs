using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Hanterar organisationsbaserad användaradministration utan att göra ApplicationUser till ägare av organisationsrelationen.
    /// </summary>
    public sealed class UserAdministrationService : IUserAdministrationService
    {
        private readonly Arb_IntDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPersonUserAccountService _personUserAccountService;
        private readonly IUserInvitationEmailService _invitationEmailService;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar administrationsservicen med Identity-, Party-, e-post- och valideringsberoenden.
        /// </summary>
        public UserAdministrationService(Arb_IntDbContext db, UserManager<ApplicationUser> userManager, IPersonUserAccountService personUserAccountService, IUserInvitationEmailService invitationEmailService, IValidationService validationService)
        {
            _db = db;
            _userManager = userManager;
            _personUserAccountService = personUserAccountService;
            _invitationEmailService = invitationEmailService;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar aktiva eller historiska organisationspersoner och vänsterkopplar eventuell användarkontoinformation.
        /// </summary>
        public async Task<Result<IReadOnlyList<OrganizationUserListResponse>>> GetOrganizationUsersAsync(Guid organizationId, OrganizationUserQueryRequest request, CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only list users for organizations the caller may administer.
            var organizationExists = await _db.Set<Organization>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == organizationId, cancellationToken);

            if (!organizationExists)
                return Result.Fail<IReadOnlyList<OrganizationUserListResponse>>(new NotFoundError(nameof(Organization), organizationId));

            IQueryable<OrganizationPerson> query = _db.Set<OrganizationPerson>()
                .AsNoTracking()
                .Where(x => x.OrganizationId == organizationId);

            if (!request.IncludeHistory)
                query = query.Where(x => x.ValidToUtc == null);

            if (!request.IncludePeopleWithoutAccounts)
                query = query.Where(x => x.Person.UserAccount != null);

            if (request.AccountStatus.HasValue)
                query = query.Where(x => x.Person.UserAccount != null && x.Person.UserAccount.Status == request.AccountStatus.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = $"%{request.Search.Trim()}%";
                query = query.Where(x =>
                    EF.Functions.ILike(x.Person.Name, search)
                    || EF.Functions.ILike(x.Person.GivenName, search)
                    || EF.Functions.ILike(x.Person.FamilyName, search)
                    || (x.Person.UserAccount != null && x.Person.UserAccount.ApplicationUser.Email != null && EF.Functions.ILike(x.Person.UserAccount.ApplicationUser.Email, search))
                    || (x.Person.UserAccount != null && x.Person.UserAccount.ApplicationUser.UserName != null && EF.Functions.ILike(x.Person.UserAccount.ApplicationUser.UserName, search)));
            }

            var items = await query
                .OrderByDescending(x => x.IsPrimaryContact)
                .ThenBy(x => x.Person.Name)
                .ThenByDescending(x => x.ValidFromUtc)
                .Select(x => new OrganizationUserListResponse
                {
                    OrganizationPersonId = x.Id,
                    OrganizationId = x.OrganizationId,
                    OrganizationName = x.Organization.Name,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Name,
                    RelationshipType = x.RelationshipType,
                    JobTitle = x.JobTitle,
                    Department = x.Department,
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc,
                    HasUserAccount = x.Person.UserAccount != null,
                    PersonUserAccountId = x.Person.UserAccount != null ? x.Person.UserAccount.Id : null,
                    ApplicationUserId = x.Person.UserAccount != null ? x.Person.UserAccount.ApplicationUserId : null,
                    AuthEmailContactPointId = x.Person.UserAccount != null ? x.Person.UserAccount.AuthEmailContactPointId : null,
                    AccountStatus = x.Person.UserAccount != null ? x.Person.UserAccount.Status : null,
                    IsExternalUser = x.Person.UserAccount != null ? x.Person.UserAccount.IsExternalUser : null,
                    Email = x.Person.UserAccount != null ? x.Person.UserAccount.ApplicationUser.Email : null,
                    UserName = x.Person.UserAccount != null ? x.Person.UserAccount.ApplicationUser.UserName : null,
                    IsEmailConfirmed = x.Person.UserAccount != null ? x.Person.UserAccount.ApplicationUser.EmailConfirmed : null,
                    IsInactivated = x.Person.UserAccount != null ? x.Person.UserAccount.ApplicationUser.IsInactivated : null,
                    InvitedAtUtc = x.Person.UserAccount != null ? x.Person.UserAccount.InvitedAtUtc : null,
                    ActivatedAtUtc = x.Person.UserAccount != null ? x.Person.UserAccount.ActivatedAtUtc : null,
                    RevokedAtUtc = x.Person.UserAccount != null ? x.Person.UserAccount.RevokedAtUtc : null
                })
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<OrganizationUserListResponse>>(items);
        }

        /// <summary>
        /// Skapar ett Identity-konto från en befintlig aktiv OrganizationPerson och skickar initial lösenordslänk till vald e-postkontaktpunkt.
        /// </summary>
        public async Task<Result<PersonUserAccountResponse>> InviteOrganizationPersonAsync(Guid organizationId, Guid personId, InviteOrganizationUserRequest request, CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only invite users for organizations the caller may administer.
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed)
                return Result.Fail<PersonUserAccountResponse>(validationResult.Errors);

            var membership = await _db.Set<OrganizationPerson>()
                .AsNoTracking()
                .Where(x => x.OrganizationId == organizationId && x.PersonId == personId && x.ValidToUtc == null)
                .Select(x => new
                {
                    x.Id,
                    x.Person.GivenName,
                    x.Person.FamilyName,
                    ExistingPersonUserAccountId = x.Person.UserAccount != null ? (Guid?)x.Person.UserAccount.Id : null
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (membership is null)
                return Result.Fail<PersonUserAccountResponse>(new NotFoundError(nameof(OrganizationPerson), $"OrganizationId:{organizationId};PersonId:{personId}"));

            if (membership.ExistingPersonUserAccountId.HasValue)
                return Result.Fail<PersonUserAccountResponse>(new ConflictError("The person already has a user-account link."));

            var contactPoint = await _db.Set<PartyContactPoint>()
                .AsNoTracking()
                .Where(x => x.Id == request.ContactPointId && x.PartyId == personId && x.ValidToUtc == null)
                .Select(x => new
                {
                    x.Id,
                    x.Kind,
                    x.Value,
                    x.OrganizationPersonId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (contactPoint is null)
                return Result.Fail<PersonUserAccountResponse>(new NotFoundError(nameof(PartyContactPoint), request.ContactPointId));

            if (contactPoint.Kind != ContactPointKind.Email)
                return Result.Fail<PersonUserAccountResponse>(new ConflictError("The selected contact point is not an email address."));

            if (contactPoint.OrganizationPersonId.HasValue && contactPoint.OrganizationPersonId.Value != membership.Id)
                return Result.Fail<PersonUserAccountResponse>(new ConflictError("The selected contextual email belongs to another OrganizationPerson relationship."));

            var email = contactPoint.Value.Trim();
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is not null)
                return Result.Fail<PersonUserAccountResponse>(new ConflictError("An Identity user with the selected email address already exists."));

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            var user = new ApplicationUser
            {
                Id = Guid.CreateVersion7(),
                UserName = email,
                Email = email,
                FirstName = membership.GivenName,
                LastName = membership.FamilyName,
                EmailConfirmed = false,
                IsPasswordChangeRequired = true,
                IsInactivated = false
            };

            var temporaryPassword = ApplicationUserManager.GenerateRandomPassword(16);
            var createUserResult = await _userManager.CreateAsync(user, temporaryPassword);
            if (!createUserResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail<PersonUserAccountResponse>(createUserResult.Errors.Select(x => new Error(x.Description)));
            }

            var accountResult = await _personUserAccountService.InviteAsync(personId, new InvitePersonUserAccountRequest
            {
                ApplicationUserId = user.Id,
                AuthEmailContactPointId = contactPoint.Id,
                IsExternalUser = request.IsExternalUser,
                Notes = request.Notes
            }, cancellationToken);

            if (accountResult.IsFailed)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail<PersonUserAccountResponse>(accountResult.Errors);
            }

            var mailResult = await _invitationEmailService.QueueInvitationAsync(user, contactPoint.Id, request.ClientUrl, cancellationToken);
            if (mailResult.IsFailed)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail<PersonUserAccountResponse>(mailResult.Errors);
            }

            await transaction.CommitAsync(cancellationToken);
            return accountResult;
        }
    }
}
