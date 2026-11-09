using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar supply analysis-roten med immutable subjects och explicit status och document-koppling.
    /// </summary>
    public sealed class SupplyAnalysisService : ISupplyAnalysisService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplyAnalysisService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar en supply analysis med resultat.
        /// </summary>
        public async Task<Result<SupplyAnalysisResponse>> GetByIdAsync(Guid supplyAnalysisId, CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(
                supplyAnalysisId,
                cancellationToken);

            if (response is null)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    new NotFoundError(
                        nameof(SupplyAnalysis),
                        supplyAnalysisId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar supply analyses.
        /// </summary>
        public async Task<Result<PagedResult<SupplyAnalysisListResponse>>> QueryAsync(SupplyAnalysisQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<SupplyAnalysisListResponse>>(
                    validationResult.Errors);
            }

            IQueryable<SupplyAnalysis> query = _db.Set<SupplyAnalysis>()
                .AsNoTracking();

            if (request.SupplierMaterialOfferingId.HasValue)
            {
                query = query.Where(
                    x => x.SupplierMaterialOfferingId == request.SupplierMaterialOfferingId.Value);
            }

            if (request.InventoryLotId.HasValue)
            {
                query = query.Where(
                    x => x.InventoryLotId == request.InventoryLotId.Value);
            }

            if (request.SupplyDocumentId.HasValue)
            {
                query = query.Where(
                    x => x.SupplyDocumentId == request.SupplyDocumentId.Value);
            }

            if (request.LaboratoryBusinessRelationshipId.HasValue)
            {
                query = query.Where(
                    x => x.LaboratoryBusinessRelationshipId == request.LaboratoryBusinessRelationshipId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(
                    x => x.Status == request.Status.Value);
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
                .Select(SupplyAnalysisProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<SupplyAnalysisListResponse>
                {
                    Items = items,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                });
        }

        /// <summary>
        /// Skapar en draft analysis och validerar subject-referensernas inbördes konsistens.
        /// </summary>
        public async Task<Result<SupplyAnalysisResponse>> CreateAsync(CreateSupplyAnalysisRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    validationResult.Errors);
            }

            var referencesResult = await ValidateReferencesAsync(
                request,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    referencesResult.Errors);
            }

            var analysis = new SupplyAnalysis
            {
                Id = Guid.CreateVersion7(),
                SupplierMaterialOfferingId = request.SupplierMaterialOfferingId,
                InventoryLotId = request.InventoryLotId,
                SupplyDocumentId = request.SupplyDocumentId,
                LaboratoryBusinessRelationshipId = request.LaboratoryBusinessRelationshipId,
                Status = AnalysisStatus.Draft
            };

            _db.Set<SupplyAnalysis>()
                .Add(analysis);

            await _db.SaveChangesAsync(
                cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    analysis.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Sätter eller rensar supporting document utan att ändra analysens offering- eller lot-subject.
        /// </summary>
        public async Task<Result<SupplyAnalysisResponse>> SetDocumentAsync(Guid supplyAnalysisId, SetSupplyAnalysisDocumentRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    validationResult.Errors);
            }

            var analysis = await FindAsync(
                supplyAnalysisId,
                cancellationToken);

            if (analysis is null)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    new NotFoundError(
                        nameof(SupplyAnalysis),
                        supplyAnalysisId));
            }

            if (request.SupplyDocumentId.HasValue)
            {
                var documentExists = await _db.Set<SupplyDocument>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == request.SupplyDocumentId.Value,
                        cancellationToken);

                if (!documentExists)
                {
                    return Result.Fail<SupplyAnalysisResponse>(
                        new NotFoundError(
                            nameof(SupplyDocument),
                            request.SupplyDocumentId.Value));
                }
            }

            if (analysis.SupplyDocumentId == request.SupplyDocumentId)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        analysis.Id,
                        cancellationToken));
            }

            SetOriginalRowVersion(
                analysis,
                request.RowVersion);

            analysis.SupplyDocumentId = request.SupplyDocumentId;

            var saveResult = await SaveChangesAsync(
                analysis.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    analysis.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Ändrar analysis status utan att införa en transition graph som modellen inte definierar.
        /// </summary>
        public async Task<Result<SupplyAnalysisResponse>> ChangeStatusAsync(Guid supplyAnalysisId, ChangeSupplyAnalysisStatusRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    validationResult.Errors);
            }

            var analysis = await FindAsync(
                supplyAnalysisId,
                cancellationToken);

            if (analysis is null)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    new NotFoundError(
                        nameof(SupplyAnalysis),
                        supplyAnalysisId));
            }

            if (analysis.Status == request.Status)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        analysis.Id,
                        cancellationToken));
            }

            SetOriginalRowVersion(
                analysis,
                request.RowVersion);

            analysis.Status = request.Status;

            var saveResult = await SaveChangesAsync(
                analysis.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    analysis.Id,
                    cancellationToken));
        }


        /// <summary>
        /// Kontrollerar offering, inventory lot, supporting document och laboratory relationship.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(CreateSupplyAnalysisRequest request, CancellationToken cancellationToken)
        {
            SupplierMaterialOffering? offering = null;
            InventoryLot? lot = null;

            if (request.SupplierMaterialOfferingId.HasValue)
            {
                offering = await _db.Set<SupplierMaterialOffering>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        x => x.Id == request.SupplierMaterialOfferingId.Value,
                        cancellationToken);

                if (offering is null)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(SupplierMaterialOffering),
                            request.SupplierMaterialOfferingId.Value));
                }
            }

            if (request.InventoryLotId.HasValue)
            {
                lot = await _db.Set<InventoryLot>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        x => x.Id == request.InventoryLotId.Value,
                        cancellationToken);

                if (lot is null)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(InventoryLot),
                            request.InventoryLotId.Value));
                }
            }

            if (offering is not null
                && lot is not null
                && lot.SupplierMaterialOfferingId != offering.Id)
            {
                return Result.Fail(
                    new ConflictError(
                        "When both SupplierMaterialOfferingId and InventoryLotId are supplied, the InventoryLot must belong to that SupplierMaterialOffering."));
            }

            if (request.SupplyDocumentId.HasValue)
            {
                var documentExists = await _db.Set<SupplyDocument>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == request.SupplyDocumentId.Value,
                        cancellationToken);

                if (!documentExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(SupplyDocument),
                            request.SupplyDocumentId.Value));
                }
            }

            if (request.LaboratoryBusinessRelationshipId.HasValue)
            {
                var laboratoryRelationshipExists = await _db.Set<BusinessRelationship>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == request.LaboratoryBusinessRelationshipId.Value,
                        cancellationToken);

                if (!laboratoryRelationshipExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(BusinessRelationship),
                            request.LaboratoryBusinessRelationshipId.Value));
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// Hämtar analysis-rooten trackad för mutation.
        /// </summary>
        private Task<SupplyAnalysis?> FindAsync(Guid supplyAnalysisId, CancellationToken cancellationToken)
        {
            return _db.Set<SupplyAnalysis>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplyAnalysisId,
                    cancellationToken);
        }

        /// <summary>
        /// Sätter original RowVersion för optimistic concurrency.
        /// </summary>
        private void SetOriginalRowVersion(SupplyAnalysis analysis, uint rowVersion)
        {
            _db.Entry(analysis)
                .Property(x => x.RowVersion)
                .OriginalValue = rowVersion;
        }

        /// <summary>
        /// Sparar analysis-ändringen och översätter stale RowVersion till concurrency-fel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid supplyAnalysisId, CancellationToken cancellationToken)
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
                        nameof(SupplyAnalysis),
                        supplyAnalysisId));
            }
        }

        /// <summary>
        /// Projekterar en supply analysis med resultat.
        /// </summary>
        private async Task<SupplyAnalysisResponse?> ProjectAsync(Guid supplyAnalysisId, CancellationToken cancellationToken)
        {
            return await _db.Set<SupplyAnalysis>()
                .AsNoTracking()
                .Where(x => x.Id == supplyAnalysisId)
                .Select(SupplyAnalysisProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad analysis och kastar om den oväntat saknas.
        /// </summary>
        private async Task<SupplyAnalysisResponse> ProjectRequiredAsync(Guid supplyAnalysisId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    supplyAnalysisId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"SupplyAnalysis '{supplyAnalysisId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Lägger på stabil sortering före paginering.
        /// </summary>
        private static IQueryable<SupplyAnalysis> ApplySorting(IQueryable<SupplyAnalysis> query, SupplyAnalysisSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (SupplyAnalysisSortField.CreateDateUtc, SortDirection.Ascending) => query.OrderBy(x => x.CreateDateUtc).ThenBy(x => x.Id),
                (SupplyAnalysisSortField.Status, SortDirection.Ascending) => query.OrderBy(x => x.Status).ThenBy(x => x.Id),
                (SupplyAnalysisSortField.Status, SortDirection.Descending) => query.OrderByDescending(x => x.Status).ThenByDescending(x => x.Id),
                _ => query.OrderByDescending(x => x.CreateDateUtc).ThenByDescending(x => x.Id)
            };
        }
    }
}
