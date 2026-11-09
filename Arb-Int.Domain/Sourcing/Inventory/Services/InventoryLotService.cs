using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Crm;
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
    /// Hanterar inventory lot-roten med separata writes för metadata, mapping, kvantiteter och status.
    /// </summary>
    public sealed class InventoryLotService : IInventoryLotService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar lot-servicen med databas- och valideringsberoenden.
        /// </summary>
        public InventoryLotService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar en lot med origins och provenance-länkar.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> GetByIdAsync(Guid inventoryLotId, CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(inventoryLotId, cancellationToken);

            if (response is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        inventoryLotId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar lots för en supplier-relation.
        /// </summary>
        public async Task<Result<PagedResult<InventoryLotListResponse>>> QueryAsync(Guid supplierBusinessRelationshipId, InventoryLotQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<InventoryLotListResponse>>(
                    validationResult.Errors);
            }

            var relationshipResult = await ValidateSupplierRelationshipAsync(
                supplierBusinessRelationshipId,
                cancellationToken);

            if (relationshipResult.IsFailed)
            {
                return Result.Fail<PagedResult<InventoryLotListResponse>>(
                    relationshipResult.Errors);
            }

            IQueryable<InventoryLot> query = _db.Set<InventoryLot>()
                .AsNoTracking()
                .Where(x => x.SupplierBusinessRelationshipId == supplierBusinessRelationshipId);

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            if (request.SupplierMaterialOfferingId.HasValue)
            {
                query = query.Where(x => x.SupplierMaterialOfferingId == request.SupplierMaterialOfferingId.Value);
            }

            if (request.MaterialId.HasValue)
            {
                query = query.Where(x => x.MaterialId == request.MaterialId.Value);
            }

            if (request.CurrentLocationId.HasValue)
            {
                query = query.Where(x => x.CurrentLocationId == request.CurrentLocationId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var pattern = $"%{request.Search.Trim()}%";

                query = query.Where(x =>
                    EF.Functions.ILike(x.LotNumber, pattern)
                    || EF.Functions.ILike(x.Name, pattern)
                    || (x.SupplierLotNumber != null && EF.Functions.ILike(x.SupplierLotNumber, pattern))
                    || (x.WarehouseReference != null && EF.Functions.ILike(x.WarehouseReference, pattern)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(
                query,
                request.SortBy,
                request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(InventoryLotProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<InventoryLotListResponse>
                {
                    Items = items,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                });
        }

        /// <summary>
        /// Skapar en planned lot och validerar supplier-, offering-, material-, unit- och location-referenser.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> CreateAsync(Guid supplierBusinessRelationshipId, CreateInventoryLotRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    validationResult.Errors);
            }

            var relationshipResult = await ValidateSupplierRelationshipAsync(
                supplierBusinessRelationshipId,
                cancellationToken);

            if (relationshipResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    relationshipResult.Errors);
            }

            var lotNumber = request.LotNumber.Trim();

            if (await _db.Set<InventoryLot>().AsNoTracking().AnyAsync(x => x.LotNumber == lotNumber, cancellationToken))
            {
                return Result.Fail<InventoryLotResponse>(
                    new ConflictError(
                        $"InventoryLot number '{lotNumber}' already exists."));
            }

            var referencesResult = await ValidateReferencesAsync(
                supplierBusinessRelationshipId,
                request.SupplierMaterialOfferingId,
                request.MaterialId,
                request.QuantityUnitId,
                request.CurrentLocationId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    referencesResult.Errors);
            }

            var lot = new InventoryLot
            {
                Id = Guid.CreateVersion7(),
                SupplierBusinessRelationshipId = supplierBusinessRelationshipId,
                SupplierMaterialOfferingId = request.SupplierMaterialOfferingId,
                MaterialId = request.MaterialId,
                LotNumber = lotNumber,
                SupplierLotNumber = TrimToNull(request.SupplierLotNumber),
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Status = InventoryLotStatus.Planned,
                TotalQuantity = request.TotalQuantity,
                AvailableQuantity = request.AvailableQuantity,
                ReservedQuantity = request.ReservedQuantity,
                QuantityUnitId = request.QuantityUnitId,
                HarvestedAtUtc = request.HarvestedAtUtc,
                ProducedAtUtc = request.ProducedAtUtc,
                BestBeforeUtc = request.BestBeforeUtc,
                ExpiresAtUtc = request.ExpiresAtUtc,
                CurrentLocationId = request.CurrentLocationId,
                WarehouseReference = TrimToNull(request.WarehouseReference),
                InternalNotes = TrimToNull(request.InternalNotes)
            };

            _db.Set<InventoryLot>().Add(lot);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueViolation(exception))
            {
                return Result.Fail<InventoryLotResponse>(
                    new ConflictError(
                        $"InventoryLot number '{lotNumber}' already exists."));
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    lot.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar metadata utan att ändra lotnummer, supplier-relation, mapping, kvantiteter eller status.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> UpdateAsync(Guid inventoryLotId, UpdateInventoryLotRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    validationResult.Errors);
            }

            var lot = await _db.Set<InventoryLot>()
                .SingleOrDefaultAsync(
                    x => x.Id == inventoryLotId,
                    cancellationToken);

            if (lot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        inventoryLotId));
            }

            if (request.CurrentLocationId.HasValue)
            {
                var locationExists = await _db.Set<GeographicLocation>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == request.CurrentLocationId.Value,
                        cancellationToken);

                if (!locationExists)
                {
                    return Result.Fail<InventoryLotResponse>(
                        new NotFoundError(
                            nameof(GeographicLocation),
                            request.CurrentLocationId.Value));
                }
            }

            _db.Entry(lot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            lot.SupplierLotNumber = TrimToNull(request.SupplierLotNumber);
            lot.Name = request.Name.Trim();
            lot.Description = request.Description.Trim();
            lot.HarvestedAtUtc = request.HarvestedAtUtc;
            lot.ProducedAtUtc = request.ProducedAtUtc;
            lot.BestBeforeUtc = request.BestBeforeUtc;
            lot.ExpiresAtUtc = request.ExpiresAtUtc;
            lot.CurrentLocationId = request.CurrentLocationId;
            lot.WarehouseReference = TrimToNull(request.WarehouseReference);
            lot.InternalNotes = TrimToNull(request.InternalNotes);

            var saveResult = await SaveChangesAsync(
                lot.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    lot.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Sätter offering- och materialmapping och förhindrar att befintliga offering-bundna referenser blir inkonsistenta.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> SetMappingAsync(Guid inventoryLotId, SetInventoryLotMappingRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    validationResult.Errors);
            }

            var lot = await _db.Set<InventoryLot>()
                .SingleOrDefaultAsync(
                    x => x.Id == inventoryLotId,
                    cancellationToken);

            if (lot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        inventoryLotId));
            }

            var referencesResult = await ValidateMappingReferencesAsync(
                lot.SupplierBusinessRelationshipId,
                request.SupplierMaterialOfferingId,
                request.MaterialId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    referencesResult.Errors);
            }

            var dependencyResult = await ValidateOfferingRemapDependenciesAsync(
                lot.Id,
                request.SupplierMaterialOfferingId,
                cancellationToken);

            if (dependencyResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    dependencyResult.Errors);
            }

            _db.Entry(lot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            lot.SupplierMaterialOfferingId = request.SupplierMaterialOfferingId;
            lot.MaterialId = request.MaterialId;

            var saveResult = await SaveChangesAsync(
                lot.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    lot.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Sätter total, available och reserved quantity samt deras gemensamma måttenhet.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> SetQuantitiesAsync(Guid inventoryLotId, SetInventoryLotQuantitiesRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    validationResult.Errors);
            }

            var lot = await _db.Set<InventoryLot>()
                .SingleOrDefaultAsync(
                    x => x.Id == inventoryLotId,
                    cancellationToken);

            if (lot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        inventoryLotId));
            }

            var unitExists = await _db.Set<UnitOfMeasure>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == request.QuantityUnitId,
                    cancellationToken);

            if (!unitExists)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(UnitOfMeasure),
                        request.QuantityUnitId));
            }

            _db.Entry(lot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            lot.TotalQuantity = request.TotalQuantity;
            lot.AvailableQuantity = request.AvailableQuantity;
            lot.ReservedQuantity = request.ReservedQuantity;
            lot.QuantityUnitId = request.QuantityUnitId;

            var saveResult = await SaveChangesAsync(
                lot.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    lot.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Ändrar lot-status utan att införa en påhittad transition graph.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> ChangeStatusAsync(Guid inventoryLotId, ChangeInventoryLotStatusRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    validationResult.Errors);
            }

            var lot = await _db.Set<InventoryLot>()
                .SingleOrDefaultAsync(
                    x => x.Id == inventoryLotId,
                    cancellationToken);

            if (lot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        inventoryLotId));
            }

            _db.Entry(lot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            lot.Status = request.Status;

            var saveResult = await SaveChangesAsync(
                lot.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    lot.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Kontrollerar att relationen finns och är av supplier-typ.
        /// </summary>
        private async Task<Result> ValidateSupplierRelationshipAsync(Guid supplierBusinessRelationshipId, CancellationToken cancellationToken)
        {
            var relationship = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .Where(x => x.Id == supplierBusinessRelationshipId)
                .Select(x => new
                {
                    x.Id,
                    RelationshipTypeCode = x.BusinessRelationshipType.Code
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (relationship is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(BusinessRelationship),
                        supplierBusinessRelationshipId));
            }

            if (relationship.RelationshipTypeCode != BusinessRelationshipTypeCodes.Supplier)
            {
                return Result.Fail(
                    new ConflictError(
                        "InventoryLot can only belong to a supplier business relationship."));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar samtliga referenser som kan anges när en lot skapas.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(Guid supplierBusinessRelationshipId, Guid? supplierMaterialOfferingId, Guid? materialId, int quantityUnitId, Guid? currentLocationId, CancellationToken cancellationToken)
        {
            var mappingResult = await ValidateMappingReferencesAsync(
                supplierBusinessRelationshipId,
                supplierMaterialOfferingId,
                materialId,
                cancellationToken);

            if (mappingResult.IsFailed)
            {
                return mappingResult;
            }

            var unitExists = await _db.Set<UnitOfMeasure>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == quantityUnitId,
                    cancellationToken);

            if (!unitExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(UnitOfMeasure),
                        quantityUnitId));
            }

            if (currentLocationId.HasValue)
            {
                var locationExists = await _db.Set<GeographicLocation>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == currentLocationId.Value,
                        cancellationToken);

                if (!locationExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(GeographicLocation),
                            currentLocationId.Value));
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar offering- och materialmapping mot supplier-relationen och masterdata.
        /// </summary>
        private async Task<Result> ValidateMappingReferencesAsync(Guid supplierBusinessRelationshipId, Guid? supplierMaterialOfferingId, Guid? materialId, CancellationToken cancellationToken)
        {
            if (supplierMaterialOfferingId.HasValue)
            {
                var offering = await _db.Set<SupplierMaterialOffering>()
                    .AsNoTracking()
                    .Where(x => x.Id == supplierMaterialOfferingId.Value)
                    .Select(x => new
                    {
                        x.Id,
                        x.BusinessRelationshipId
                    })
                    .SingleOrDefaultAsync(cancellationToken);

                if (offering is null)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(SupplierMaterialOffering),
                            supplierMaterialOfferingId.Value));
                }

                if (offering.BusinessRelationshipId != supplierBusinessRelationshipId)
                {
                    return Result.Fail(
                        new ConflictError(
                            "The SupplierMaterialOffering must belong to the same supplier business relationship as the InventoryLot."));
                }
            }

            if (materialId.HasValue)
            {
                var materialExists = await _db.Set<Material>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == materialId.Value,
                        cancellationToken);

                if (!materialExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(Material),
                            materialId.Value));
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// Förhindrar en offering-remap som skulle göra lot-bundna availability-, analysis- eller document-poster inkonsistenta.
        /// </summary>
        private async Task<Result> ValidateOfferingRemapDependenciesAsync(Guid inventoryLotId, Guid? requestedSupplierMaterialOfferingId, CancellationToken cancellationToken)
        {
            var conflictingAvailabilityExists = await _db.Set<SupplierOfferingAvailability>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.InventoryLotId == inventoryLotId
                        && x.SupplierMaterialOfferingId != requestedSupplierMaterialOfferingId,
                    cancellationToken);

            if (conflictingAvailabilityExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "The InventoryLot cannot be remapped while a lot-specific availability belongs to another SupplierMaterialOffering."));
            }

            var conflictingAnalysisExists = await _db.Set<SupplyAnalysis>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.InventoryLotId == inventoryLotId
                        && x.SupplierMaterialOfferingId.HasValue
                        && x.SupplierMaterialOfferingId != requestedSupplierMaterialOfferingId,
                    cancellationToken);

            if (conflictingAnalysisExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "The InventoryLot cannot be remapped while a SupplyAnalysis explicitly references another SupplierMaterialOffering."));
            }

            var conflictingDocumentExists = await _db.Set<SupplyDocument>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.InventoryLotId == inventoryLotId
                        && x.SupplierMaterialOfferingId.HasValue
                        && x.SupplierMaterialOfferingId != requestedSupplierMaterialOfferingId,
                    cancellationToken);

            if (conflictingDocumentExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "The InventoryLot cannot be remapped while a SupplyDocument explicitly references another SupplierMaterialOffering."));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Sparar lot-ändringen och översätter concurrency- och unique-fel till domänfel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid inventoryLotId, CancellationToken cancellationToken)
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
                        nameof(InventoryLot),
                        inventoryLotId));
            }
            catch (DbUpdateException exception) when (IsUniqueViolation(exception))
            {
                return Result.Fail(
                    new ConflictError(
                        "Another InventoryLot already uses the same globally unique LotNumber."));
            }
        }

        /// <summary>
        /// Projekterar en lot direkt från databasen.
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
            return await ProjectAsync(
                    inventoryLotId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"InventoryLot '{inventoryLotId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Lägger på stabil sortering före paginering.
        /// </summary>
        private static IQueryable<InventoryLot> ApplySorting(IQueryable<InventoryLot> query, InventoryLotSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (InventoryLotSortField.Name, SortDirection.Descending) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (InventoryLotSortField.LotNumber, SortDirection.Ascending) => query.OrderBy(x => x.LotNumber).ThenBy(x => x.Id),
                (InventoryLotSortField.LotNumber, SortDirection.Descending) => query.OrderByDescending(x => x.LotNumber).ThenByDescending(x => x.Id),
                (InventoryLotSortField.Status, SortDirection.Ascending) => query.OrderBy(x => x.Status).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (InventoryLotSortField.Status, SortDirection.Descending) => query.OrderByDescending(x => x.Status).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (InventoryLotSortField.TotalQuantity, SortDirection.Ascending) => query.OrderBy(x => x.TotalQuantity).ThenBy(x => x.Id),
                (InventoryLotSortField.TotalQuantity, SortDirection.Descending) => query.OrderByDescending(x => x.TotalQuantity).ThenByDescending(x => x.Id),
                (InventoryLotSortField.AvailableQuantity, SortDirection.Ascending) => query.OrderBy(x => x.AvailableQuantity).ThenBy(x => x.Id),
                (InventoryLotSortField.AvailableQuantity, SortDirection.Descending) => query.OrderByDescending(x => x.AvailableQuantity).ThenByDescending(x => x.Id),
                (InventoryLotSortField.BestBeforeUtc, SortDirection.Ascending) => query.OrderBy(x => x.BestBeforeUtc).ThenBy(x => x.Id),
                (InventoryLotSortField.BestBeforeUtc, SortDirection.Descending) => query.OrderByDescending(x => x.BestBeforeUtc).ThenByDescending(x => x.Id),
                (InventoryLotSortField.ExpiresAtUtc, SortDirection.Ascending) => query.OrderBy(x => x.ExpiresAtUtc).ThenBy(x => x.Id),
                (InventoryLotSortField.ExpiresAtUtc, SortDirection.Descending) => query.OrderByDescending(x => x.ExpiresAtUtc).ThenByDescending(x => x.Id),
                (InventoryLotSortField.CreateDateUtc, SortDirection.Ascending) => query.OrderBy(x => x.CreateDateUtc).ThenBy(x => x.Id),
                (InventoryLotSortField.CreateDateUtc, SortDirection.Descending) => query.OrderByDescending(x => x.CreateDateUtc).ThenByDescending(x => x.Id),
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
