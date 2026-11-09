using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Location;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar normaliserade identifierare för organisationer.
    /// </summary>
    public sealed class OrganizationIdentifierService : IOrganizationIdentifierService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;
        private readonly IOrganizationIdentifierNormalizer _normalizer;

        /// <summary>
        /// Skapar identifierarservicen med databas-, validerings- och normaliseringsberoenden.
        /// </summary>
        public OrganizationIdentifierService(
            Arb_IntDbContext db,
            IValidationService validationService,
            IOrganizationIdentifierNormalizer normalizer)
        {
            _db = db;
            _validationService = validationService;
            _normalizer = normalizer;
        }

        /// <summary>
        /// Hämtar samtliga identifierare för en organisation.
        /// </summary>
        public async Task<Result<IReadOnlyList<OrganizationIdentifierResponse>>> GetAsync(
            Guid organizationId,
            CancellationToken cancellationToken = default)
        {
            var organizationExists = await _db.Set<Organization>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == organizationId,
                    cancellationToken);

            if (!organizationExists)
            {
                return Result.Fail<IReadOnlyList<OrganizationIdentifierResponse>>(
                    new NotFoundError(
                        nameof(Organization),
                        organizationId));
            }

            var identifiers = await _db.Set<OrganizationIdentifier>()
                .AsNoTracking()
                .Where(x => x.OrganizationId == organizationId)
                .OrderBy(x => x.Scheme)
                .ThenByDescending(x => x.IsPrimary)
                .ThenBy(x => x.Value)
                .Select(OrganizationIdentifierProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<OrganizationIdentifierResponse>>(
                identifiers);
        }

        /// <summary>
        /// Hämtar en enskild organisationsidentifierare.
        /// </summary>
        public async Task<Result<OrganizationIdentifierResponse>> GetByIdAsync(
            Guid identifierId,
            CancellationToken cancellationToken = default)
        {
            var identifier = await _db.Set<OrganizationIdentifier>()
                .AsNoTracking()
                .Where(x => x.Id == identifierId)
                .Select(OrganizationIdentifierProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);

            if (identifier is null)
            {
                return Result.Fail<OrganizationIdentifierResponse>(
                    new NotFoundError(
                        nameof(OrganizationIdentifier),
                        identifierId));
            }

            return Result.Ok(identifier);
        }

        /// <summary>
        /// Validerar och skapar en identifierare för en organisation.
        /// </summary>
        public async Task<Result<OrganizationIdentifierResponse>> CreateAsync(
            Guid organizationId,
            CreateOrganizationIdentifierRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<OrganizationIdentifierResponse>(
                    validationResult.Errors);
            }

            var referenceResult = await ValidateReferencesAsync(
                organizationId,
                request.IssuingCountryId,
                cancellationToken);

            if (referenceResult.IsFailed)
            {
                return Result.Fail<OrganizationIdentifierResponse>(
                    referenceResult.Errors);
            }

            var normalizedScheme = _normalizer.NormalizeScheme(
                request.Scheme);

            var normalizedValue = _normalizer.NormalizeValue(
                normalizedScheme,
                request.Value);

            var identifierKey = _normalizer.CreateIdentifierKey(
                normalizedScheme,
                request.IssuingCountryId,
                normalizedValue);

            var duplicateExists = await _db.Set<OrganizationIdentifier>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.IdentifierKey == identifierKey,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<OrganizationIdentifierResponse>(
                    new ConflictError(
                        "An organization identifier with the same scheme, issuing country and value already exists."));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    organizationId,
                    normalizedScheme,
                    excludeIdentifierId: null,
                    cancellationToken);
            }

            var identifier = new OrganizationIdentifier
            {
                Id = Guid.CreateVersion7(),
                OrganizationId = organizationId,
                Scheme = normalizedScheme,
                Value = request.Value.Trim(),
                NormalizedValue = normalizedValue,
                Issuer = TrimToNull(request.Issuer),
                IssuingCountryId = request.IssuingCountryId,
                IsPrimary = request.IsPrimary,
                IdentifierKey = identifierKey
            };

            _db.Set<OrganizationIdentifier>()
                .Add(identifier);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    identifier.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Validerar och uppdaterar en befintlig organisationsidentifierare.
        /// </summary>
        public async Task<Result<OrganizationIdentifierResponse>> UpdateAsync(
            Guid identifierId,
            UpdateOrganizationIdentifierRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<OrganizationIdentifierResponse>(
                    validationResult.Errors);
            }

            var identifier = await _db.Set<OrganizationIdentifier>()
                .SingleOrDefaultAsync(
                    x => x.Id == identifierId,
                    cancellationToken);

            if (identifier is null)
            {
                return Result.Fail<OrganizationIdentifierResponse>(
                    new NotFoundError(
                        nameof(OrganizationIdentifier),
                        identifierId));
            }

            var referenceResult = await ValidateReferencesAsync(
                identifier.OrganizationId,
                request.IssuingCountryId,
                cancellationToken);

            if (referenceResult.IsFailed)
            {
                return Result.Fail<OrganizationIdentifierResponse>(
                    referenceResult.Errors);
            }

            var normalizedScheme = _normalizer.NormalizeScheme(
                request.Scheme);

            var normalizedValue = _normalizer.NormalizeValue(
                normalizedScheme,
                request.Value);

            var identifierKey = _normalizer.CreateIdentifierKey(
                normalizedScheme,
                request.IssuingCountryId,
                normalizedValue);

            var duplicateExists = await _db.Set<OrganizationIdentifier>()
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Id != identifier.Id
                        && x.IdentifierKey == identifierKey,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<OrganizationIdentifierResponse>(
                    new ConflictError(
                        "An organization identifier with the same scheme, issuing country and value already exists."));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    identifier.OrganizationId,
                    normalizedScheme,
                    identifier.Id,
                    cancellationToken);
            }

            identifier.Scheme = normalizedScheme;
            identifier.Value = request.Value.Trim();
            identifier.NormalizedValue = normalizedValue;
            identifier.Issuer = TrimToNull(request.Issuer);
            identifier.IssuingCountryId = request.IssuingCountryId;
            identifier.IsPrimary = request.IsPrimary;
            identifier.IdentifierKey = identifierKey;

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    identifier.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort en organisationsidentifierare.
        /// </summary>
        public async Task<Result> DeleteAsync(
            Guid identifierId,
            CancellationToken cancellationToken = default)
        {
            var identifier = await _db.Set<OrganizationIdentifier>()
                .SingleOrDefaultAsync(
                    x => x.Id == identifierId,
                    cancellationToken);

            if (identifier is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(OrganizationIdentifier),
                        identifierId));
            }

            _db.Set<OrganizationIdentifier>()
                .Remove(identifier);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar att organisationen och eventuellt utfärdande land finns.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(
            Guid organizationId,
            Guid? issuingCountryId,
            CancellationToken cancellationToken)
        {
            var organizationExists = await _db.Set<Organization>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == organizationId,
                    cancellationToken);

            if (!organizationExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Organization),
                        organizationId));
            }

            if (!issuingCountryId.HasValue)
            {
                return Result.Ok();
            }

            var countryExists = await _db.Set<Country>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == issuingCountryId.Value,
                    cancellationToken);

            if (!countryExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Country),
                        issuingCountryId.Value));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Tar bort primärstatus från andra identifierare med samma organisation och scheme.
        /// </summary>
        private async Task DemoteCurrentPrimaryAsync(
            Guid organizationId,
            string scheme,
            Guid? excludeIdentifierId,
            CancellationToken cancellationToken)
        {
            IQueryable<OrganizationIdentifier> query = _db
                .Set<OrganizationIdentifier>()
                .Where(x =>
                    x.OrganizationId == organizationId
                    && x.Scheme == scheme
                    && x.IsPrimary);

            if (excludeIdentifierId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeIdentifierId.Value);
            }

            await query.ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.IsPrimary,
                        false),
                cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad identifierare och kastar om den oväntat saknas.
        /// </summary>
        private async Task<OrganizationIdentifierResponse> ProjectRequiredAsync(
            Guid identifierId,
            CancellationToken cancellationToken)
        {
            return await _db.Set<OrganizationIdentifier>()
                .AsNoTracking()
                .Where(x => x.Id == identifierId)
                .Select(OrganizationIdentifierProjections.Details)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException(
                    $"OrganizationIdentifier '{identifierId}' was saved but could not be projected.");
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