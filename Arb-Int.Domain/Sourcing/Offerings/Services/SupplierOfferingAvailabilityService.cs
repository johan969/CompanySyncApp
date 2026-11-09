using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar supplier offering availability med både child- och aggregate-concurrency.
    /// </summary>
    public sealed class SupplierOfferingAvailabilityService : ISupplierOfferingAvailabilityService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplierOfferingAvailabilityService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Skapar en generell availability eller en lot-specifik availability för offeringet.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> CreateAsync(Guid supplierMaterialOfferingId, CreateSupplierOfferingAvailabilityRequest request, CancellationToken cancellationToken = default)
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

            var referencesResult = await ValidateReferencesAsync(
                offering.Id,
                request.InventoryLotId,
                request.QuantityUnitId,
                request.MinimumOrderUnitId,
                request.AvailabilityLocationId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(referencesResult.Errors);
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            var availability = new SupplierOfferingAvailability
            {
                Id = Guid.CreateVersion7(),
                SupplierMaterialOfferingId = offering.Id,
                InventoryLotId = request.InventoryLotId,
                Status = request.Status,
                AvailableQuantity = request.AvailableQuantity,
                ReservedQuantity = request.ReservedQuantity,
                QuantityUnitId = request.QuantityUnitId,
                MinimumOrderQuantity = request.MinimumOrderQuantity,
                MinimumOrderUnitId = request.MinimumOrderUnitId,
                AvailableFromUtc = request.AvailableFromUtc,
                AvailableToUtc = request.AvailableToUtc,
                LeadTimeDays = request.LeadTimeDays,
                AvailabilityLocationId = request.AvailabilityLocationId,
                SourceReference = TrimToNull(request.SourceReference),
                LastConfirmedAtUtc = request.LastConfirmedAtUtc
            };

            _db.Set<SupplierOfferingAvailability>()
                .Add(availability);

            TouchOffering(offering);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new ConcurrencyError(
                        nameof(SupplierMaterialOffering),
                        offering.Id));
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar availability-raden och behåller offering- och lot-identiteten oförändrad.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(Guid supplierOfferingAvailabilityId, UpdateSupplierOfferingAvailabilityRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(validationResult.Errors);
            }

            var availability = await _db.Set<SupplierOfferingAvailability>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierOfferingAvailabilityId,
                    cancellationToken);

            if (availability is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierOfferingAvailability),
                        supplierOfferingAvailabilityId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == availability.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        availability.SupplierMaterialOfferingId));
            }

            var referencesResult = await ValidateReferencesAsync(
                offering.Id,
                availability.InventoryLotId,
                request.QuantityUnitId,
                request.MinimumOrderUnitId,
                request.AvailabilityLocationId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(referencesResult.Errors);
            }

            _db.Entry(availability)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            availability.Status = request.Status;
            availability.AvailableQuantity = request.AvailableQuantity;
            availability.ReservedQuantity = request.ReservedQuantity;
            availability.QuantityUnitId = request.QuantityUnitId;
            availability.MinimumOrderQuantity = request.MinimumOrderQuantity;
            availability.MinimumOrderUnitId = request.MinimumOrderUnitId;
            availability.AvailableFromUtc = request.AvailableFromUtc;
            availability.AvailableToUtc = request.AvailableToUtc;
            availability.LeadTimeDays = request.LeadTimeDays;
            availability.AvailabilityLocationId = request.AvailabilityLocationId;
            availability.SourceReference = TrimToNull(request.SourceReference);
            availability.LastConfirmedAtUtc = request.LastConfirmedAtUtc;

            TouchOffering(offering);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException exception)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    CreateConcurrencyError(
                        exception,
                        offering.Id,
                        availability.Id));
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort availability-raden och concurrency-skyddar både child och offering-root.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierOfferingAvailabilityId, uint rowVersion, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default)
        {
            var availability = await _db.Set<SupplierOfferingAvailability>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierOfferingAvailabilityId,
                    cancellationToken);

            if (availability is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierOfferingAvailability),
                        supplierOfferingAvailabilityId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == availability.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        availability.SupplierMaterialOfferingId));
            }

            _db.Entry(availability)
                .Property(x => x.RowVersion)
                .OriginalValue = rowVersion;

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = supplierMaterialOfferingRowVersion;

            _db.Set<SupplierOfferingAvailability>()
                .Remove(availability);

            TouchOffering(offering);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException exception)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    CreateConcurrencyError(
                        exception,
                        offering.Id,
                        availability.Id));
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Kontrollerar lot-provenance samt unit- och location-referenser som används av availability-posten.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(Guid supplierMaterialOfferingId, Guid? inventoryLotId, int quantityUnitId, int? minimumOrderUnitId, Guid? availabilityLocationId, CancellationToken cancellationToken)
        {
            if (inventoryLotId.HasValue)
            {
                var lot = await _db.Set<InventoryLot>()
                    .AsNoTracking()
                    .Where(x => x.Id == inventoryLotId.Value)
                    .Select(
                        x => new
                        {
                            x.Id,
                            x.SupplierMaterialOfferingId
                        })
                    .SingleOrDefaultAsync(cancellationToken);

                if (lot is null)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(InventoryLot),
                            inventoryLotId.Value));
                }

                if (lot.SupplierMaterialOfferingId != supplierMaterialOfferingId)
                {
                    return Result.Fail(
                        new ConflictError(
                            "A lot-specific supplier offering availability requires an InventoryLot that belongs to the same SupplierMaterialOffering."));
                }
            }

            var quantityUnitExists = await _db.Set<UnitOfMeasure>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == quantityUnitId,
                    cancellationToken);

            if (!quantityUnitExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(UnitOfMeasure),
                        quantityUnitId));
            }

            if (minimumOrderUnitId.HasValue)
            {
                var minimumOrderUnitExists = await _db.Set<UnitOfMeasure>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == minimumOrderUnitId.Value,
                        cancellationToken);

                if (!minimumOrderUnitExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(UnitOfMeasure),
                            minimumOrderUnitId.Value));
                }
            }

            if (availabilityLocationId.HasValue)
            {
                var locationExists = await _db.Set<GeographicLocation>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == availabilityLocationId.Value,
                        cancellationToken);

                if (!locationExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(GeographicLocation),
                            availabilityLocationId.Value));
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// Markerar offering-roten så att availability-writes konkurrerar med övriga aggregate-writes.
        /// </summary>
        private static void TouchOffering(SupplierMaterialOffering offering)
        {
            offering.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Väljer child- eller root-concurrencyfel beroende på vilken tracked entity som misslyckades.
        /// </summary>
        private static ConcurrencyError CreateConcurrencyError(DbUpdateConcurrencyException exception, Guid supplierMaterialOfferingId, Guid supplierOfferingAvailabilityId)
        {
            if (exception.Entries.Any(
                    x => x.Entity is SupplierOfferingAvailability))
            {
                return new ConcurrencyError(
                    nameof(SupplierOfferingAvailability),
                    supplierOfferingAvailabilityId);
            }

            return new ConcurrencyError(
                nameof(SupplierMaterialOffering),
                supplierMaterialOfferingId);
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
