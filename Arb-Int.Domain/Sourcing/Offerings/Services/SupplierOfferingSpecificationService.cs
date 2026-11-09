using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar offering-specifications med child- och aggregate-concurrency samt provenance-kontroller.
    /// </summary>
    public sealed class SupplierOfferingSpecificationService : ISupplierOfferingSpecificationService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplierOfferingSpecificationService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Skapar en specification efter kontroll av canonical target och supporting document.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> CreateAsync(Guid supplierMaterialOfferingId, CreateSupplierOfferingSpecificationRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(validationResult.Errors);
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        supplierMaterialOfferingId));
            }

            var referencesResult = await ValidateReferencesAsync(
                offering,
                request.ConstituentId,
                request.CharacteristicId,
                request.UnitOfMeasureId,
                request.SupplyDocumentId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(referencesResult.Errors);
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            var specification = new SupplierOfferingSpecification
            {
                Id = Guid.CreateVersion7(),
                SupplierMaterialOfferingId = offering.Id,
                Kind = request.Kind,
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                ConstituentId = request.ConstituentId,
                CharacteristicId = request.CharacteristicId,
                Operator = request.Operator,
                MinimumValue = request.MinimumValue,
                MaximumValue = request.MaximumValue,
                TargetValue = request.TargetValue,
                TextValue = TrimToNull(request.TextValue),
                UnitOfMeasureId = request.UnitOfMeasureId,
                Basis = request.Basis,
                AnalysisMethod = TrimToNull(request.AnalysisMethod),
                VerificationState = request.VerificationState,
                ValidFromUtc = request.ValidFromUtc,
                ValidToUtc = request.ValidToUtc,
                SupplyDocumentId = request.SupplyDocumentId
            };

            _db.Set<SupplierOfferingSpecification>()
                .Add(specification);

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
        /// Uppdaterar specificationen och behåller dess offering-identitet oförändrad.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(Guid supplierOfferingSpecificationId, UpdateSupplierOfferingSpecificationRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(validationResult.Errors);
            }

            var specification = await _db.Set<SupplierOfferingSpecification>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierOfferingSpecificationId,
                    cancellationToken);

            if (specification is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierOfferingSpecification),
                        supplierOfferingSpecificationId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == specification.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        specification.SupplierMaterialOfferingId));
            }

            var referencesResult = await ValidateReferencesAsync(
                offering,
                request.ConstituentId,
                request.CharacteristicId,
                request.UnitOfMeasureId,
                request.SupplyDocumentId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(referencesResult.Errors);
            }

            _db.Entry(specification)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            specification.Kind = request.Kind;
            specification.Name = request.Name.Trim();
            specification.Description = request.Description.Trim();
            specification.ConstituentId = request.ConstituentId;
            specification.CharacteristicId = request.CharacteristicId;
            specification.Operator = request.Operator;
            specification.MinimumValue = request.MinimumValue;
            specification.MaximumValue = request.MaximumValue;
            specification.TargetValue = request.TargetValue;
            specification.TextValue = TrimToNull(request.TextValue);
            specification.UnitOfMeasureId = request.UnitOfMeasureId;
            specification.Basis = request.Basis;
            specification.AnalysisMethod = TrimToNull(request.AnalysisMethod);
            specification.VerificationState = request.VerificationState;
            specification.ValidFromUtc = request.ValidFromUtc;
            specification.ValidToUtc = request.ValidToUtc;
            specification.SupplyDocumentId = request.SupplyDocumentId;

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
                        specification.Id));
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort specificationen och concurrency-skyddar både child-raden och offering-rooten.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierOfferingSpecificationId, uint rowVersion, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default)
        {
            var specification = await _db.Set<SupplierOfferingSpecification>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierOfferingSpecificationId,
                    cancellationToken);

            if (specification is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierOfferingSpecification),
                        supplierOfferingSpecificationId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == specification.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        specification.SupplierMaterialOfferingId));
            }

            _db.Entry(specification)
                .Property(x => x.RowVersion)
                .OriginalValue = rowVersion;

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = supplierMaterialOfferingRowVersion;

            _db.Set<SupplierOfferingSpecification>()
                .Remove(specification);

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
                        specification.Id));
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Kontrollerar canonical target, måttenhet och supporting document mot offering-kontexten.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(SupplierMaterialOffering offering, Guid? constituentId, Guid? characteristicId, int? unitOfMeasureId, Guid? supplyDocumentId, CancellationToken cancellationToken)
        {
            if (constituentId.HasValue)
            {
                var constituentExists = await _db.Set<Constituent>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == constituentId.Value,
                        cancellationToken);

                if (!constituentExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(Constituent),
                            constituentId.Value));
                }
            }

            if (characteristicId.HasValue)
            {
                var characteristicExists = await _db.Set<Characteristic>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == characteristicId.Value,
                        cancellationToken);

                if (!characteristicExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(Characteristic),
                            characteristicId.Value));
                }
            }

            if (unitOfMeasureId.HasValue)
            {
                var unitExists = await _db.Set<UnitOfMeasure>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == unitOfMeasureId.Value,
                        cancellationToken);

                if (!unitExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(UnitOfMeasure),
                            unitOfMeasureId.Value));
                }
            }

            if (supplyDocumentId.HasValue)
            {
                var document = await _db.Set<SupplyDocument>()
                    .AsNoTracking()
                    .Where(x => x.Id == supplyDocumentId.Value)
                    .Select(
                        x => new
                        {
                            x.Id,
                            x.SubjectBusinessRelationshipId,
                            x.SupplierMaterialOfferingId,
                            x.InventoryLotId
                        })
                    .SingleOrDefaultAsync(cancellationToken);

                if (document is null)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(SupplyDocument),
                            supplyDocumentId.Value));
                }

                if (document.SubjectBusinessRelationshipId.HasValue
                    && document.SubjectBusinessRelationshipId.Value != offering.BusinessRelationshipId)
                {
                    return Result.Fail(
                        new ConflictError(
                            "The supporting SupplyDocument belongs to another business relationship."));
                }

                if (document.SupplierMaterialOfferingId.HasValue
                    && document.SupplierMaterialOfferingId.Value != offering.Id)
                {
                    return Result.Fail(
                        new ConflictError(
                            "The supporting SupplyDocument is explicitly linked to another SupplierMaterialOffering."));
                }

                Guid? lotOfferingId = null;

                if (document.InventoryLotId.HasValue)
                {
                    lotOfferingId = await _db.Set<InventoryLot>()
                        .AsNoTracking()
                        .Where(x => x.Id == document.InventoryLotId.Value)
                        .Select(x => x.SupplierMaterialOfferingId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (lotOfferingId.HasValue
                        && lotOfferingId.Value != offering.Id)
                    {
                        return Result.Fail(
                            new ConflictError(
                                "The supporting SupplyDocument is linked to an InventoryLot from another SupplierMaterialOffering."));
                    }
                }

                var isRelevant =
                    document.SupplierMaterialOfferingId == offering.Id
                    || document.SubjectBusinessRelationshipId == offering.BusinessRelationshipId
                    || lotOfferingId == offering.Id;

                if (!isRelevant)
                {
                    return Result.Fail(
                        new ConflictError(
                            "The supporting SupplyDocument must be linked to this SupplierMaterialOffering, its supplier relationship, or an InventoryLot belonging to the offering."));
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// Markerar offering-roten så att specification-writes konkurrerar med övriga aggregate-writes.
        /// </summary>
        private static void TouchOffering(SupplierMaterialOffering offering)
        {
            offering.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Väljer child- eller root-concurrencyfel beroende på vilken tracked entity som misslyckades.
        /// </summary>
        private static ConcurrencyError CreateConcurrencyError(DbUpdateConcurrencyException exception, Guid supplierMaterialOfferingId, Guid supplierOfferingSpecificationId)
        {
            if (exception.Entries.Any(
                    x => x.Entity is SupplierOfferingSpecification))
            {
                return new ConcurrencyError(
                    nameof(SupplierOfferingSpecification),
                    supplierOfferingSpecificationId);
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
