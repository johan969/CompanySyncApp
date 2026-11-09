using Arb_Int.Domain.Location;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar certification scheme jurisdictions med child- och aggregate-concurrency.
    /// </summary>
    public sealed class CertificationSchemeJurisdictionService : ICertificationSchemeJurisdictionService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public CertificationSchemeJurisdictionService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Lägger till en jurisdiction med immutable code inom scheme-versionen.
        /// </summary>
        public async Task<Result<CertificationSchemeResponse>> AddAsync(int certificationSchemeId, CreateCertificationSchemeJurisdictionRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    validationResult.Errors);
            }

            var scheme = await _db.Set<CertificationScheme>()
                .SingleOrDefaultAsync(
                    x => x.Id == certificationSchemeId,
                    cancellationToken);

            if (scheme is null)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new NotFoundError(
                        nameof(CertificationScheme),
                        certificationSchemeId));
            }

            var countryResult = await ValidateCountryAsync(
                request.CountryId,
                cancellationToken);

            if (countryResult.IsFailed)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    countryResult.Errors);
            }

            var code = request.Code.Trim();

            var duplicateExists = await _db.Set<CertificationSchemeJurisdiction>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.CertificationSchemeId == scheme.Id
                        && x.Code == code,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new ConflictError(
                        $"CertificationScheme jurisdiction code '{code}' already exists within this scheme version."));
            }

            _db.Entry(scheme)
                .Property(x => x.RowVersion)
                .OriginalValue = request.CertificationSchemeRowVersion;

            var jurisdiction = new CertificationSchemeJurisdiction
            {
                CertificationSchemeId = scheme.Id,
                Kind = request.Kind,
                Code = code,
                Name = request.Name.Trim(),
                CountryId = request.CountryId,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<CertificationSchemeJurisdiction>()
                .Add(jurisdiction);

            TouchScheme(scheme);

            var saveResult = await SaveChangesAsync(
                scheme.Id,
                jurisdiction.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    scheme.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar kind, name, country och notes medan scheme och code förblir immutable.
        /// </summary>
        public async Task<Result<CertificationSchemeResponse>> UpdateAsync(int certificationSchemeJurisdictionId, UpdateCertificationSchemeJurisdictionRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    validationResult.Errors);
            }

            var jurisdiction = await _db.Set<CertificationSchemeJurisdiction>()
                .SingleOrDefaultAsync(
                    x => x.Id == certificationSchemeJurisdictionId,
                    cancellationToken);

            if (jurisdiction is null)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new NotFoundError(
                        nameof(CertificationSchemeJurisdiction),
                        certificationSchemeJurisdictionId));
            }

            var scheme = await _db.Set<CertificationScheme>()
                .SingleOrDefaultAsync(
                    x => x.Id == jurisdiction.CertificationSchemeId,
                    cancellationToken);

            if (scheme is null)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new NotFoundError(
                        nameof(CertificationScheme),
                        jurisdiction.CertificationSchemeId));
            }

            var countryResult = await ValidateCountryAsync(
                request.CountryId,
                cancellationToken);

            if (countryResult.IsFailed)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    countryResult.Errors);
            }

            var name = request.Name.Trim();
            var notes = TrimToNull(request.Notes);

            if (jurisdiction.Kind == request.Kind
                && jurisdiction.Name == name
                && jurisdiction.CountryId == request.CountryId
                && jurisdiction.Notes == notes)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        scheme.Id,
                        cancellationToken));
            }

            _db.Entry(jurisdiction)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            _db.Entry(scheme)
                .Property(x => x.RowVersion)
                .OriginalValue = request.CertificationSchemeRowVersion;

            jurisdiction.Kind = request.Kind;
            jurisdiction.Name = name;
            jurisdiction.CountryId = request.CountryId;
            jurisdiction.Notes = notes;

            TouchScheme(scheme);

            var saveResult = await SaveChangesAsync(
                scheme.Id,
                jurisdiction.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    scheme.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort jurisdictionen och concurrency-skyddar både child-raden och scheme-rooten.
        /// </summary>
        public async Task<Result<CertificationSchemeResponse>> DeleteAsync(int certificationSchemeJurisdictionId, uint rowVersion, uint certificationSchemeRowVersion, CancellationToken cancellationToken = default)
        {
            var jurisdiction = await _db.Set<CertificationSchemeJurisdiction>()
                .SingleOrDefaultAsync(
                    x => x.Id == certificationSchemeJurisdictionId,
                    cancellationToken);

            if (jurisdiction is null)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new NotFoundError(
                        nameof(CertificationSchemeJurisdiction),
                        certificationSchemeJurisdictionId));
            }

            var scheme = await _db.Set<CertificationScheme>()
                .SingleOrDefaultAsync(
                    x => x.Id == jurisdiction.CertificationSchemeId,
                    cancellationToken);

            if (scheme is null)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new NotFoundError(
                        nameof(CertificationScheme),
                        jurisdiction.CertificationSchemeId));
            }

            var inUse = await _db.Set<SupplyCertification>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.CertificationSchemeJurisdictionId == jurisdiction.Id,
                    cancellationToken);

            if (inUse)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new ConflictError(
                        "The CertificationSchemeJurisdiction cannot be deleted while it is referenced by SupplyCertification."));
            }

            _db.Entry(jurisdiction)
                .Property(x => x.RowVersion)
                .OriginalValue = rowVersion;

            _db.Entry(scheme)
                .Property(x => x.RowVersion)
                .OriginalValue = certificationSchemeRowVersion;

            _db.Set<CertificationSchemeJurisdiction>()
                .Remove(jurisdiction);

            TouchScheme(scheme);

            var saveResult = await SaveChangesAsync(
                scheme.Id,
                jurisdiction.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    scheme.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Verifierar country-referensen när jurisdictionen använder en sådan.
        /// </summary>
        private async Task<Result> ValidateCountryAsync(Guid? countryId, CancellationToken cancellationToken)
        {
            if (!countryId.HasValue)
            {
                return Result.Ok();
            }

            var countryExists = await _db.Set<Country>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == countryId.Value,
                    cancellationToken);

            if (!countryExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Country),
                        countryId.Value));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Markerar scheme-rooten så att jurisdiction-writes konkurrerar på samma RowVersion.
        /// </summary>
        private static void TouchScheme(CertificationScheme scheme)
        {
            scheme.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar jurisdiction-ändringen och skiljer child- och aggregate-concurrency åt.
        /// </summary>
        private async Task<Result> SaveChangesAsync(int certificationSchemeId, int certificationSchemeJurisdictionId, CancellationToken cancellationToken)
        {
            try
            {
                await _db.SaveChangesAsync(
                    cancellationToken);

                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                if (exception.Entries.Any(
                        x => x.Entity is CertificationSchemeJurisdiction))
                {
                    return Result.Fail(
                        new ConcurrencyError(
                            nameof(CertificationSchemeJurisdiction),
                            certificationSchemeJurisdictionId));
                }

                return Result.Fail(
                    new ConcurrencyError(
                        nameof(CertificationScheme),
                        certificationSchemeId));
            }
            catch (DbUpdateException exception)
                when (IsUniqueViolation(exception))
            {
                return Result.Fail(
                    new ConflictError(
                        "Another jurisdiction already uses the same Code within this CertificationScheme."));
            }
        }

        /// <summary>
        /// Projekterar scheme-versionen med jurisdictions.
        /// </summary>
        private async Task<CertificationSchemeResponse?> ProjectAsync(int certificationSchemeId, CancellationToken cancellationToken)
        {
            return await _db.Set<CertificationScheme>()
                .AsNoTracking()
                .Where(x => x.Id == certificationSchemeId)
                .Select(CertificationSchemeProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad scheme-version och kastar om den oväntat saknas.
        /// </summary>
        private async Task<CertificationSchemeResponse> ProjectRequiredAsync(int certificationSchemeId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    certificationSchemeId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"CertificationScheme '{certificationSchemeId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Kontrollerar om databasfelet kommer från en unik constraint.
        /// </summary>
        private static bool IsUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            };
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
