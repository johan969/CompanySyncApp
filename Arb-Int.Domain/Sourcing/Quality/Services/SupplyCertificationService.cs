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
    /// Hanterar certification assertions med immutable scheme, jurisdiction, subject och scope.
    /// </summary>
    public sealed class SupplyCertificationService : ISupplyCertificationService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplyCertificationService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar en supply certification.
        /// </summary>
        public async Task<Result<SupplyCertificationResponse>> GetByIdAsync(Guid supplyCertificationId, CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(supplyCertificationId, cancellationToken);

            return response is null
                ? Result.Fail<SupplyCertificationResponse>(new NotFoundError(nameof(SupplyCertification), supplyCertificationId))
                : Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar supply certifications.
        /// </summary>
        public async Task<Result<PagedResult<SupplyCertificationResponse>>> QueryAsync(SupplyCertificationQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<PagedResult<SupplyCertificationResponse>>(validationResult.Errors);

            IQueryable<SupplyCertification> query = _db.Set<SupplyCertification>().AsNoTracking();

            if (request.CertificationSchemeId.HasValue) query = query.Where(x => x.CertificationSchemeId == request.CertificationSchemeId.Value);
            if (request.CertificationSchemeJurisdictionId.HasValue) query = query.Where(x => x.CertificationSchemeJurisdictionId == request.CertificationSchemeJurisdictionId.Value);
            if (request.Scope.HasValue) query = query.Where(x => x.Scope == request.Scope.Value);
            if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
            if (request.CertifiedBusinessRelationshipId.HasValue) query = query.Where(x => x.CertifiedBusinessRelationshipId == request.CertifiedBusinessRelationshipId.Value);
            if (request.SupplierMaterialOfferingId.HasValue) query = query.Where(x => x.SupplierMaterialOfferingId == request.SupplierMaterialOfferingId.Value);
            if (request.SupplierProcessCapabilityId.HasValue) query = query.Where(x => x.SupplierProcessCapabilityId == request.SupplierProcessCapabilityId.Value);
            if (request.InventoryLotId.HasValue) query = query.Where(x => x.InventoryLotId == request.InventoryLotId.Value);
            if (request.ShipmentId.HasValue) query = query.Where(x => x.ShipmentId == request.ShipmentId.Value);
            if (request.SupplyDocumentId.HasValue) query = query.Where(x => x.SupplyDocumentId == request.SupplyDocumentId.Value);

            var totalCount = await query.CountAsync(cancellationToken);
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(SupplyCertificationProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok(new PagedResult<SupplyCertificationResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
        }

        /// <summary>
        /// Skapar en certification assertion efter scheme-, jurisdiction-, subject- och document-kontroll.
        /// </summary>
        public async Task<Result<SupplyCertificationResponse>> CreateAsync(CreateSupplyCertificationRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<SupplyCertificationResponse>(validationResult.Errors);

            var referencesResult = await ValidateReferencesAsync(request, cancellationToken);
            if (referencesResult.IsFailed) return Result.Fail<SupplyCertificationResponse>(referencesResult.Errors);

            var certification = new SupplyCertification
            {
                Id = Guid.CreateVersion7(),
                CertificationSchemeId = request.CertificationSchemeId,
                CertificationSchemeJurisdictionId = request.CertificationSchemeJurisdictionId,
                CertifiedBusinessRelationshipId = request.CertifiedBusinessRelationshipId,
                SupplierMaterialOfferingId = request.SupplierMaterialOfferingId,
                SupplierProcessCapabilityId = request.SupplierProcessCapabilityId,
                InventoryLotId = request.InventoryLotId,
                ShipmentId = request.ShipmentId,
                Scope = request.Scope,
                Status = CertificationStatus.Claimed,
                ValidFromUtc = request.ValidFromUtc,
                ValidToUtc = request.ValidToUtc,
                SupplyDocumentId = request.SupplyDocumentId
            };

            _db.Set<SupplyCertification>().Add(certification);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(await ProjectRequiredAsync(certification.Id, cancellationToken));
        }

        /// <summary>
        /// Sätter validity utan att ändra scheme, jurisdiction, subject eller scope.
        /// </summary>
        public async Task<Result<SupplyCertificationResponse>> SetValidityAsync(Guid supplyCertificationId, SetSupplyCertificationValidityRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<SupplyCertificationResponse>(validationResult.Errors);

            var certification = await FindAsync(supplyCertificationId, cancellationToken);
            if (certification is null) return Result.Fail<SupplyCertificationResponse>(new NotFoundError(nameof(SupplyCertification), supplyCertificationId));

            if (certification.ValidFromUtc == request.ValidFromUtc && certification.ValidToUtc == request.ValidToUtc)
            {
                return Result.Ok(await ProjectRequiredAsync(certification.Id, cancellationToken));
            }

            SetOriginalRowVersion(certification, request.RowVersion);
            certification.ValidFromUtc = request.ValidFromUtc;
            certification.ValidToUtc = request.ValidToUtc;

            var saveResult = await SaveChangesAsync(certification.Id, cancellationToken);
            return saveResult.IsFailed
                ? Result.Fail<SupplyCertificationResponse>(saveResult.Errors)
                : Result.Ok(await ProjectRequiredAsync(certification.Id, cancellationToken));
        }

        /// <summary>
        /// Ändrar status utan att införa en transition graph som modellen inte definierar.
        /// </summary>
        public async Task<Result<SupplyCertificationResponse>> ChangeStatusAsync(Guid supplyCertificationId, ChangeSupplyCertificationStatusRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<SupplyCertificationResponse>(validationResult.Errors);

            var certification = await FindAsync(supplyCertificationId, cancellationToken);
            if (certification is null) return Result.Fail<SupplyCertificationResponse>(new NotFoundError(nameof(SupplyCertification), supplyCertificationId));

            if (certification.Status == request.Status)
            {
                return Result.Ok(await ProjectRequiredAsync(certification.Id, cancellationToken));
            }

            SetOriginalRowVersion(certification, request.RowVersion);
            certification.Status = request.Status;

            var saveResult = await SaveChangesAsync(certification.Id, cancellationToken);
            return saveResult.IsFailed
                ? Result.Fail<SupplyCertificationResponse>(saveResult.Errors)
                : Result.Ok(await ProjectRequiredAsync(certification.Id, cancellationToken));
        }


        /// <summary>
        /// Sätter eller rensar supporting supply document efter existence-check.
        /// </summary>
        public async Task<Result<SupplyCertificationResponse>> SetDocumentAsync(Guid supplyCertificationId, SetSupplyCertificationDocumentRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<SupplyCertificationResponse>(validationResult.Errors);

            var certification = await FindAsync(supplyCertificationId, cancellationToken);
            if (certification is null) return Result.Fail<SupplyCertificationResponse>(new NotFoundError(nameof(SupplyCertification), supplyCertificationId));

            if (request.SupplyDocumentId.HasValue)
            {
                var documentExists = await _db.Set<SupplyDocument>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == request.SupplyDocumentId.Value, cancellationToken);

                if (!documentExists)
                {
                    return Result.Fail<SupplyCertificationResponse>(
                        new NotFoundError(nameof(SupplyDocument), request.SupplyDocumentId.Value));
                }
            }

            if (certification.SupplyDocumentId == request.SupplyDocumentId)
            {
                return Result.Ok(await ProjectRequiredAsync(certification.Id, cancellationToken));
            }

            SetOriginalRowVersion(certification, request.RowVersion);
            certification.SupplyDocumentId = request.SupplyDocumentId;

            var saveResult = await SaveChangesAsync(certification.Id, cancellationToken);
            return saveResult.IsFailed
                ? Result.Fail<SupplyCertificationResponse>(saveResult.Errors)
                : Result.Ok(await ProjectRequiredAsync(certification.Id, cancellationToken));
        }

        /// <summary>
        /// Kontrollerar scheme, jurisdiction, exakt subject och supporting document.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(CreateSupplyCertificationRequest request, CancellationToken cancellationToken)
        {
            var schemeExists = await _db.Set<CertificationScheme>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.CertificationSchemeId, cancellationToken);

            if (!schemeExists)
            {
                return Result.Fail(new NotFoundError(nameof(CertificationScheme), request.CertificationSchemeId));
            }

            if (request.CertificationSchemeJurisdictionId.HasValue)
            {
                var jurisdictionMatchesScheme = await _db.Set<CertificationSchemeJurisdiction>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == request.CertificationSchemeJurisdictionId.Value
                            && x.CertificationSchemeId == request.CertificationSchemeId,
                        cancellationToken);

                if (!jurisdictionMatchesScheme)
                {
                    return Result.Fail(
                        new ConflictError(
                            "CertificationSchemeJurisdiction must belong to the selected CertificationScheme."));
                }
            }

            if (request.CertifiedBusinessRelationshipId.HasValue
                && !await _db.Set<BusinessRelationship>().AsNoTracking().AnyAsync(x => x.Id == request.CertifiedBusinessRelationshipId.Value, cancellationToken))
            {
                return Result.Fail(new NotFoundError(nameof(BusinessRelationship), request.CertifiedBusinessRelationshipId.Value));
            }

            if (request.SupplierMaterialOfferingId.HasValue
                && !await _db.Set<SupplierMaterialOffering>().AsNoTracking().AnyAsync(x => x.Id == request.SupplierMaterialOfferingId.Value, cancellationToken))
            {
                return Result.Fail(new NotFoundError(nameof(SupplierMaterialOffering), request.SupplierMaterialOfferingId.Value));
            }

            if (request.SupplierProcessCapabilityId.HasValue
                && !await _db.Set<SupplierProcessCapability>().AsNoTracking().AnyAsync(x => x.Id == request.SupplierProcessCapabilityId.Value, cancellationToken))
            {
                return Result.Fail(new NotFoundError(nameof(SupplierProcessCapability), request.SupplierProcessCapabilityId.Value));
            }

            if (request.InventoryLotId.HasValue
                && !await _db.Set<InventoryLot>().AsNoTracking().AnyAsync(x => x.Id == request.InventoryLotId.Value, cancellationToken))
            {
                return Result.Fail(new NotFoundError(nameof(InventoryLot), request.InventoryLotId.Value));
            }

            if (request.ShipmentId.HasValue
                && !await _db.Set<Shipment>().AsNoTracking().AnyAsync(x => x.Id == request.ShipmentId.Value, cancellationToken))
            {
                return Result.Fail(new NotFoundError(nameof(Shipment), request.ShipmentId.Value));
            }

            if (request.SupplyDocumentId.HasValue
                && !await _db.Set<SupplyDocument>().AsNoTracking().AnyAsync(x => x.Id == request.SupplyDocumentId.Value, cancellationToken))
            {
                return Result.Fail(new NotFoundError(nameof(SupplyDocument), request.SupplyDocumentId.Value));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Hämtar certification-rooten trackad för mutation.
        /// </summary>
        private Task<SupplyCertification?> FindAsync(Guid supplyCertificationId, CancellationToken cancellationToken)
        {
            return _db.Set<SupplyCertification>()
                .SingleOrDefaultAsync(x => x.Id == supplyCertificationId, cancellationToken);
        }

        /// <summary>
        /// Sätter original RowVersion för optimistic concurrency.
        /// </summary>
        private void SetOriginalRowVersion(SupplyCertification certification, uint rowVersion)
        {
            _db.Entry(certification)
                .Property(x => x.RowVersion)
                .OriginalValue = rowVersion;
        }

        /// <summary>
        /// Sparar certification-ändringen och översätter stale RowVersion till concurrency-fel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid supplyCertificationId, CancellationToken cancellationToken)
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
                        nameof(SupplyCertification),
                        supplyCertificationId));
            }
        }

        /// <summary>
        /// Projekterar en supply certification direkt från databasen.
        /// </summary>
        private async Task<SupplyCertificationResponse?> ProjectAsync(Guid supplyCertificationId, CancellationToken cancellationToken)
        {
            return await _db.Set<SupplyCertification>()
                .AsNoTracking()
                .Where(x => x.Id == supplyCertificationId)
                .Select(SupplyCertificationProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad certification och kastar om den oväntat saknas.
        /// </summary>
        private async Task<SupplyCertificationResponse> ProjectRequiredAsync(Guid supplyCertificationId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(supplyCertificationId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"SupplyCertification '{supplyCertificationId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Lägger på stabil sortering före paginering.
        /// </summary>
        private static IQueryable<SupplyCertification> ApplySorting(IQueryable<SupplyCertification> query, SupplyCertificationSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (SupplyCertificationSortField.CreateDateUtc, SortDirection.Ascending) => query.OrderBy(x => x.CreateDateUtc).ThenBy(x => x.Id),
                (SupplyCertificationSortField.ValidFromUtc, SortDirection.Ascending) => query.OrderBy(x => x.ValidFromUtc).ThenBy(x => x.Id),
                (SupplyCertificationSortField.ValidFromUtc, SortDirection.Descending) => query.OrderByDescending(x => x.ValidFromUtc).ThenByDescending(x => x.Id),
                (SupplyCertificationSortField.ValidToUtc, SortDirection.Ascending) => query.OrderBy(x => x.ValidToUtc).ThenBy(x => x.Id),
                (SupplyCertificationSortField.ValidToUtc, SortDirection.Descending) => query.OrderByDescending(x => x.ValidToUtc).ThenByDescending(x => x.Id),
                (SupplyCertificationSortField.Status, SortDirection.Ascending) => query.OrderBy(x => x.Status).ThenBy(x => x.Id),
                (SupplyCertificationSortField.Status, SortDirection.Descending) => query.OrderByDescending(x => x.Status).ThenByDescending(x => x.Id),
                (SupplyCertificationSortField.Scope, SortDirection.Ascending) => query.OrderBy(x => x.Scope).ThenBy(x => x.Id),
                (SupplyCertificationSortField.Scope, SortDirection.Descending) => query.OrderByDescending(x => x.Scope).ThenByDescending(x => x.Id),
                _ => query.OrderByDescending(x => x.CreateDateUtc).ThenByDescending(x => x.Id)
            };
        }
    }
}
