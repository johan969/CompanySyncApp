using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Håller ApplicationUser synkad med Person och den PartyContactPoint som explicit valts för auth-e-post.
    /// </summary>
    public sealed class UserIdentitySynchronizationService : IUserIdentitySynchronizationService
    {
        private readonly Arb_IntDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Skapar synkroniseringstjänsten med databas- och Identity-beroenden.
        /// </summary>
        public UserIdentitySynchronizationService(Arb_IntDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Uppdaterar Identity-profilen och duplicerade personnamen samt kan välja en ny befintlig auth-e-postkontaktpunkt.
        /// </summary>
        public async Task<Result> UpdateProfileAsync(Guid applicationUserId, UpdateUserDetailRequest request, CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization before allowing administration of another user's profile.
            var user = await _userManager.FindByIdAsync(applicationUserId.ToString());
            if (user is null)
                return Result.Fail(new NotFoundError(nameof(ApplicationUser), applicationUserId));

            var account = await _db.Set<PersonUserAccount>()
                .Include(x => x.Person)
                .SingleOrDefaultAsync(x => x.ApplicationUserId == applicationUserId, cancellationToken);

            var requestedFirstName = TrimToNull(request.FirstName);
            var requestedLastName = TrimToNull(request.LastName);

            var synchronizedFirstName = account is not null
                ? requestedFirstName ?? account.Person.GivenName
                : requestedFirstName;
            var synchronizedLastName = account is not null
                ? requestedLastName ?? account.Person.FamilyName
                : requestedLastName;

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            user.FirstName = synchronizedFirstName;
            user.LastName = synchronizedLastName;

            if (account is not null)
            {
                account.Person.GivenName = synchronizedFirstName!;
                account.Person.FamilyName = synchronizedLastName!;
            }

            var requestedPhoneNumber = TrimToNull(request.PhoneNumber);
            if (!string.Equals(user.PhoneNumber, requestedPhoneNumber, StringComparison.Ordinal))
            {
                var phoneResult = await _userManager.SetPhoneNumberAsync(user, requestedPhoneNumber);
                if (!phoneResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Fail(phoneResult.Errors.Select(x => new Error(x.Description)));
                }
            }

            if (request.AuthEmailContactPointId.HasValue)
            {
                if (account is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Fail(new ConflictError("AuthEmailContactPointId requires a linked PersonUserAccount."));
                }

                var emailResult = await ValidateEmailContactPointAsync(account.PersonId, request.AuthEmailContactPointId.Value, cancellationToken);
                if (emailResult.IsFailed)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Fail(emailResult.Errors);
                }

                account.AuthEmailContactPointId = emailResult.Value.Id;

                var identityEmailResult = await SetIdentityEmailAsync(user, emailResult.Value.Value, cancellationToken);
                if (identityEmailResult.IsFailed)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return identityEmailResult;
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail(updateResult.Errors.Select(x => new Error(x.Description)));
            }

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Ok();
        }

        /// <summary>
        /// Synkroniserar Identity-namn och vald auth-e-post från Person efter en domänuppdatering.
        /// </summary>
        public async Task<Result> SynchronizeIdentityFromPersonAsync(Guid personId, CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization in the caller before allowing person administration outside the current user's permitted organizations.
            var account = await _db.Set<PersonUserAccount>()
                .Include(x => x.Person)
                .Include(x => x.ApplicationUser)
                .Include(x => x.AuthEmailContactPoint)
                .SingleOrDefaultAsync(x => x.PersonId == personId, cancellationToken);

            if (account is null)
                return Result.Ok();

            var user = account.ApplicationUser;
            user.FirstName = account.Person.GivenName;
            user.LastName = account.Person.FamilyName;

            if (account.AuthEmailContactPoint is not null)
            {
                if (account.AuthEmailContactPoint.PartyId != personId
                    || account.AuthEmailContactPoint.Kind != ContactPointKind.Email
                    || account.AuthEmailContactPoint.ValidToUtc.HasValue)
                {
                    return Result.Fail(new ConflictError("The configured auth email contact point is not an active email for the linked Person."));
                }

                var emailResult = await SetIdentityEmailAsync(user, account.AuthEmailContactPoint.Value, cancellationToken);
                if (emailResult.IsFailed)
                    return emailResult;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            return updateResult.Succeeded
                ? Result.Ok()
                : Result.Fail(updateResult.Errors.Select(x => new Error(x.Description)));
        }

        /// <summary>
        /// Flyttar auth-e-postkopplingen till den historiska ersättaren och uppdaterar Identity-e-posten.
        /// </summary>
        public async Task<Result> ReplaceAuthEmailContactPointAsync(Guid oldContactPointId, Guid replacementContactPointId, CancellationToken cancellationToken = default)
        {
            var account = await _db.Set<PersonUserAccount>()
                .Include(x => x.ApplicationUser)
                .SingleOrDefaultAsync(x => x.AuthEmailContactPointId == oldContactPointId, cancellationToken);

            if (account is null)
                return Result.Ok();

            var replacementResult = await ValidateEmailContactPointAsync(account.PersonId, replacementContactPointId, cancellationToken);
            if (replacementResult.IsFailed)
                return Result.Fail(replacementResult.Errors);

            account.AuthEmailContactPointId = replacementResult.Value.Id;

            var emailResult = await SetIdentityEmailAsync(account.ApplicationUser, replacementResult.Value.Value, cancellationToken);
            if (emailResult.IsFailed)
                return emailResult;

            await _db.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }

        /// <summary>
        /// Validerar att kontaktpunkten är en aktiv e-postadress för rätt domänperson.
        /// </summary>
        private async Task<Result<PartyContactPoint>> ValidateEmailContactPointAsync(Guid personId, Guid contactPointId, CancellationToken cancellationToken)
        {
            var contactPoint = await _db.Set<PartyContactPoint>()
                .SingleOrDefaultAsync(x => x.Id == contactPointId, cancellationToken);

            if (contactPoint is null)
                return Result.Fail<PartyContactPoint>(new NotFoundError(nameof(PartyContactPoint), contactPointId));

            if (contactPoint.PartyId != personId)
                return Result.Fail<PartyContactPoint>(new ConflictError("The selected auth email contact point belongs to another Person."));

            if (contactPoint.Kind != ContactPointKind.Email)
                return Result.Fail<PartyContactPoint>(new ConflictError("The selected auth contact point is not an email address."));

            if (contactPoint.ValidToUtc.HasValue)
                return Result.Fail<PartyContactPoint>(new ConflictError("The selected auth email contact point has ended."));

            return Result.Ok(contactPoint);
        }

        /// <summary>
        /// Uppdaterar Identity-e-post och username när auth-adressen ändras och kräver ny e-postbekräftelse.
        /// </summary>
        private async Task<Result> SetIdentityEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            var normalizedEmail = email.Trim();
            if (string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase)
                && string.Equals(user.UserName, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Ok();
            }

            var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (existingUser is not null && existingUser.Id != user.Id)
                return Result.Fail(new ConflictError("Another Identity account already uses the selected email address."));

            var emailResult = await _userManager.SetEmailAsync(user, normalizedEmail);
            if (!emailResult.Succeeded)
                return Result.Fail(emailResult.Errors.Select(x => new Error(x.Description)));

            var usernameResult = await _userManager.SetUserNameAsync(user, normalizedEmail);
            if (!usernameResult.Succeeded)
                return Result.Fail(usernameResult.Errors.Select(x => new Error(x.Description)));

            user.EmailConfirmed = false;
            return Result.Ok();
        }

        /// <summary>
        /// Trimmar text och ersätter tomma värden med null.
        /// </summary>
        private static string? TrimToNull(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
