using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar certification schemes som versionerad reference-data med immutable standardidentitet.
    /// </summary>
    public sealed class CertificationSchemeService : ICertificationSchemeService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public CertificationSchemeService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar en scheme-version med jurisdictions.
        /// </summary>
        public async Task<Result<CertificationSchemeResponse>> GetByIdAsync(int certificationSchemeId, CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(
                certificationSchemeId,
                cancellationToken);

            if (response is null)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new NotFoundError(
                        nameof(CertificationScheme),
                        certificationSchemeId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar certification schemes.
        /// </summary>
        public async Task<Result<PagedResult<CertificationSchemeListResponse>>> QueryAsync(CertificationSchemeQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<CertificationSchemeListResponse>>(
                    validationResult.Errors);
            }

            IQueryable<CertificationScheme> query = _db.Set<CertificationScheme>()
                .AsNoTracking();

            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            if (request.Kind.HasValue)
            {
                query = query.Where(
                    x => x.Kind == request.Kind.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var pattern = $"%{request.Search.Trim()}%";

                query = query.Where(x =>
                    EF.Functions.ILike(x.Code, pattern)
                    || EF.Functions.ILike(x.Name, pattern)
                    || EF.Functions.ILike(x.Description, pattern)
                    || (x.StandardOwner != null && EF.Functions.ILike(x.StandardOwner, pattern))
                    || (x.StandardVersion != null && EF.Functions.ILike(x.StandardVersion, pattern)));
            }

            var totalCount = await query.CountAsync(
                cancellationToken);

            query = ApplySorting(
                query,
                request.SortBy,
                request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(CertificationSchemeProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<CertificationSchemeListResponse>
                {
                    Items = items,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                });
        }

        /// <summary>
        /// Skapar en ny scheme-version och låter code samt standardidentitet vara immutable efter skapandet.
        /// </summary>
        public async Task<Result<CertificationSchemeResponse>> CreateAsync(CreateCertificationSchemeRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    validationResult.Errors);
            }

            var code = request.Code.Trim();

            var duplicateExists = await _db.Set<CertificationScheme>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Code == code,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new ConflictError(
                        $"CertificationScheme code '{code}' already exists."));
            }

            var scheme = new CertificationScheme
            {
                Code = code,
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Kind = request.Kind,
                StandardOwner = request.StandardOwner.Trim(),
                StandardVersion = request.StandardVersion.Trim(),
                PublicUrl = TrimToNull(request.PublicUrl),
                ValidFromUtc = request.ValidFromUtc,
                ValidToUtc = request.ValidToUtc,
                IsActive = true
            };

            _db.Set<CertificationScheme>()
                .Add(scheme);

            try
            {
                await _db.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateException exception)
                when (IsUniqueViolation(exception))
            {
                return Result.Fail<CertificationSchemeResponse>(
                    new ConflictError(
                        $"CertificationScheme code '{code}' already exists."));
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    scheme.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar presentation och validity utan att skriva om scheme-versionens standardidentitet.
        /// </summary>
        public async Task<Result<CertificationSchemeResponse>> UpdateAsync(int certificationSchemeId, UpdateCertificationSchemeRequest request, CancellationToken cancellationToken = default)
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

            var name = request.Name.Trim();
            var description = request.Description.Trim();
            var publicUrl = TrimToNull(request.PublicUrl);

            if (scheme.Name == name
                && scheme.Description == description
                && scheme.PublicUrl == publicUrl
                && scheme.ValidFromUtc == request.ValidFromUtc
                && scheme.ValidToUtc == request.ValidToUtc)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        scheme.Id,
                        cancellationToken));
            }

            _db.Entry(scheme)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            scheme.Name = name;
            scheme.Description = description;
            scheme.PublicUrl = publicUrl;
            scheme.ValidFromUtc = request.ValidFromUtc;
            scheme.ValidToUtc = request.ValidToUtc;

            var saveResult = await SaveChangesAsync(
                scheme.Id,
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
        /// Aktiverar eller inaktiverar scheme-versionen utan att radera historiska referenser.
        /// </summary>
        public async Task<Result<CertificationSchemeResponse>> SetActiveAsync(int certificationSchemeId, SetCertificationSchemeActiveRequest request, CancellationToken cancellationToken = default)
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

            if (scheme.IsActive == request.IsActive)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        scheme.Id,
                        cancellationToken));
            }

            _db.Entry(scheme)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            scheme.IsActive = request.IsActive;

            var saveResult = await SaveChangesAsync(
                scheme.Id,
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
        /// Sparar scheme-ändringen och översätter concurrency- och unique-fel till domänfel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(int certificationSchemeId, CancellationToken cancellationToken)
        {
            try
            {
                await _db.SaveChangesAsync(
                    cancellationToken);

                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
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
                        "Another CertificationScheme already uses the same Code."));
            }
        }

        /// <summary>
        /// Projekterar en scheme-version med jurisdictions direkt från databasen.
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
        /// Lägger på stabil sortering före paginering.
        /// </summary>
        private static IQueryable<CertificationScheme> ApplySorting(IQueryable<CertificationScheme> query, CertificationSchemeSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (CertificationSchemeSortField.Name, SortDirection.Descending) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (CertificationSchemeSortField.Code, SortDirection.Ascending) => query.OrderBy(x => x.Code).ThenBy(x => x.Id),
                (CertificationSchemeSortField.Code, SortDirection.Descending) => query.OrderByDescending(x => x.Code).ThenByDescending(x => x.Id),
                (CertificationSchemeSortField.Kind, SortDirection.Ascending) => query.OrderBy(x => x.Kind).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (CertificationSchemeSortField.Kind, SortDirection.Descending) => query.OrderByDescending(x => x.Kind).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (CertificationSchemeSortField.StandardOwner, SortDirection.Ascending) => query.OrderBy(x => x.StandardOwner).ThenBy(x => x.StandardVersion).ThenBy(x => x.Id),
                (CertificationSchemeSortField.StandardOwner, SortDirection.Descending) => query.OrderByDescending(x => x.StandardOwner).ThenByDescending(x => x.StandardVersion).ThenByDescending(x => x.Id),
                (CertificationSchemeSortField.StandardVersion, SortDirection.Ascending) => query.OrderBy(x => x.StandardVersion).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (CertificationSchemeSortField.StandardVersion, SortDirection.Descending) => query.OrderByDescending(x => x.StandardVersion).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (CertificationSchemeSortField.IsActive, SortDirection.Ascending) => query.OrderBy(x => x.IsActive).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (CertificationSchemeSortField.IsActive, SortDirection.Descending) => query.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
            };
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
