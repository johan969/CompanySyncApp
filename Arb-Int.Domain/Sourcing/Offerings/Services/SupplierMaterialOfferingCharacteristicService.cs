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
    /// Hanterar offering-characteristics med SupplierMaterialOffering som concurrency-root.
    /// </summary>
    public sealed class SupplierMaterialOfferingCharacteristicService : ISupplierMaterialOfferingCharacteristicService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplierMaterialOfferingCharacteristicService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Lägger till en befintlig characteristic om offeringet inte redan har samma tagg.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> AddAsync(Guid supplierMaterialOfferingId, CreateSupplierMaterialOfferingCharacteristicRequest request, CancellationToken cancellationToken = default)
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

            var characteristicExists = await _db.Set<Characteristic>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.CharacteristicId, cancellationToken);

            if (!characteristicExists)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(nameof(Characteristic), request.CharacteristicId));
            }

            var duplicateExists = await _db.Set<SupplierMaterialOfferingCharacteristic>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.SupplierMaterialOfferingId == offering.Id
                        && x.CharacteristicId == request.CharacteristicId,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new ConflictError(
                        "The supplier material offering already contains this characteristic."));
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            var link = new SupplierMaterialOfferingCharacteristic
            {
                Id = Guid.CreateVersion7(),
                SupplierMaterialOfferingId = offering.Id,
                CharacteristicId = request.CharacteristicId
            };

            _db.Set<SupplierMaterialOfferingCharacteristic>()
                .Add(link);

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
        /// Tar bort characteristic-kopplingen och behåller characteristic som masterdata.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierMaterialOfferingCharacteristicId, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default)
        {
            var link = await _db.Set<SupplierMaterialOfferingCharacteristic>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierMaterialOfferingCharacteristicId,
                    cancellationToken);

            if (link is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOfferingCharacteristic),
                        supplierMaterialOfferingCharacteristicId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == link.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        link.SupplierMaterialOfferingId));
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = supplierMaterialOfferingRowVersion;

            _db.Set<SupplierMaterialOfferingCharacteristic>()
                .Remove(link);

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
        /// Markerar offering-roten så att characteristic-writes konkurrerar på samma RowVersion.
        /// </summary>
        private static void TouchOffering(SupplierMaterialOffering offering)
        {
            offering.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar ändringen och översätter concurrency- och unique-fel till domänfel.
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
                        "The supplier material offering already contains this characteristic."));
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
    }
}
