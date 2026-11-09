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
    /// Hanterar inventory lot origins med högst en primary per lot och origin-roll.
    /// </summary>
    public sealed class InventoryLotOriginService : IInventoryLotOriginService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public InventoryLotOriginService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Lägger till en origin-post och växlar primary atomiskt inom den valda rollen.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> AddAsync(Guid inventoryLotId, CreateInventoryLotOriginRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(validationResult.Errors);
            }

            var lot = await _db.Set<InventoryLot>()
                .SingleOrDefaultAsync(x => x.Id == inventoryLotId, cancellationToken);

            if (lot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(nameof(InventoryLot), inventoryLotId));
            }

            var locationExists = await _db.Set<GeographicLocation>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.GeographicLocationId, cancellationToken);

            if (!locationExists)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(nameof(GeographicLocation), request.GeographicLocationId));
            }

            var duplicateExists = await _db.Set<InventoryLotOrigin>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.InventoryLotId == lot.Id
                        && x.GeographicLocationId == request.GeographicLocationId
                        && x.Role == request.Role,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<InventoryLotResponse>(
                    new ConflictError(
                        "The InventoryLot already contains this geographic location with the selected origin role."));
            }

            _db.Entry(lot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.InventoryLotRowVersion;

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    lot.Id,
                    request.Role,
                    excludeOriginId: null,
                    cancellationToken);
            }

            var origin = new InventoryLotOrigin
            {
                Id = Guid.CreateVersion7(),
                InventoryLotId = lot.Id,
                GeographicLocationId = request.GeographicLocationId,
                Role = request.Role,
                VerificationStatus = request.VerificationStatus,
                IsPrimary = request.IsPrimary,
                Proportion = request.Proportion,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<InventoryLotOrigin>().Add(origin);

            TouchLot(lot);

            var saveResult = await SaveChangesAsync(
                lot.Id,
                origin.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(saveResult.Errors);
            }

            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(lot.Id, cancellationToken));
        }

        /// <summary>
        /// Uppdaterar origin-metadata atomiskt medan GeographicLocation förblir immutable.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> UpdateAsync(Guid inventoryLotOriginId, UpdateInventoryLotOriginRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(validationResult.Errors);
            }

            var origin = await _db.Set<InventoryLotOrigin>()
                .SingleOrDefaultAsync(x => x.Id == inventoryLotOriginId, cancellationToken);

            if (origin is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(nameof(InventoryLotOrigin), inventoryLotOriginId));
            }

            var lot = await _db.Set<InventoryLot>()
                .SingleOrDefaultAsync(x => x.Id == origin.InventoryLotId, cancellationToken);

            if (lot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(nameof(InventoryLot), origin.InventoryLotId));
            }

            var duplicateExists = await _db.Set<InventoryLotOrigin>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.InventoryLotId == lot.Id
                        && x.Id != origin.Id
                        && x.GeographicLocationId == origin.GeographicLocationId
                        && x.Role == request.Role,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<InventoryLotResponse>(
                    new ConflictError(
                        "The InventoryLot already contains this geographic location with the selected origin role."));
            }

            var notes = TrimToNull(request.Notes);

            if (origin.Role == request.Role
                && origin.VerificationStatus == request.VerificationStatus
                && origin.IsPrimary == request.IsPrimary
                && origin.Proportion == request.Proportion
                && origin.Notes == notes)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(lot.Id, cancellationToken));
            }

            _db.Entry(origin)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            _db.Entry(lot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.InventoryLotRowVersion;

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    lot.Id,
                    request.Role,
                    origin.Id,
                    cancellationToken);
            }

            origin.Role = request.Role;
            origin.VerificationStatus = request.VerificationStatus;
            origin.IsPrimary = request.IsPrimary;
            origin.Proportion = request.Proportion;
            origin.Notes = notes;

            TouchLot(lot);

            var saveResult = await SaveChangesAsync(
                lot.Id,
                origin.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(saveResult.Errors);
            }

            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(lot.Id, cancellationToken));
        }

        /// <summary>
        /// Tar bort origin-posten och concurrency-skyddar både child-raden och lot-rooten.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> DeleteAsync(Guid inventoryLotOriginId, uint rowVersion, uint inventoryLotRowVersion, CancellationToken cancellationToken = default)
        {
            var origin = await _db.Set<InventoryLotOrigin>()
                .SingleOrDefaultAsync(x => x.Id == inventoryLotOriginId, cancellationToken);

            if (origin is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(nameof(InventoryLotOrigin), inventoryLotOriginId));
            }

            var lot = await _db.Set<InventoryLot>()
                .SingleOrDefaultAsync(x => x.Id == origin.InventoryLotId, cancellationToken);

            if (lot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(nameof(InventoryLot), origin.InventoryLotId));
            }

            _db.Entry(origin)
                .Property(x => x.RowVersion)
                .OriginalValue = rowVersion;

            _db.Entry(lot)
                .Property(x => x.RowVersion)
                .OriginalValue = inventoryLotRowVersion;

            _db.Set<InventoryLotOrigin>().Remove(origin);

            TouchLot(lot);

            var saveResult = await SaveChangesAsync(
                lot.Id,
                origin.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(lot.Id, cancellationToken));
        }

        /// <summary>
        /// Avmarkerar nuvarande primary endast inom samma lot och origin-roll.
        /// </summary>
        private Task<int> DemoteCurrentPrimaryAsync(Guid inventoryLotId, OriginRole role, Guid? excludeOriginId, CancellationToken cancellationToken)
        {
            IQueryable<InventoryLotOrigin> query = _db.Set<InventoryLotOrigin>()
                .Where(
                    x => x.InventoryLotId == inventoryLotId
                        && x.Role == role
                        && x.IsPrimary);

            if (excludeOriginId.HasValue)
            {
                query = query.Where(x => x.Id != excludeOriginId.Value);
            }

            return query.ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.IsPrimary, false),
                cancellationToken);
        }

        /// <summary>
        /// Markerar lot-roten så att origin-writes konkurrerar med övriga aggregate-writes.
        /// </summary>
        private static void TouchLot(InventoryLot lot)
        {
            lot.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar origin-ändringar och översätter concurrency- och unique-fel till domänfel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid inventoryLotId, Guid inventoryLotOriginId, CancellationToken cancellationToken)
        {
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                if (exception.Entries.Any(x => x.Entity is InventoryLotOrigin))
                {
                    return Result.Fail(
                        new ConcurrencyError(nameof(InventoryLotOrigin), inventoryLotOriginId));
                }

                return Result.Fail(
                    new ConcurrencyError(nameof(InventoryLot), inventoryLotId));
            }
            catch (DbUpdateException exception)
                when (IsUniqueViolation(exception))
            {
                return Result.Fail(
                    new ConflictError(
                        "The InventoryLot already contains this origin, or another origin is already primary for the selected role."));
            }
        }

        /// <summary>
        /// Projekterar loten med origins och provenance-länkar.
        /// </summary>
        private async Task<InventoryLotResponse?> ProjectAsync(Guid inventoryLotId, CancellationToken cancellationToken)
        {
            return await _db.Set<InventoryLot>()
                .AsNoTracking()
                .Where(x => x.Id == inventoryLotId)
                .Select(InventoryLotProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad lot och kastar om den oväntat saknas.
        /// </summary>
        private async Task<InventoryLotResponse> ProjectRequiredAsync(Guid inventoryLotId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(inventoryLotId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"InventoryLot '{inventoryLotId}' was saved but could not be projected.");
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
