using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Trading;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar supply document-roten med immutable subjects och explicit status och validity.
    /// </summary>
    public sealed class SupplyDocumentService : ISupplyDocumentService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar document-servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplyDocumentService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar ett supply document.
        /// </summary>
        public async Task<Result<SupplyDocumentResponse>> GetByIdAsync(Guid supplyDocumentId, CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(
                supplyDocumentId,
                cancellationToken);

            if (response is null)
            {
                return Result.Fail<SupplyDocumentResponse>(
                    new NotFoundError(
                        nameof(SupplyDocument),
                        supplyDocumentId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar supply documents.
        /// </summary>
        public async Task<Result<PagedResult<SupplyDocumentListResponse>>> QueryAsync(SupplyDocumentQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<SupplyDocumentListResponse>>(
                    validationResult.Errors);
            }

            IQueryable<SupplyDocument> query = _db.Set<SupplyDocument>()
                .AsNoTracking();

            if (request.DocumentTypeId.HasValue)
            {
                query = query.Where(
                    x => x.DocumentTypeId == request.DocumentTypeId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(
                    x => x.Status == request.Status.Value);
            }


            if (request.SubjectBusinessRelationshipId.HasValue)
            {
                query = query.Where(
                    x => x.SubjectBusinessRelationshipId == request.SubjectBusinessRelationshipId.Value);
            }

            if (request.SupplierMaterialOfferingId.HasValue)
            {
                query = query.Where(
                    x => x.SupplierMaterialOfferingId == request.SupplierMaterialOfferingId.Value);
            }

            if (request.SupplierProcessCapabilityId.HasValue)
            {
                query = query.Where(
                    x => x.SupplierProcessCapabilityId == request.SupplierProcessCapabilityId.Value);
            }

            if (request.InventoryLotId.HasValue)
            {
                query = query.Where(
                    x => x.InventoryLotId == request.InventoryLotId.Value);
            }

            if (request.ShipmentId.HasValue)
            {
                query = query.Where(
                    x => x.ShipmentId == request.ShipmentId.Value);
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
                .Select(SupplyDocumentProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<SupplyDocumentListResponse>
                {
                    Items = items,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                });
        }

        /// <summary>
        /// Registrerar ett document efter referens- och supplier-kontextkontroll.
        /// </summary>
        public async Task<Result<SupplyDocumentResponse>> CreateAsync(CreateSupplyDocumentRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplyDocumentResponse>(
                    validationResult.Errors);
            }

            var referencesResult = await ValidateReferencesAsync(
                request,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<SupplyDocumentResponse>(
                    referencesResult.Errors);
            }

            var document = new SupplyDocument
            {
                Id = Guid.CreateVersion7(),
                DocumentTypeId = request.DocumentTypeId,
                SubjectBusinessRelationshipId = request.SubjectBusinessRelationshipId,
                SupplierMaterialOfferingId = request.SupplierMaterialOfferingId,
                SupplierProcessCapabilityId = request.SupplierProcessCapabilityId,
                InventoryLotId = request.InventoryLotId,
                ShipmentId = request.ShipmentId,
                Name = request.Name.Trim(),
                Status = DocumentStatus.Requested,
                ValidFromUtc = request.ValidFromUtc,
                ValidToUtc = request.ValidToUtc
            };

            _db.Set<SupplyDocument>()
                .Add(document);

            await _db.SaveChangesAsync(
                cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    document.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Sätter giltighetsfönstret utan att ändra document subjects.
        /// </summary>
        public async Task<Result<SupplyDocumentResponse>> SetValidityAsync(Guid supplyDocumentId, SetSupplyDocumentValidityRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplyDocumentResponse>(
                    validationResult.Errors);
            }

            var document = await _db.Set<SupplyDocument>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplyDocumentId,
                    cancellationToken);

            if (document is null)
            {
                return Result.Fail<SupplyDocumentResponse>(
                    new NotFoundError(
                        nameof(SupplyDocument),
                        supplyDocumentId));
            }

            if (document.ValidFromUtc == request.ValidFromUtc
                && document.ValidToUtc == request.ValidToUtc)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        document.Id,
                        cancellationToken));
            }

            _db.Entry(document)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            document.ValidFromUtc = request.ValidFromUtc;
            document.ValidToUtc = request.ValidToUtc;

            var saveResult = await SaveChangesAsync(
                document.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplyDocumentResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    document.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Ändrar document-status utan att införa en transition graph som modellen inte definierar.
        /// </summary>
        public async Task<Result<SupplyDocumentResponse>> ChangeStatusAsync(Guid supplyDocumentId, ChangeSupplyDocumentStatusRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplyDocumentResponse>(
                    validationResult.Errors);
            }

            var document = await _db.Set<SupplyDocument>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplyDocumentId,
                    cancellationToken);

            if (document is null)
            {
                return Result.Fail<SupplyDocumentResponse>(
                    new NotFoundError(
                        nameof(SupplyDocument),
                        supplyDocumentId));
            }

            if (document.Status == request.Status)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        document.Id,
                        cancellationToken));
            }

            _db.Entry(document)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            document.Status = request.Status;

            var saveResult = await SaveChangesAsync(
                document.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplyDocumentResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    document.Id,
                    cancellationToken));
        }


        /// <summary>
        /// Kontrollerar document type, subject-existens och gemensam supplier-kontext.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(CreateSupplyDocumentRequest request, CancellationToken cancellationToken)
        {
            var documentTypeExists = await _db.Set<DocumentType>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == request.DocumentTypeId,
                    cancellationToken);

            if (!documentTypeExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(DocumentType),
                        request.DocumentTypeId));
            }

            Guid? resolvedSupplierBusinessRelationshipId = null;

            if (request.SubjectBusinessRelationshipId.HasValue)
            {
                var relationshipExists = await _db.Set<BusinessRelationship>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == request.SubjectBusinessRelationshipId.Value,
                        cancellationToken);

                if (!relationshipExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(BusinessRelationship),
                            request.SubjectBusinessRelationshipId.Value));
                }

                resolvedSupplierBusinessRelationshipId = request.SubjectBusinessRelationshipId.Value;
            }

            if (request.SupplierMaterialOfferingId.HasValue)
            {
                var offering = await _db.Set<SupplierMaterialOffering>()
                    .AsNoTracking()
                    .Where(x => x.Id == request.SupplierMaterialOfferingId.Value)
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
                            request.SupplierMaterialOfferingId.Value));
                }

                var consistencyResult = ResolveSupplierContext(
                    resolvedSupplierBusinessRelationshipId,
                    offering.BusinessRelationshipId);

                if (consistencyResult.IsFailed)
                {
                    return consistencyResult;
                }

                resolvedSupplierBusinessRelationshipId = offering.BusinessRelationshipId;
            }

            if (request.SupplierProcessCapabilityId.HasValue)
            {
                var capability = await _db.Set<SupplierProcessCapability>()
                    .AsNoTracking()
                    .Where(x => x.Id == request.SupplierProcessCapabilityId.Value)
                    .Select(x => new
                    {
                        x.Id,
                        x.BusinessRelationshipId
                    })
                    .SingleOrDefaultAsync(cancellationToken);

                if (capability is null)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(SupplierProcessCapability),
                            request.SupplierProcessCapabilityId.Value));
                }

                var consistencyResult = ResolveSupplierContext(
                    resolvedSupplierBusinessRelationshipId,
                    capability.BusinessRelationshipId);

                if (consistencyResult.IsFailed)
                {
                    return consistencyResult;
                }

                resolvedSupplierBusinessRelationshipId = capability.BusinessRelationshipId;
            }

            if (request.InventoryLotId.HasValue)
            {
                var lot = await _db.Set<InventoryLot>()
                    .AsNoTracking()
                    .Where(x => x.Id == request.InventoryLotId.Value)
                    .Select(x => new
                    {
                        x.Id,
                        x.SupplierBusinessRelationshipId
                    })
                    .SingleOrDefaultAsync(cancellationToken);

                if (lot is null)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(InventoryLot),
                            request.InventoryLotId.Value));
                }

                var consistencyResult = ResolveSupplierContext(
                    resolvedSupplierBusinessRelationshipId,
                    lot.SupplierBusinessRelationshipId);

                if (consistencyResult.IsFailed)
                {
                    return consistencyResult;
                }

                resolvedSupplierBusinessRelationshipId = lot.SupplierBusinessRelationshipId;
            }

            if (request.ShipmentId.HasValue)
            {
                var shipmentExists = await _db.Set<Shipment>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == request.ShipmentId.Value,
                        cancellationToken);

                if (!shipmentExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(Shipment),
                            request.ShipmentId.Value));
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// Säkerställer att flera supplier-bundna subjects pekar på samma business relationship.
        /// </summary>
        private static Result ResolveSupplierContext(Guid? currentBusinessRelationshipId, Guid candidateBusinessRelationshipId)
        {
            if (!currentBusinessRelationshipId.HasValue
                || currentBusinessRelationshipId.Value == candidateBusinessRelationshipId)
            {
                return Result.Ok();
            }

            return Result.Fail(
                new ConflictError(
                    "All supplier-bound SupplyDocument subjects must belong to the same BusinessRelationship."));
        }

        /// <summary>
        /// Sparar document-ändringen och översätter stale RowVersion till concurrency-fel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid supplyDocumentId, CancellationToken cancellationToken)
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
                        nameof(SupplyDocument),
                        supplyDocumentId));
            }
        }

        /// <summary>
        /// Projekterar ett document direkt från databasen.
        /// </summary>
        private async Task<SupplyDocumentResponse?> ProjectAsync(Guid supplyDocumentId, CancellationToken cancellationToken)
        {
            return await _db.Set<SupplyDocument>()
                .AsNoTracking()
                .Where(x => x.Id == supplyDocumentId)
                .Select(SupplyDocumentProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar ett sparat document och kastar om det oväntat saknas.
        /// </summary>
        private async Task<SupplyDocumentResponse> ProjectRequiredAsync(Guid supplyDocumentId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    supplyDocumentId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"SupplyDocument '{supplyDocumentId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Lägger på stabil sortering före paginering.
        /// </summary>
        private static IQueryable<SupplyDocument> ApplySorting(IQueryable<SupplyDocument> query, SupplyDocumentSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (SupplyDocumentSortField.CreateDateUtc, SortDirection.Ascending) => query.OrderBy(x => x.CreateDateUtc).ThenBy(x => x.Id),
                (SupplyDocumentSortField.ValidFromUtc, SortDirection.Ascending) => query.OrderBy(x => x.ValidFromUtc).ThenBy(x => x.Id),
                (SupplyDocumentSortField.ValidFromUtc, SortDirection.Descending) => query.OrderByDescending(x => x.ValidFromUtc).ThenByDescending(x => x.Id),
                (SupplyDocumentSortField.ValidToUtc, SortDirection.Ascending) => query.OrderBy(x => x.ValidToUtc).ThenBy(x => x.Id),
                (SupplyDocumentSortField.ValidToUtc, SortDirection.Descending) => query.OrderByDescending(x => x.ValidToUtc).ThenByDescending(x => x.Id),
                (SupplyDocumentSortField.Status, SortDirection.Ascending) => query.OrderBy(x => x.Status).ThenBy(x => x.Id),
                (SupplyDocumentSortField.Status, SortDirection.Descending) => query.OrderByDescending(x => x.Status).ThenByDescending(x => x.Id),
                _ => query.OrderByDescending(x => x.CreateDateUtc).ThenByDescending(x => x.Id)
            };
        }
    }
}
