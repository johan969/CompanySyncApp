using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties.Projections;
using Arb_Int.Domain.User;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar användarkontokopplingar och deras livscykel.
    /// </summary>
    public sealed class PersonUserAccountService : IPersonUserAccountService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPartyContactPointNormalizer _contactPointNormalizer;

        /// <summary>
        /// Skapar användarkontoservicen med databas-, validerings- och användarkontextberoenden.
        /// </summary>
        public PersonUserAccountService(
            Arb_IntDbContext db,
            IValidationService validationService,
            ICurrentUserService currentUserService,
            IPartyContactPointNormalizer contactPointNormalizer)
        {
            _db = db;
            _validationService = validationService;
            _currentUserService = currentUserService;
            _contactPointNormalizer = contactPointNormalizer;
        }

        /// <summary>
        /// Hämtar en användarkontokoppling via dess identifierare.
        /// </summary>
        public async Task<Result<PersonUserAccountResponse>> GetByIdAsync(
            Guid personUserAccountId,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only read user-account links in permitted organizations/person scopes.
            var response = await ProjectAsync(
                personUserAccountId,
                cancellationToken);

            if (response is null)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new NotFoundError(
                        nameof(PersonUserAccount),
                        personUserAccountId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Hämtar användarkontokopplingen för en person.
        /// </summary>
        public async Task<Result<PersonUserAccountResponse>> GetByPersonAsync(
            Guid personId,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only read persons in permitted organizations.
            var response = await _db.Set<PersonUserAccount>()
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(PersonUserAccountProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new NotFoundError(
                        nameof(PersonUserAccount),
                        $"PersonId:{personId}"));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Hämtar användarkontokopplingen för en Identity-användare.
        /// </summary>
        public async Task<Result<PersonUserAccountResponse>> GetByApplicationUserAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only read users in permitted organizations.
            var response = await _db.Set<PersonUserAccount>()
                .AsNoTracking()
                .Where(x => x.ApplicationUserId == applicationUserId)
                .Select(PersonUserAccountProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new NotFoundError(
                        nameof(PersonUserAccount),
                        $"ApplicationUserId:{applicationUserId}"));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Kopplar ett befintligt Identity-konto till en person och markerar kopplingen som inviterad.
        /// </summary>
        public async Task<Result<PersonUserAccountResponse>> InviteAsync(
            Guid personId,
            InvitePersonUserAccountRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only invite persons in organizations the caller may administer.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    validationResult.Errors);
            }

            var personExists = await _db.Set<Person>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == personId,
                    cancellationToken);

            if (!personExists)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new NotFoundError(
                        nameof(Person),
                        personId));
            }

            var applicationUser = await _db.Set<ApplicationUser>()
                .AsNoTracking()
                .Where(x => x.Id == request.ApplicationUserId)
                .Select(x => new
                {
                    x.Id,
                    x.Email
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (applicationUser is null)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new NotFoundError(
                        nameof(ApplicationUser),
                        request.ApplicationUserId));
            }

            var authEmailContactPoint = await _db.Set<PartyContactPoint>()
                .AsNoTracking()
                .Where(x => x.Id == request.AuthEmailContactPointId)
                .Select(x => new
                {
                    x.Id,
                    x.PartyId,
                    x.Kind,
                    x.Value,
                    x.NormalizedValue,
                    x.ValidToUtc
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (authEmailContactPoint is null)
                return Result.Fail<PersonUserAccountResponse>(new NotFoundError(nameof(PartyContactPoint), request.AuthEmailContactPointId));

            if (authEmailContactPoint.PartyId != personId)
                return Result.Fail<PersonUserAccountResponse>(new ConflictError("The selected auth email contact point belongs to another Person."));

            if (authEmailContactPoint.Kind != ContactPointKind.Email || authEmailContactPoint.ValidToUtc.HasValue)
                return Result.Fail<PersonUserAccountResponse>(new ConflictError("The selected auth email contact point must be an active email address."));

            if (string.IsNullOrWhiteSpace(applicationUser.Email)
                || _contactPointNormalizer.Normalize(ContactPointKind.Email, applicationUser.Email) != authEmailContactPoint.NormalizedValue)
            {
                return Result.Fail<PersonUserAccountResponse>(new ConflictError("The selected auth email contact point does not match the Identity account email."));
            }

            var personAlreadyLinked = await _db.Set<PersonUserAccount>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.PersonId == personId,
                    cancellationToken);

            if (personAlreadyLinked)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new ConflictError(
                        "The person is already linked to an application user."));
            }

            var userAlreadyLinked = await _db.Set<PersonUserAccount>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.ApplicationUserId == request.ApplicationUserId,
                    cancellationToken);

            if (userAlreadyLinked)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new ConflictError(
                        "The application user is already linked to another person."));
            }

            var account = new PersonUserAccount
            {
                Id = Guid.CreateVersion7(),
                PersonId = personId,
                ApplicationUserId = request.ApplicationUserId,
                AuthEmailContactPointId = request.AuthEmailContactPointId,
                Status = PersonUserAccountStatus.Invited,
                IsExternalUser = request.IsExternalUser,
                InvitedAtUtc = DateTimeOffset.UtcNow,
                ActivatedAtUtc = null,
                RevokedAtUtc = null,
                InvitedByUserId = _currentUserService.UserId,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<PersonUserAccount>()
                .Add(account);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    account.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar metadata för en användarkontokoppling med optimistic concurrency.
        /// </summary>
        public async Task<Result<PersonUserAccountResponse>> UpdateAsync(
            Guid personUserAccountId,
            UpdatePersonUserAccountRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only update user-account links in permitted organizations.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    validationResult.Errors);
            }

            var account = await _db.Set<PersonUserAccount>()
                .SingleOrDefaultAsync(
                    x => x.Id == personUserAccountId,
                    cancellationToken);

            if (account is null)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new NotFoundError(
                        nameof(PersonUserAccount),
                        personUserAccountId));
            }

            var concurrencyResult = ValidateRowVersion(
                account,
                request.RowVersion);

            if (concurrencyResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    concurrencyResult.Errors);
            }

            account.IsExternalUser = request.IsExternalUser;
            account.Notes = TrimToNull(request.Notes);

            var saveResult = await SaveWithConcurrencyAsync(
                account,
                request.RowVersion,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    account.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Aktiverar en inviterad användarkontokoppling med optimistic concurrency.
        /// </summary>
        public async Task<Result<PersonUserAccountResponse>> ActivateAsync(
            Guid personUserAccountId,
            ActivatePersonUserAccountRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only activate users in permitted organizations.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    validationResult.Errors);
            }

            var account = await _db.Set<PersonUserAccount>()
                .Include(x => x.ApplicationUser)
                .SingleOrDefaultAsync(
                    x => x.Id == personUserAccountId,
                    cancellationToken);

            if (account is null)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new NotFoundError(
                        nameof(PersonUserAccount),
                        personUserAccountId));
            }

            var concurrencyResult = ValidateRowVersion(
                account,
                request.RowVersion);

            if (concurrencyResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    concurrencyResult.Errors);
            }

            if (account.Status != PersonUserAccountStatus.Invited)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new ConflictError(
                        $"A person user account with status '{account.Status}' cannot be activated."));
            }

            account.Status = PersonUserAccountStatus.Active;
            account.ActivatedAtUtc = DateTimeOffset.UtcNow;
            account.RevokedAtUtc = null;

            account.ApplicationUser.IsInactivated = false;
            account.ApplicationUser.InactivatedTime = null;
            account.ApplicationUser.InactivatedByUserId = null;
            account.ApplicationUser.InactivatedByOnBehalfOfUserId = null;

            var saveResult = await SaveWithConcurrencyAsync(
                account,
                request.RowVersion,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    account.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Återkallar en inviterad eller aktiv användarkontokoppling med optimistic concurrency.
        /// </summary>
        public async Task<Result<PersonUserAccountResponse>> RevokeAsync(
            Guid personUserAccountId,
            RevokePersonUserAccountRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only revoke users in permitted organizations.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    validationResult.Errors);
            }

            var account = await _db.Set<PersonUserAccount>()
                .Include(x => x.ApplicationUser)
                .SingleOrDefaultAsync(
                    x => x.Id == personUserAccountId,
                    cancellationToken);

            if (account is null)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new NotFoundError(
                        nameof(PersonUserAccount),
                        personUserAccountId));
            }

            var concurrencyResult = ValidateRowVersion(
                account,
                request.RowVersion);

            if (concurrencyResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    concurrencyResult.Errors);
            }

            if (account.Status == PersonUserAccountStatus.Revoked)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    new ConflictError(
                        "The person user account has already been revoked."));
            }

            account.Status = PersonUserAccountStatus.Revoked;
            account.RevokedAtUtc = DateTimeOffset.UtcNow;

            account.ApplicationUser.IsInactivated = true;
            account.ApplicationUser.InactivatedTime = DateTimeOffset.UtcNow;

            var saveResult = await SaveWithConcurrencyAsync(
                account,
                request.RowVersion,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<PersonUserAccountResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    account.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Kontrollerar att klientens RowVersion motsvarar senast lästa version.
        /// </summary>
        private static Result ValidateRowVersion(
            PersonUserAccount account,
            uint expectedRowVersion)
        {
            if (account.RowVersion == expectedRowVersion)
            {
                return Result.Ok();
            }

            return Result.Fail(
                new ConcurrencyError(
                    nameof(PersonUserAccount),
                    account.Id));
        }

        /// <summary>
        /// Sparar ändringar med RowVersion som optimistic concurrency-token.
        /// </summary>
        private async Task<Result> SaveWithConcurrencyAsync(
            PersonUserAccount account,
            uint expectedRowVersion,
            CancellationToken cancellationToken)
        {
            _db.Entry(account)
                .Property(x => x.RowVersion)
                .OriginalValue = expectedRowVersion;

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                foreach (var entry in exception.Entries)
                {
                    await entry.ReloadAsync(cancellationToken);
                }

                return Result.Fail(
                    new ConcurrencyError(
                        nameof(PersonUserAccount),
                        account.Id));
            }
        }

        /// <summary>
        /// Projekterar en användarkontokoppling direkt från databasen.
        /// </summary>
        private async Task<PersonUserAccountResponse?> ProjectAsync(
            Guid personUserAccountId,
            CancellationToken cancellationToken)
        {
            return await _db.Set<PersonUserAccount>()
                .AsNoTracking()
                .Where(x => x.Id == personUserAccountId)
                .Select(PersonUserAccountProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad användarkontokoppling och kastar om den oväntat saknas.
        /// </summary>
        private async Task<PersonUserAccountResponse> ProjectRequiredAsync(
            Guid personUserAccountId,
            CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    personUserAccountId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"PersonUserAccount '{personUserAccountId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Trimmar text och ersätter tomma värden med null.
        /// </summary>
        private static string? TrimToNull(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}