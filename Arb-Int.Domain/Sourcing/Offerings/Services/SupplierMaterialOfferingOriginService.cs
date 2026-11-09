using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar offering-origins med högst en primary per offering och origin-roll.
    /// </summary>
    public sealed class SupplierMaterialOfferingOriginService : ISupplierMaterialOfferingOriginService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplierMaterialOfferingOriginService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Lägger till en geografisk origin och växlar primary atomiskt inom den valda rollen.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> AddAsync(Guid supplierMaterialOfferingId, CreateSupplierMaterialOfferingOriginRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(validationResult.Errors);
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(x => x.Id == supplierMaterialOfferingId, cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(nameof(SupplierMaterialOffering), supplierMaterialOfferingId));
            }

            var locationResult = await ValidateGeographicLocationAsync(
                request.GeographicLocationId,
                cancellationToken);

            if (locationResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(locationResult.Errors);
            }

            var duplicateResult = await ValidateUniqueOriginAsync(
                offering.Id,
                request.GeographicLocationId,
                request.Role,
                excludeOriginId: null,
                cancellationToken);

            if (duplicateResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(duplicateResult.Errors);
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    offering.Id,
                    request.Role,
                    excludeOriginId: null,
                    cancellationToken);
            }

            var origin = new SupplierMaterialOfferingOrigin
            {
                Id = Guid.CreateVersion7(),
                SupplierMaterialOfferingId = offering.Id,
                GeographicLocationId = request.GeographicLocationId,
                Role = request.Role,
                IsPrimary = request.IsPrimary,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<SupplierMaterialOfferingOrigin>()
                .Add(origin);

            TouchOffering(offering);

            var saveResult = await SaveChangesAsync(
                offering.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(saveResult.Errors);
            }

            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar roll och primary-status atomiskt medan GeographicLocation förblir immutable.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(Guid supplierMaterialOfferingOriginId, UpdateSupplierMaterialOfferingOriginRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(validationResult.Errors);
            }

            var origin = await _db.Set<SupplierMaterialOfferingOrigin>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierMaterialOfferingOriginId,
                    cancellationToken);

            if (origin is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOfferingOrigin),
                        supplierMaterialOfferingOriginId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == origin.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        origin.SupplierMaterialOfferingId));
            }

            var duplicateResult = await ValidateUniqueOriginAsync(
                offering.Id,
                origin.GeographicLocationId,
                request.Role,
                origin.Id,
                cancellationToken);

            if (duplicateResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(duplicateResult.Errors);
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    offering.Id,
                    request.Role,
                    origin.Id,
                    cancellationToken);
            }

            origin.Role = request.Role;
            origin.IsPrimary = request.IsPrimary;
            origin.Notes = TrimToNull(request.Notes);

            TouchOffering(offering);

            var saveResult = await SaveChangesAsync(
                offering.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(saveResult.Errors);
            }

            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort origin-kopplingen och touch:ar offeringets concurrency-root.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierMaterialOfferingOriginId, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default)
        {
            var origin = await _db.Set<SupplierMaterialOfferingOrigin>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierMaterialOfferingOriginId,
                    cancellationToken);

            if (origin is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOfferingOrigin),
                        supplierMaterialOfferingOriginId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == origin.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        origin.SupplierMaterialOfferingId));
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = supplierMaterialOfferingRowVersion;

            _db.Set<SupplierMaterialOfferingOrigin>()
                .Remove(origin);

            TouchOffering(offering);

            var saveResult = await SaveChangesAsync(
                offering.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Kontrollerar att GeographicLocation finns innan länken skapas.
        /// </summary>
        private async Task<Result> ValidateGeographicLocationAsync(Guid geographicLocationId, CancellationToken cancellationToken)
        {
            var exists = await _db.Set<GeographicLocation>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == geographicLocationId,
                    cancellationToken);

            if (exists)
            {
                return Result.Ok();
            }

            return Result.Fail(
                new NotFoundError(
                    nameof(GeographicLocation),
                    geographicLocationId));
        }

        /// <summary>
        /// Kontrollerar den unika kombinationen offering, location och role.
        /// </summary>
        private async Task<Result> ValidateUniqueOriginAsync(Guid supplierMaterialOfferingId, Guid geographicLocationId, SupplierOfferingOriginRole role, Guid? excludeOriginId, CancellationToken cancellationToken)
        {
            IQueryable<SupplierMaterialOfferingOrigin> query = _db
                .Set<SupplierMaterialOfferingOrigin>()
                .AsNoTracking()
                .Where(
                    x => x.SupplierMaterialOfferingId == supplierMaterialOfferingId
                        && x.GeographicLocationId == geographicLocationId
                        && x.Role == role);

            if (excludeOriginId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeOriginId.Value);
            }

            if (!await query.AnyAsync(cancellationToken))
            {
                return Result.Ok();
            }

            return Result.Fail(
                new ConflictError(
                    "The supplier material offering already contains this geographic location with the selected origin role."));
        }

        /// <summary>
        /// Avmarkerar nuvarande primary endast inom samma offering och origin-roll.
        /// </summary>
        private Task<int> DemoteCurrentPrimaryAsync(Guid supplierMaterialOfferingId, SupplierOfferingOriginRole role, Guid? excludeOriginId, CancellationToken cancellationToken)
        {
            IQueryable<SupplierMaterialOfferingOrigin> query = _db
                .Set<SupplierMaterialOfferingOrigin>()
                .Where(
                    x => x.SupplierMaterialOfferingId == supplierMaterialOfferingId
                        && x.Role == role
                        && x.IsPrimary);

            if (excludeOriginId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeOriginId.Value);
            }

            return query.ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    x => x.IsPrimary,
                    false),
                cancellationToken);
        }

        /// <summary>
        /// Markerar offering-roten så att origin-writes konkurrerar på samma RowVersion.
        /// </summary>
        private static void TouchOffering(SupplierMaterialOffering offering)
        {
            offering.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar origin-ändringar och översätter concurrency- och unique-fel till domänfel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid supplierMaterialOfferingId, CancellationToken cancellationToken)
        {
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail(
                    new ConcurrencyError(
                        nameof(SupplierMaterialOffering),
                        supplierMaterialOfferingId));
            }
            catch (DbUpdateException exception)
                when (IsUniqueViolation(exception))
            {
                return Result.Fail(
                    new ConflictError(
                        "The supplier material offering already contains this origin, or another origin is already primary for the selected role."));
            }
        }

        /// <summary>
        /// Projekterar offeringet med samtliga offering-ägda child-data.
        /// </summary>
        private async Task<SupplierMaterialOfferingResponse?> ProjectAsync(Guid supplierMaterialOfferingId, CancellationToken cancellationToken)
        {
            return await _db.Set<SupplierMaterialOffering>()
                .AsNoTracking()
                .Where(x => x.Id == supplierMaterialOfferingId)
                .Select(SupplierMaterialOfferingProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar ett sparat offering och kastar om det oväntat saknas.
        /// </summary>
        private async Task<SupplierMaterialOfferingResponse> ProjectRequiredAsync(Guid supplierMaterialOfferingId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    supplierMaterialOfferingId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"SupplierMaterialOffering '{supplierMaterialOfferingId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Kontrollerar om databasfelet kommer från en unik constraint.
        /// </summary>
        private static bool IsUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException
                is PostgresException
                {
                    SqlState:
                        PostgresErrorCodes.UniqueViolation
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
