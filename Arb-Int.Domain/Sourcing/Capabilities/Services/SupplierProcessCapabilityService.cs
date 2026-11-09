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
    /// Hanterar supplier process capability-roten med explicit mapping, status och verifiering.
    /// </summary>
    public sealed class SupplierProcessCapabilityService : ISupplierProcessCapabilityService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;
        private readonly ISupplierProcessCapabilityNormalizer _normalizer;

        /// <summary>
        /// Skapar capability-servicen med databas-, validerings- och normaliseringsberoenden.
        /// </summary>
        public SupplierProcessCapabilityService(Arb_IntDbContext db, IValidationService validationService, ISupplierProcessCapabilityNormalizer normalizer)
        {
            _db = db;
            _validationService = validationService;
            _normalizer = normalizer;
        }

        /// <summary>
        /// Hämtar en capability med capability-ägda scopes och evidens.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> GetByIdAsync(Guid supplierProcessCapabilityId, CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(supplierProcessCapabilityId, cancellationToken);
            return response is null
                ? Result.Fail<SupplierProcessCapabilityResponse>(new NotFoundError(nameof(SupplierProcessCapability), supplierProcessCapabilityId))
                : Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar capabilities för en supplier-relation.
        /// </summary>
        public async Task<Result<PagedResult<SupplierProcessCapabilityListResponse>>> QueryAsync(Guid businessRelationshipId, SupplierProcessCapabilityQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<PagedResult<SupplierProcessCapabilityListResponse>>(validationResult.Errors);

            var relationshipResult = await ValidateSupplierRelationshipAsync(businessRelationshipId, cancellationToken);
            if (relationshipResult.IsFailed) return Result.Fail<PagedResult<SupplierProcessCapabilityListResponse>>(relationshipResult.Errors);

            IQueryable<SupplierProcessCapability> query = _db.Set<SupplierProcessCapability>().AsNoTracking().Where(x => x.BusinessRelationshipId == businessRelationshipId);

            query = ApplyFilters(query, request);

            var totalCount = await query.CountAsync(cancellationToken);
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(SupplierProcessCapabilityProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(new PagedResult<SupplierProcessCapabilityListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
        }

        /// <summary>
        /// Skapar en observerad, omappad och ännu overifierad capability med immutable capability key.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> CreateAsync(Guid businessRelationshipId, CreateSupplierProcessCapabilityRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<SupplierProcessCapabilityResponse>(validationResult.Errors);

            var relationshipResult = await ValidateSupplierRelationshipAsync(businessRelationshipId, cancellationToken);
            if (relationshipResult.IsFailed) return Result.Fail<SupplierProcessCapabilityResponse>(relationshipResult.Errors);

            var capabilityKey = request.CapabilityKey.Trim();
            var duplicateExists = await _db.Set<SupplierProcessCapability>()
                .AsNoTracking()
                .AnyAsync(x => x.BusinessRelationshipId == businessRelationshipId && x.CapabilityKey == capabilityKey, cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConflictError($"SupplierProcessCapability key '{capabilityKey}' already exists within this supplier relationship."));
            }

            var now = DateTimeOffset.UtcNow;
            var firstObservedUtc = request.FirstObservedUtc ?? request.LastObservedUtc ?? now;
            var lastObservedUtc = request.LastObservedUtc ?? firstObservedUtc;

            if (lastObservedUtc < firstObservedUtc)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConflictError("LastObservedUtc cannot be earlier than FirstObservedUtc."));
            }

            var declaredProcessName = request.DeclaredProcessName.Trim();

            var capability = new SupplierProcessCapability
            {
                Id = Guid.CreateVersion7(),
                BusinessRelationshipId = businessRelationshipId,
                CapabilityKey = capabilityKey,
                Name = request.Name.Trim(),
                DeclaredProcessName = declaredProcessName,
                NormalizedDeclaredProcessName = _normalizer.NormalizeDeclaredProcessName(declaredProcessName),
                ProcessTypeId = null,
                InputMaterialTypeId = null,
                OutputMaterialTypeId = null,
                Description = request.Description.Trim(),
                Notes = TrimToNull(request.Notes),
                MappingStatus = SourcingMappingStatus.Unmapped,
                MappingConfidence = null,
                Status = SupplierProcessCapabilityStatus.Observed,
                VerificationStatus = SourcingVerificationStatus.Unverified,
                FirstObservedUtc = firstObservedUtc,
                LastObservedUtc = lastObservedUtc,
                LastVerifiedUtc = null
            };

            _db.Set<SupplierProcessCapability>().Add(capability);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueViolation(exception))
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConflictError("Another SupplierProcessCapability already uses the same capability key within this supplier relationship."));
            }

            return Result.Ok(await ProjectRequiredAsync(capability.Id, cancellationToken));
        }

        /// <summary>
        /// Uppdaterar deklarerad metadata utan att ändra capability key, supplier-relation, mapping, status eller verifiering.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> UpdateAsync(Guid supplierProcessCapabilityId, UpdateSupplierProcessCapabilityRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<SupplierProcessCapabilityResponse>(validationResult.Errors);

            var capability = await _db.Set<SupplierProcessCapability>().SingleOrDefaultAsync(x => x.Id == supplierProcessCapabilityId, cancellationToken);
            if (capability is null) return Result.Fail<SupplierProcessCapabilityResponse>(new NotFoundError(nameof(SupplierProcessCapability), supplierProcessCapabilityId));

            if (request.LastObservedUtc < request.FirstObservedUtc)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConflictError("LastObservedUtc cannot be earlier than FirstObservedUtc."));
            }

            if (capability.LastVerifiedUtc.HasValue && capability.LastVerifiedUtc.Value < request.FirstObservedUtc)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConflictError("FirstObservedUtc cannot be moved after the recorded LastVerifiedUtc."));
            }

            _db.Entry(capability).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

            var declaredProcessName = request.DeclaredProcessName.Trim();
            capability.Name = request.Name.Trim();
            capability.DeclaredProcessName = declaredProcessName;
            capability.NormalizedDeclaredProcessName = _normalizer.NormalizeDeclaredProcessName(declaredProcessName);
            capability.Description = request.Description.Trim();
            capability.Notes = TrimToNull(request.Notes);
            capability.FirstObservedUtc = request.FirstObservedUtc;
            capability.LastObservedUtc = request.LastObservedUtc;

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConcurrencyError(nameof(SupplierProcessCapability), capability.Id));
            }

            return Result.Ok(await ProjectRequiredAsync(capability.Id, cancellationToken));
        }

        /// <summary>
        /// Sätter process- och materialtypmapping efter kontroll av referenserna.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> SetMappingAsync(Guid supplierProcessCapabilityId, SetSupplierProcessCapabilityMappingRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<SupplierProcessCapabilityResponse>(validationResult.Errors);

            var capability = await _db.Set<SupplierProcessCapability>().SingleOrDefaultAsync(x => x.Id == supplierProcessCapabilityId, cancellationToken);
            if (capability is null) return Result.Fail<SupplierProcessCapabilityResponse>(new NotFoundError(nameof(SupplierProcessCapability), supplierProcessCapabilityId));

            var referencesResult = await ValidateMappingReferencesAsync(request.ProcessTypeId, request.InputMaterialTypeId, request.OutputMaterialTypeId, cancellationToken);
            if (referencesResult.IsFailed) return Result.Fail<SupplierProcessCapabilityResponse>(referencesResult.Errors);

            if (request.MappingStatus == SourcingMappingStatus.Confirmed && !request.ProcessTypeId.HasValue)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConflictError("Confirmed process mapping requires ProcessTypeId."));
            }

            _db.Entry(capability).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

            capability.ProcessTypeId = request.ProcessTypeId;
            capability.InputMaterialTypeId = request.InputMaterialTypeId;
            capability.OutputMaterialTypeId = request.OutputMaterialTypeId;
            capability.MappingStatus = request.MappingStatus;
            capability.MappingConfidence = request.MappingConfidence;

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConcurrencyError(nameof(SupplierProcessCapability), capability.Id));
            }

            return Result.Ok(await ProjectRequiredAsync(capability.Id, cancellationToken));
        }

        /// <summary>
        /// Ändrar sourcing-status utan att införa en påhittad transition graph.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> ChangeStatusAsync(Guid supplierProcessCapabilityId, ChangeSupplierProcessCapabilityStatusRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<SupplierProcessCapabilityResponse>(validationResult.Errors);

            var capability = await _db.Set<SupplierProcessCapability>().SingleOrDefaultAsync(x => x.Id == supplierProcessCapabilityId, cancellationToken);
            if (capability is null) return Result.Fail<SupplierProcessCapabilityResponse>(new NotFoundError(nameof(SupplierProcessCapability), supplierProcessCapabilityId));

            _db.Entry(capability).Property(x => x.RowVersion).OriginalValue = request.RowVersion;
            capability.Status = request.Status;

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConcurrencyError(nameof(SupplierProcessCapability), capability.Id));
            }

            return Result.Ok(await ProjectRequiredAsync(capability.Id, cancellationToken));
        }

        /// <summary>
        /// Sätter verification-status och uppdaterar LastVerifiedUtc endast när en ny verifieringstid skickas in.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> SetVerificationAsync(Guid supplierProcessCapabilityId, SetSupplierProcessCapabilityVerificationRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<SupplierProcessCapabilityResponse>(validationResult.Errors);

            var capability = await _db.Set<SupplierProcessCapability>().SingleOrDefaultAsync(x => x.Id == supplierProcessCapabilityId, cancellationToken);
            if (capability is null) return Result.Fail<SupplierProcessCapabilityResponse>(new NotFoundError(nameof(SupplierProcessCapability), supplierProcessCapabilityId));

            if (request.VerifiedAtUtc.HasValue && request.VerifiedAtUtc.Value < capability.FirstObservedUtc)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConflictError("VerifiedAtUtc cannot be earlier than FirstObservedUtc."));
            }

            _db.Entry(capability).Property(x => x.RowVersion).OriginalValue = request.RowVersion;
            capability.VerificationStatus = request.VerificationStatus;

            if (request.VerifiedAtUtc.HasValue)
            {
                capability.LastVerifiedUtc = request.VerifiedAtUtc.Value;
            }

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConcurrencyError(nameof(SupplierProcessCapability), capability.Id));
            }

            return Result.Ok(await ProjectRequiredAsync(capability.Id, cancellationToken));
        }

        /// <summary>
        /// Kontrollerar att relationen finns och är av supplier-typ.
        /// </summary>
        private async Task<Result> ValidateSupplierRelationshipAsync(Guid businessRelationshipId, CancellationToken cancellationToken)
        {
            var relationship = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .Where(x => x.Id == businessRelationshipId)
                .Select(x => new { x.Id, RelationshipTypeCode = x.BusinessRelationshipType.Code })
                .SingleOrDefaultAsync(cancellationToken);

            if (relationship is null)
            {
                return Result.Fail(new NotFoundError(nameof(BusinessRelationship), businessRelationshipId));
            }

            return relationship.RelationshipTypeCode == BusinessRelationshipTypeCodes.Supplier
                ? Result.Ok()
                : Result.Fail(new ConflictError("SupplierProcessCapability can only belong to a supplier business relationship."));
        }

        /// <summary>
        /// Kontrollerar att process- och materialtyper som används av mappingen finns.
        /// </summary>
        private async Task<Result> ValidateMappingReferencesAsync(int? processTypeId, int? inputMaterialTypeId, int? outputMaterialTypeId, CancellationToken cancellationToken)
        {
            if (processTypeId.HasValue && !await _db.Set<ProcessType>().AsNoTracking().AnyAsync(x => x.Id == processTypeId.Value, cancellationToken))
            {
                return Result.Fail(new NotFoundError(nameof(ProcessType), processTypeId.Value));
            }

            if (inputMaterialTypeId.HasValue && !await _db.Set<MaterialType>().AsNoTracking().AnyAsync(x => x.Id == inputMaterialTypeId.Value, cancellationToken))
            {
                return Result.Fail(new NotFoundError(nameof(MaterialType), inputMaterialTypeId.Value));
            }

            if (outputMaterialTypeId.HasValue && !await _db.Set<MaterialType>().AsNoTracking().AnyAsync(x => x.Id == outputMaterialTypeId.Value, cancellationToken))
            {
                return Result.Fail(new NotFoundError(nameof(MaterialType), outputMaterialTypeId.Value));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Projekterar en capability direkt från databasen.
        /// </summary>
        private async Task<SupplierProcessCapabilityResponse?> ProjectAsync(Guid supplierProcessCapabilityId, CancellationToken cancellationToken)
        {
            return await _db.Set<SupplierProcessCapability>()
                .AsNoTracking()
                .Where(x => x.Id == supplierProcessCapabilityId)
                .Select(SupplierProcessCapabilityProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad capability och kastar om den oväntat saknas.
        /// </summary>
        private async Task<SupplierProcessCapabilityResponse> ProjectRequiredAsync(Guid supplierProcessCapabilityId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(supplierProcessCapabilityId, cancellationToken)
                ?? throw new InvalidOperationException($"SupplierProcessCapability '{supplierProcessCapabilityId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Lägger på stabil sortering före paginering.
        /// </summary>
        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar capabilities över samtliga supplier-relationer
        /// som ägs av organisationen.
        /// </summary>
        public async Task<Result<PagedResult<SupplierProcessCapabilityListResponse>>> QueryByOrganizationAsync(Guid owningOrganizationId, SupplierProcessCapabilityQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailed) return Result.Fail<PagedResult<SupplierProcessCapabilityListResponse>>(validationResult.Errors);

            IQueryable<SupplierProcessCapability> query = _db.Set<SupplierProcessCapability>().AsNoTracking()
                .Where(x =>
                    x.BusinessRelationship.OwningOrganizationId == owningOrganizationId
                    && x.BusinessRelationship.BusinessRelationshipType.Code == BusinessRelationshipTypeCodes.Supplier);

            if (request.BusinessRelationshipId.HasValue) query = query.Where(x => x.BusinessRelationshipId == request.BusinessRelationshipId.Value);

            query = ApplyFilters(query, request);

            var totalCount = await query.CountAsync(cancellationToken);
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(SupplierProcessCapabilityProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(new PagedResult<SupplierProcessCapabilityListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
        }

        /// <summary>
        /// Applicerar de filter som är gemensamma för relations- och organisationsscopade sökningar.
        /// </summary>
        private static IQueryable<SupplierProcessCapability> ApplyFilters(IQueryable<SupplierProcessCapability> query, SupplierProcessCapabilityQueryRequest request)
        {
            if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
            if (request.MappingStatus.HasValue) query = query.Where(x => x.MappingStatus == request.MappingStatus.Value);
            if (request.VerificationStatus.HasValue) query = query.Where(x => x.VerificationStatus == request.VerificationStatus.Value);
            if (request.ProcessTypeId.HasValue) query = query.Where(x => x.ProcessTypeId == request.ProcessTypeId.Value);
            if (request.InputMaterialTypeId.HasValue) query = query.Where(x => x.InputMaterialTypeId == request.InputMaterialTypeId.Value);
            if (request.OutputMaterialTypeId.HasValue) query = query.Where(x => x.OutputMaterialTypeId == request.OutputMaterialTypeId.Value);

            if (request.BiologicalSourceId.HasValue)
            {
                var biologicalSourceId = request.BiologicalSourceId.Value;
                query = query.Where(x => x.BiologicalSourceScopes.Any(scope => scope.BiologicalSourceClaim.BiologicalSourceId == biologicalSourceId));
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var pattern = $"%{request.Search.Trim()}%";
                query = query.Where(x =>
                    EF.Functions.ILike(x.CapabilityKey, pattern)
                    || EF.Functions.ILike(x.Name, pattern)
                    || EF.Functions.ILike(x.DeclaredProcessName, pattern)
                    || (x.Notes != null && EF.Functions.ILike(x.Notes, pattern))
                    || (x.ProcessType != null && EF.Functions.ILike(x.ProcessType.Name, pattern))
                    || (x.InputMaterialType != null && EF.Functions.ILike(x.InputMaterialType.Name, pattern))
                    || (x.OutputMaterialType != null && EF.Functions.ILike(x.OutputMaterialType.Name, pattern))
                    || x.BiologicalSourceScopes.Any(scope =>
                        EF.Functions.ILike(scope.BiologicalSourceClaim.DeclaredName, pattern)
                        || (scope.BiologicalSourceClaim.BiologicalSource != null
                            && EF.Functions.ILike(scope.BiologicalSourceClaim.BiologicalSource.Name, pattern))));
            }

            return query;
        }

        private static IQueryable<SupplierProcessCapability> ApplySorting(IQueryable<SupplierProcessCapability> query, SupplierProcessCapabilitySortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (SupplierProcessCapabilitySortField.Name, SortDirection.Descending) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (SupplierProcessCapabilitySortField.Status, SortDirection.Ascending) => query.OrderBy(x => x.Status).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (SupplierProcessCapabilitySortField.Status, SortDirection.Descending) => query.OrderByDescending(x => x.Status).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (SupplierProcessCapabilitySortField.MappingStatus, SortDirection.Ascending) => query.OrderBy(x => x.MappingStatus).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (SupplierProcessCapabilitySortField.MappingStatus, SortDirection.Descending) => query.OrderByDescending(x => x.MappingStatus).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (SupplierProcessCapabilitySortField.VerificationStatus, SortDirection.Ascending) => query.OrderBy(x => x.VerificationStatus).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (SupplierProcessCapabilitySortField.VerificationStatus, SortDirection.Descending) => query.OrderByDescending(x => x.VerificationStatus).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (SupplierProcessCapabilitySortField.FirstObservedUtc, SortDirection.Ascending) => query.OrderBy(x => x.FirstObservedUtc).ThenBy(x => x.Id),
                (SupplierProcessCapabilitySortField.FirstObservedUtc, SortDirection.Descending) => query.OrderByDescending(x => x.FirstObservedUtc).ThenByDescending(x => x.Id),
                (SupplierProcessCapabilitySortField.LastObservedUtc, SortDirection.Ascending) => query.OrderBy(x => x.LastObservedUtc).ThenBy(x => x.Id),
                (SupplierProcessCapabilitySortField.LastObservedUtc, SortDirection.Descending) => query.OrderByDescending(x => x.LastObservedUtc).ThenByDescending(x => x.Id),
                (SupplierProcessCapabilitySortField.LastVerifiedUtc, SortDirection.Ascending) => query.OrderBy(x => x.LastVerifiedUtc).ThenBy(x => x.Id),
                (SupplierProcessCapabilitySortField.LastVerifiedUtc, SortDirection.Descending) => query.OrderByDescending(x => x.LastVerifiedUtc).ThenByDescending(x => x.Id),
                (SupplierProcessCapabilitySortField.CreateDateUtc, SortDirection.Ascending) => query.OrderBy(x => x.CreateDateUtc).ThenBy(x => x.Id),
                (SupplierProcessCapabilitySortField.CreateDateUtc, SortDirection.Descending) => query.OrderByDescending(x => x.CreateDateUtc).ThenByDescending(x => x.Id),
                _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
            };
        }

        /// <summary>
        /// Kontrollerar om databasfelet kommer från en unik constraint.
        /// </summary>
        private static bool IsUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
        }

        /// <summary>
        /// Trimmar text och ersätter tomma värden med null.
        /// </summary>
        private static string? TrimToNull(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
