using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Validation;
using Arb_Int.Domain.User;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar normaliserade och historiserade kontaktpunkter för parter.
    /// </summary>
    public sealed class PartyContactPointService : IPartyContactPointService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;
        private readonly IPartyContactPointNormalizer _normalizer;
        private readonly IUserIdentitySynchronizationService _userIdentitySynchronizationService;

        /// <summary>
        /// Skapar kontaktpunktservicen med databas-, validerings- och normaliseringsberoenden.
        /// </summary>
        public PartyContactPointService(
            Arb_IntDbContext db,
            IValidationService validationService,
            IPartyContactPointNormalizer normalizer,
            IUserIdentitySynchronizationService userIdentitySynchronizationService)
        {
            _db = db;
            _validationService = validationService;
            _normalizer = normalizer;
            _userIdentitySynchronizationService = userIdentitySynchronizationService;
        }

        /// <summary>
        /// Hämtar aktiva kontaktpunkter eller hela kontakthistoriken för en Party.
        /// </summary>
        public async Task<Result<IReadOnlyList<PartyContactPointResponse>>> GetAsync(
            Guid partyId,
            bool includeHistory = false,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only read contact points for permitted parties/organizations.
            var partyExists = await _db.Set<Party>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == partyId,
                    cancellationToken);

            if (!partyExists)
            {
                return Result.Fail<IReadOnlyList<PartyContactPointResponse>>(
                    new NotFoundError(
                        nameof(Party),
                        partyId));
            }

            IQueryable<PartyContactPoint> query = _db.Set<PartyContactPoint>()
                .AsNoTracking()
                .Where(x => x.PartyId == partyId);

            if (!includeHistory)
            {
                query = query.Where(
                    x => x.ValidToUtc == null);
            }

            var items = await query
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.Kind)
                .ThenBy(x => x.Label)
                .ThenByDescending(x => x.ValidFromUtc)
                .Select(PartyContactPointProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<PartyContactPointResponse>>(items);
        }

        /// <summary>
        /// Validerar och skapar en ny aktiv kontaktpunkt.
        /// </summary>
        public async Task<Result<PartyContactPointResponse>> CreateAsync(
            Guid partyId,
            CreatePartyContactPointRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only create contact points for permitted parties/organizations.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyContactPointResponse>(
                    validationResult.Errors);
            }

            var validFromUtc = request.ValidFromUtc
                ?? DateTimeOffset.UtcNow;

            var referenceResult = await ValidatePartyAndContextAsync(
                partyId,
                request.OrganizationPersonId,
                validFromUtc,
                cancellationToken);

            if (referenceResult.IsFailed)
            {
                return Result.Fail<PartyContactPointResponse>(
                    referenceResult.Errors);
            }

            var normalizedValue = _normalizer.Normalize(
                request.Kind,
                request.Value);

            var duplicateExists = await ActiveDuplicateExistsAsync(
                partyId,
                request.OrganizationPersonId,
                request.Kind,
                normalizedValue,
                excludeId: null,
                cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<PartyContactPointResponse>(
                    new ConflictError(
                        "An active contact point with the same type and value already exists in this context."));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    partyId,
                    request.OrganizationPersonId,
                    request.Kind,
                    excludeId: null,
                    cancellationToken);
            }

            var contactPoint = new PartyContactPoint
            {
                Id = Guid.CreateVersion7(),
                PartyId = partyId,
                OrganizationPersonId = request.OrganizationPersonId,
                Kind = request.Kind,
                Label = TrimToNull(request.Label),
                Value = request.Value.Trim(),
                NormalizedValue = normalizedValue,
                Extension = TrimToNull(request.Extension),
                IsPrimary = request.IsPrimary,
                IsVerified = request.IsVerified,
                PermissionStatus = request.PermissionStatus,
                ValidFromUtc = validFromUtc,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<PartyContactPoint>().Add(contactPoint);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    contactPoint.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar metadata på en aktiv kontaktpunkt utan att ändra dess identitet.
        /// </summary>
        public async Task<Result<PartyContactPointResponse>> UpdateAsync(
            Guid contactPointId,
            UpdatePartyContactPointRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only update contact points for permitted parties/organizations.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyContactPointResponse>(
                    validationResult.Errors);
            }

            var contactPoint = await _db.Set<PartyContactPoint>()
                .SingleOrDefaultAsync(
                    x =>
                        x.Id == contactPointId
                        && x.ValidToUtc == null,
                    cancellationToken);

            if (contactPoint is null)
            {
                return Result.Fail<PartyContactPointResponse>(
                    new NotFoundError(
                        nameof(PartyContactPoint),
                        contactPointId));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    contactPoint.PartyId,
                    contactPoint.OrganizationPersonId,
                    contactPoint.Kind,
                    contactPoint.Id,
                    cancellationToken);
            }

            contactPoint.Label = TrimToNull(request.Label);
            contactPoint.Extension = TrimToNull(request.Extension);
            contactPoint.IsPrimary = request.IsPrimary;
            contactPoint.IsVerified = request.IsVerified;
            contactPoint.PermissionStatus = request.PermissionStatus;
            contactPoint.Notes = TrimToNull(request.Notes);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    contactPoint.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Avslutar en aktiv kontaktpunkt och skapar en ny historisk version.
        /// </summary>
        public async Task<Result<PartyContactPointResponse>> ReplaceAsync(
            Guid contactPointId,
            ReplacePartyContactPointRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only replace contact points for permitted parties/organizations.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyContactPointResponse>(
                    validationResult.Errors);
            }

            var currentContactPoint = await _db.Set<PartyContactPoint>()
                .SingleOrDefaultAsync(
                    x =>
                        x.Id == contactPointId
                        && x.ValidToUtc == null,
                    cancellationToken);

            if (currentContactPoint is null)
            {
                return Result.Fail<PartyContactPointResponse>(
                    new NotFoundError(
                        nameof(PartyContactPoint),
                        contactPointId));
            }

            var validFromUtc = request.ValidFromUtc
                ?? DateTimeOffset.UtcNow;

            if (validFromUtc < currentContactPoint.ValidFromUtc)
            {
                return Result.Fail<PartyContactPointResponse>(
                    new ConflictError(
                        "The replacement contact point cannot start before the current contact point."));
            }

            var referenceResult = await ValidatePartyAndContextAsync(
                currentContactPoint.PartyId,
                request.OrganizationPersonId,
                validFromUtc,
                cancellationToken);

            if (referenceResult.IsFailed)
            {
                return Result.Fail<PartyContactPointResponse>(
                    referenceResult.Errors);
            }

            var normalizedValue = _normalizer.Normalize(
                request.Kind,
                request.Value);

            var duplicateExists = await ActiveDuplicateExistsAsync(
                currentContactPoint.PartyId,
                request.OrganizationPersonId,
                request.Kind,
                normalizedValue,
                currentContactPoint.Id,
                cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<PartyContactPointResponse>(
                    new ConflictError(
                        "An active contact point with the same type and value already exists in this context."));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            currentContactPoint.ValidToUtc = validFromUtc;

            await _db.SaveChangesAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    currentContactPoint.PartyId,
                    request.OrganizationPersonId,
                    request.Kind,
                    currentContactPoint.Id,
                    cancellationToken);
            }

            var replacement = new PartyContactPoint
            {
                Id = Guid.CreateVersion7(),
                PartyId = currentContactPoint.PartyId,
                OrganizationPersonId = request.OrganizationPersonId,
                Kind = request.Kind,
                Label = TrimToNull(request.Label),
                Value = request.Value.Trim(),
                NormalizedValue = normalizedValue,
                Extension = TrimToNull(request.Extension),
                IsPrimary = request.IsPrimary,
                IsVerified = request.IsVerified,
                PermissionStatus = request.PermissionStatus,
                ValidFromUtc = validFromUtc,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<PartyContactPoint>().Add(replacement);

            await _db.SaveChangesAsync(cancellationToken);

            var identitySyncResult = await _userIdentitySynchronizationService.ReplaceAuthEmailContactPointAsync(
                currentContactPoint.Id,
                replacement.Id,
                cancellationToken);

            if (identitySyncResult.IsFailed)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail<PartyContactPointResponse>(identitySyncResult.Errors);
            }

            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    replacement.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Avslutar en aktiv kontaktpunkt utan att ta bort dess historik.
        /// </summary>
        public async Task<Result> EndAsync(
            Guid contactPointId,
            EndPartyContactPointRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only end contact points for permitted parties/organizations.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            var contactPoint = await _db.Set<PartyContactPoint>()
                .SingleOrDefaultAsync(
                    x =>
                        x.Id == contactPointId
                        && x.ValidToUtc == null,
                    cancellationToken);

            if (contactPoint is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(PartyContactPoint),
                        contactPointId));
            }

            if (request.ValidToUtc < contactPoint.ValidFromUtc)
            {
                return Result.Fail(
                    new ConflictError(
                        "ValidToUtc cannot be earlier than ValidFromUtc."));
            }

            var isSelectedAuthEmail = await _db.Set<PersonUserAccount>()
                .AsNoTracking()
                .AnyAsync(x => x.AuthEmailContactPointId == contactPoint.Id, cancellationToken);

            if (isSelectedAuthEmail)
            {
                return Result.Fail(
                    new ConflictError(
                        "The contact point is the selected auth email for an Identity account. Replace it or select another auth email before ending it."));
            }

            contactPoint.ValidToUtc = request.ValidToUtc;

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar att parten finns och att eventuell OrganizationPerson-kontext tillhör samma person.
        /// </summary>
        private async Task<Result> ValidatePartyAndContextAsync(
            Guid partyId,
            Guid? organizationPersonId,
            DateTimeOffset validFromUtc,
            CancellationToken cancellationToken)
        {
            var partyExists = await _db.Set<Party>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == partyId,
                    cancellationToken);

            if (!partyExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Party),
                        partyId));
            }

            if (!organizationPersonId.HasValue)
            {
                return Result.Ok();
            }

            var relationship = await _db.Set<OrganizationPerson>()
                .AsNoTracking()
                .Where(x => x.Id == organizationPersonId.Value)
                .Select(x => new
                {
                    x.PersonId,
                    x.ValidFromUtc,
                    x.ValidToUtc
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (relationship is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(OrganizationPerson),
                        organizationPersonId.Value));
            }

            if (relationship.PersonId != partyId)
            {
                return Result.Fail(
                    new ConflictError(
                        "The selected OrganizationPerson relationship belongs to another person."));
            }

            if (relationship.ValidToUtc.HasValue)
            {
                return Result.Fail(
                    new ConflictError(
                        "An active contextual contact point cannot be created for an ended OrganizationPerson relationship."));
            }

            if (relationship.ValidFromUtc.HasValue
                && validFromUtc < relationship.ValidFromUtc.Value)
            {
                return Result.Fail(
                    new ConflictError(
                        "The contact point cannot start before the OrganizationPerson relationship."));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar om samma aktiva normaliserade kontaktpunkt redan finns i aktuell kontext.
        /// </summary>
        private async Task<bool> ActiveDuplicateExistsAsync(
            Guid partyId,
            Guid? organizationPersonId,
            ContactPointKind kind,
            string normalizedValue,
            Guid? excludeId,
            CancellationToken cancellationToken)
        {
            IQueryable<PartyContactPoint> query = _db.Set<PartyContactPoint>()
                .AsNoTracking()
                .Where(x =>
                    x.Kind == kind
                    && x.NormalizedValue == normalizedValue
                    && x.ValidToUtc == null);

            if (organizationPersonId.HasValue)
            {
                query = query.Where(
                    x => x.OrganizationPersonId == organizationPersonId.Value);
            }
            else
            {
                query = query.Where(
                    x =>
                        x.PartyId == partyId
                        && x.OrganizationPersonId == null);
            }

            if (excludeId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Tar bort primärstatus från andra aktiva kontaktpunkter av samma typ och kontext.
        /// </summary>
        private async Task DemoteCurrentPrimaryAsync(
            Guid partyId,
            Guid? organizationPersonId,
            ContactPointKind kind,
            Guid? excludeId,
            CancellationToken cancellationToken)
        {
            IQueryable<PartyContactPoint> query = _db.Set<PartyContactPoint>()
                .Where(x =>
                    x.Kind == kind
                    && x.IsPrimary
                    && x.ValidToUtc == null);

            if (organizationPersonId.HasValue)
            {
                query = query.Where(
                    x => x.OrganizationPersonId == organizationPersonId.Value);
            }
            else
            {
                query = query.Where(
                    x =>
                        x.PartyId == partyId
                        && x.OrganizationPersonId == null);
            }

            if (excludeId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeId.Value);
            }

            await query.ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.IsPrimary, false),
                cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad kontaktpunkt och kastar om den oväntat saknas.
        /// </summary>
        private async Task<PartyContactPointResponse> ProjectRequiredAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _db.Set<PartyContactPoint>()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(PartyContactPointProjections.Details)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException(
                    $"PartyContactPoint '{id}' was saved but could not be projected.");
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