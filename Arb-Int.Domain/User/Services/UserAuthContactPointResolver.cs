using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Löser auth-e-post från PersonUserAccount och PartyContactPoint utan att gissa mellan flera möjliga adresser.
    /// </summary>
    public sealed class UserAuthContactPointResolver : IUserAuthContactPointResolver
    {
        private readonly Arb_IntDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPartyContactPointNormalizer _normalizer;

        /// <summary>
        /// Skapar resolvern med databas-, Identity- och normaliseringsberoenden.
        /// </summary>
        public UserAuthContactPointResolver(Arb_IntDbContext db, UserManager<ApplicationUser> userManager, IPartyContactPointNormalizer normalizer)
        {
            _db = db;
            _userManager = userManager;
            _normalizer = normalizer;
        }

        /// <summary>
        /// Hämtar explicit vald auth-e-postkontaktpunkt eller en entydig legacy-match mot ApplicationUser.Email.
        /// </summary>
        public async Task<Result<UserAuthEmailContactPointResponse>> ResolveAsync(Guid applicationUserId, CancellationToken cancellationToken = default)
        {
            var account = await _db.Set<PersonUserAccount>()
                .AsNoTracking()
                .Where(x => x.ApplicationUserId == applicationUserId)
                .Select(x => new
                {
                    x.PersonId,
                    x.AuthEmailContactPointId,
                    ApplicationUserEmail = x.ApplicationUser.Email,
                    AuthContactPoint = x.AuthEmailContactPoint == null
                        ? null
                        : new
                        {
                            x.AuthEmailContactPoint.Id,
                            x.AuthEmailContactPoint.PartyId,
                            x.AuthEmailContactPoint.Kind,
                            x.AuthEmailContactPoint.Value,
                            x.AuthEmailContactPoint.ValidToUtc
                        }
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (account is null)
                return Result.Fail<UserAuthEmailContactPointResponse>(new NotFoundError(nameof(PersonUserAccount), $"ApplicationUserId:{applicationUserId}"));

            if (account.AuthEmailContactPointId.HasValue)
            {
                if (account.AuthContactPoint is null
                    || account.AuthContactPoint.PartyId != account.PersonId
                    || account.AuthContactPoint.Kind != ContactPointKind.Email
                    || account.AuthContactPoint.ValidToUtc.HasValue)
                {
                    return Result.Fail<UserAuthEmailContactPointResponse>(new ConflictError("The configured auth email contact point is missing, ended or invalid for the linked person."));
                }

                if (string.IsNullOrWhiteSpace(account.ApplicationUserEmail)
                    || _normalizer.Normalize(ContactPointKind.Email, account.ApplicationUserEmail)
                        != _normalizer.Normalize(ContactPointKind.Email, account.AuthContactPoint.Value))
                {
                    return Result.Fail<UserAuthEmailContactPointResponse>(new ConflictError("The configured auth email contact point is out of sync with ApplicationUser.Email."));
                }

                return Result.Ok(new UserAuthEmailContactPointResponse
                {
                    ContactPointId = account.AuthContactPoint.Id,
                    PersonId = account.PersonId,
                    Email = account.AuthContactPoint.Value
                });
            }

            var user = await _userManager.FindByIdAsync(applicationUserId.ToString());
            if (user is null)
                return Result.Fail<UserAuthEmailContactPointResponse>(new NotFoundError(nameof(ApplicationUser), applicationUserId));

            if (string.IsNullOrWhiteSpace(user.Email))
                return Result.Fail<UserAuthEmailContactPointResponse>(new ConflictError("The Identity account has no email address that can be matched to a PartyContactPoint."));

            var normalizedEmail = _normalizer.Normalize(ContactPointKind.Email, user.Email);
            var matches = await _db.Set<PartyContactPoint>()
                .AsNoTracking()
                .Where(x => x.PartyId == account.PersonId
                    && x.Kind == ContactPointKind.Email
                    && x.ValidToUtc == null
                    && x.NormalizedValue == normalizedEmail)
                .OrderBy(x => x.Id)
                .Select(x => new UserAuthEmailContactPointResponse
                {
                    ContactPointId = x.Id,
                    PersonId = x.PartyId,
                    Email = x.Value
                })
                .Take(2)
                .ToListAsync(cancellationToken);

            if (matches.Count == 0)
                return Result.Fail<UserAuthEmailContactPointResponse>(new NotFoundError(nameof(PartyContactPoint), $"Auth email for ApplicationUserId:{applicationUserId}"));

            if (matches.Count > 1)
                return Result.Fail<UserAuthEmailContactPointResponse>(new ConflictError("Multiple active email contact points match the Identity email. Set AuthEmailContactPointId explicitly on PersonUserAccount."));

            return Result.Ok(matches[0]);
        }
    }
}
