using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Hanterar supplier material offering-roten med explicit mapping och statusändring.
/// </summary>
public sealed class SupplierMaterialOfferingService : ISupplierMaterialOfferingService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ISupplierMaterialOfferingNormalizer _normalizer;

    /// <summary>
    /// Skapar offering-servicen med databas-, validerings- och normaliseringsberoenden.
    /// </summary>
    public SupplierMaterialOfferingService(
        Arb_IntDbContext db,
        IValidationService validationService,
        ISupplierMaterialOfferingNormalizer normalizer)
    {
        _db = db;
        _validationService = validationService;
        _normalizer = normalizer;
    }

    /// <summary>
    /// Hämtar ett offering med offering-ägda child-data.
    /// </summary>
    public async Task<Result<SupplierMaterialOfferingResponse>> GetByIdAsync(
        Guid supplierMaterialOfferingId,
        CancellationToken cancellationToken = default)
    {
        var response = await ProjectAsync(
            supplierMaterialOfferingId,
            cancellationToken);

        if (response is null)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                new NotFoundError(
                    nameof(SupplierMaterialOffering),
                    supplierMaterialOfferingId));
        }

        return Result.Ok(response);
    }

    /// <summary>
    /// Söker, filtrerar, sorterar och paginerar offerings för en supplier-relation.
    /// </summary>
    public async Task<Result<PagedResult<SupplierMaterialOfferingListResponse>>> QueryAsync(
        Guid businessRelationshipId,
        SupplierMaterialOfferingQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<PagedResult<SupplierMaterialOfferingListResponse>>(
                validationResult.Errors);
        }

        var relationshipResult = await ValidateSupplierRelationshipAsync(
            businessRelationshipId,
            cancellationToken);

        if (relationshipResult.IsFailed)
        {
            return Result.Fail<PagedResult<SupplierMaterialOfferingListResponse>>(
                relationshipResult.Errors);
        }

        IQueryable<SupplierMaterialOffering> query = _db
            .Set<SupplierMaterialOffering>()
            .AsNoTracking()
            .Where(x => x.BusinessRelationshipId == businessRelationshipId);

        query = ApplyFilters(query, request);

        var totalCount = await query.CountAsync(
            cancellationToken);

        query = ApplySorting(
            query,
            request.SortBy,
            request.SortDirection);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(SupplierMaterialOfferingProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok(
            new PagedResult<SupplierMaterialOfferingListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }

    /// <summary>
    /// Skapar ett observerat offering med immutable supplier-identitet och offering key.
    /// </summary>
    public async Task<Result<SupplierMaterialOfferingResponse>> CreateAsync(
        Guid businessRelationshipId,
        CreateSupplierMaterialOfferingRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                validationResult.Errors);
        }

        var relationshipResult = await ValidateSupplierRelationshipAsync(
            businessRelationshipId,
            cancellationToken);

        if (relationshipResult.IsFailed)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                relationshipResult.Errors);
        }

        var offeringKey = request.OfferingKey.Trim();
        var supplierSku = TrimToNull(request.SupplierSku);
        var normalizedSupplierSku = supplierSku is null ? null : _normalizer.NormalizeSupplierSku(supplierSku);

        var uniquenessResult = await ValidateUniqueIdentifiersAsync(
            businessRelationshipId,
            offeringKey,
            normalizedSupplierSku,
            excludeOfferingId: null,
            cancellationToken);

        if (uniquenessResult.IsFailed)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                uniquenessResult.Errors);
        }

        var now = DateTimeOffset.UtcNow;
        var firstObservedUtc = request.FirstObservedUtc ?? request.LastObservedUtc ?? now;
        var lastObservedUtc = request.LastObservedUtc ?? firstObservedUtc;

        if (lastObservedUtc < firstObservedUtc)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                new ConflictError(
                    "LastObservedUtc cannot be earlier than FirstObservedUtc."));
        }

        if (request.LastVerifiedUtc.HasValue
            && request.LastVerifiedUtc.Value < firstObservedUtc)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                new ConflictError(
                    "LastVerifiedUtc cannot be earlier than FirstObservedUtc."));
        }

        var offering = new SupplierMaterialOffering
        {
            Id = Guid.CreateVersion7(),
            BusinessRelationshipId = businessRelationshipId,
            OfferingKey = offeringKey,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            SupplierSku = supplierSku,
            NormalizedSupplierSku = normalizedSupplierSku,
            DeclaredFormat = TrimToNull(request.DeclaredFormat),
            OfferedMaterialTypeId = null,
            MappedMaterialId = null,
            MappingStatus = SourcingMappingStatus.Unmapped,
            MappingConfidence = null,
            MappingNotes = null,
            Status = SupplierOfferingStatus.Observed,
            FirstObservedUtc = firstObservedUtc,
            LastObservedUtc = lastObservedUtc,
            LastVerifiedUtc = request.LastVerifiedUtc
        };

        _db.Set<SupplierMaterialOffering>()
            .Add(offering);

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                new ConflictError(
                    "Another SupplierMaterialOffering already uses the same offering key or normalized supplier SKU within this supplier relationship."));
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                offering.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar deklarerad metadata utan att ändra offering key, supplier-relation, mapping eller status.
    /// </summary>
    public async Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(
        Guid supplierMaterialOfferingId,
        UpdateSupplierMaterialOfferingRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                validationResult.Errors);
        }

        var offering = await _db
            .Set<SupplierMaterialOffering>()
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

        if (request.LastObservedUtc < request.FirstObservedUtc)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                new ConflictError(
                    "LastObservedUtc cannot be earlier than FirstObservedUtc."));
        }

        if (request.LastVerifiedUtc.HasValue
            && request.LastVerifiedUtc.Value < request.FirstObservedUtc)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                new ConflictError(
                    "LastVerifiedUtc cannot be earlier than FirstObservedUtc."));
        }

        var supplierSku = TrimToNull(request.SupplierSku);
        var normalizedSupplierSku = supplierSku is null ? null : _normalizer.NormalizeSupplierSku(supplierSku);

        var uniquenessResult = await ValidateUniqueIdentifiersAsync(
            offering.BusinessRelationshipId,
            offering.OfferingKey,
            normalizedSupplierSku,
            offering.Id,
            cancellationToken);

        if (uniquenessResult.IsFailed)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                uniquenessResult.Errors);
        }

        _db.Entry(offering)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        offering.Name = request.Name.Trim();
        offering.Description = request.Description.Trim();
        offering.SupplierSku = supplierSku;
        offering.NormalizedSupplierSku = normalizedSupplierSku;
        offering.DeclaredFormat = TrimToNull(request.DeclaredFormat);
        offering.FirstObservedUtc = request.FirstObservedUtc;
        offering.LastObservedUtc = request.LastObservedUtc;
        offering.LastVerifiedUtc = request.LastVerifiedUtc;

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                new ConcurrencyError(
                    nameof(SupplierMaterialOffering),
                    offering.Id));
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                new ConflictError(
                    "Another SupplierMaterialOffering already uses the same offering key or normalized supplier SKU within this supplier relationship."));
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                offering.Id,
                cancellationToken));
    }

    /// <summary>
    /// Sätter normaliserad materialtyp och kanonisk materialmapping explicit.
    /// </summary>
    public async Task<Result<SupplierMaterialOfferingResponse>> SetMappingAsync(
        Guid supplierMaterialOfferingId,
        SetSupplierMaterialOfferingMappingRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                validationResult.Errors);
        }

        var offering = await _db
            .Set<SupplierMaterialOffering>()
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

        var referencesResult = await ValidateMappingReferencesAsync(
            request.OfferedMaterialTypeId,
            request.MappedMaterialId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                referencesResult.Errors);
        }

        if (request.MappingStatus == SourcingMappingStatus.Confirmed
            && !request.MappedMaterialId.HasValue)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                new ConflictError(
                    "Confirmed material mapping requires MappedMaterialId."));
        }

        _db.Entry(offering)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        offering.OfferedMaterialTypeId = request.OfferedMaterialTypeId;
        offering.MappedMaterialId = request.MappedMaterialId;
        offering.MappingStatus = request.MappingStatus;
        offering.MappingConfidence = request.MappingConfidence;
        offering.MappingNotes = TrimToNull(request.MappingNotes);

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
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
    /// Ändrar offering-status utan att införa en påhittad transition graph.
    /// </summary>
    public async Task<Result<SupplierMaterialOfferingResponse>> ChangeStatusAsync(
        Guid supplierMaterialOfferingId,
        ChangeSupplierMaterialOfferingStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<SupplierMaterialOfferingResponse>(
                validationResult.Errors);
        }

        var offering = await _db
            .Set<SupplierMaterialOffering>()
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

        _db.Entry(offering)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        offering.Status = request.Status;

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
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
    /// Kontrollerar att relationen finns och är av supplier-typ.
    /// </summary>
    private async Task<Result> ValidateSupplierRelationshipAsync(
        Guid businessRelationshipId,
        CancellationToken cancellationToken)
    {
        var relationship = await _db
            .Set<BusinessRelationship>()
            .AsNoTracking()
            .Where(x => x.Id == businessRelationshipId)
            .Select(
                x => new
                {
                    x.Id,
                    RelationshipTypeCode = x.BusinessRelationshipType.Code
                })
            .SingleOrDefaultAsync(
                cancellationToken);

        if (relationship is null)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(BusinessRelationship),
                    businessRelationshipId));
        }

        if (relationship.RelationshipTypeCode
            != BusinessRelationshipTypeCodes.Supplier)
        {
            return Result.Fail(
                new ConflictError(
                    "SupplierMaterialOffering can only belong to a supplier business relationship."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar offering key och supplier SKU mot databasens unika regler.
    /// </summary>
    private async Task<Result> ValidateUniqueIdentifiersAsync(
        Guid businessRelationshipId,
        string offeringKey,
        string? normalizedSupplierSku,
        Guid? excludeOfferingId,
        CancellationToken cancellationToken)
    {
        IQueryable<SupplierMaterialOffering> baseQuery = _db
            .Set<SupplierMaterialOffering>()
            .AsNoTracking()
            .Where(x => x.BusinessRelationshipId == businessRelationshipId);

        if (excludeOfferingId.HasValue)
        {
            baseQuery = baseQuery.Where(
                x => x.Id != excludeOfferingId.Value);
        }

        var offeringKeyExists = await baseQuery.AnyAsync(
            x => x.OfferingKey == offeringKey,
            cancellationToken);

        if (offeringKeyExists)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplierMaterialOffering key '{offeringKey}' already exists within this supplier relationship."));
        }

        if (normalizedSupplierSku is null)
        {
            return Result.Ok();
        }

        var supplierSkuExists = await baseQuery.AnyAsync(
            x => x.NormalizedSupplierSku == normalizedSupplierSku,
            cancellationToken);

        if (supplierSkuExists)
        {
            return Result.Fail(
                new ConflictError(
                    "Supplier SKU already exists within this supplier relationship."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar att referenser som används av materialmappingen finns.
    /// </summary>
    private async Task<Result> ValidateMappingReferencesAsync(
        int? offeredMaterialTypeId,
        Guid? mappedMaterialId,
        CancellationToken cancellationToken)
    {
        if (offeredMaterialTypeId.HasValue)
        {
            var materialTypeExists = await _db
                .Set<MaterialType>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == offeredMaterialTypeId.Value,
                    cancellationToken);

            if (!materialTypeExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(MaterialType),
                        offeredMaterialTypeId.Value));
            }
        }

        if (mappedMaterialId.HasValue)
        {
            var materialExists = await _db
                .Set<Material>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == mappedMaterialId.Value,
                    cancellationToken);

            if (!materialExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Material),
                        mappedMaterialId.Value));
            }
        }

        return Result.Ok();
    }

    /// <summary>
    /// Projekterar ett offering direkt från databasen.
    /// </summary>
    private async Task<SupplierMaterialOfferingResponse?> ProjectAsync(
        Guid supplierMaterialOfferingId,
        CancellationToken cancellationToken)
    {
        return await _db
            .Set<SupplierMaterialOffering>()
            .AsNoTracking()
            .Where(x => x.Id == supplierMaterialOfferingId)
            .Select(SupplierMaterialOfferingProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar ett sparat offering och kastar om det oväntat saknas.
    /// </summary>
    private async Task<SupplierMaterialOfferingResponse> ProjectRequiredAsync(
        Guid supplierMaterialOfferingId,
        CancellationToken cancellationToken)
    {
        return await ProjectAsync(
                supplierMaterialOfferingId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"SupplierMaterialOffering '{supplierMaterialOfferingId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Lägger på stabil sortering före paginering.
    /// </summary>
    /// <summary>
    /// Söker, filtrerar, sorterar och paginerar offerings över samtliga supplier-relationer
    /// som ägs av organisationen.
    /// </summary>
    public async Task<Result<PagedResult<SupplierMaterialOfferingListResponse>>> QueryByOrganizationAsync(
        Guid owningOrganizationId,
        SupplierMaterialOfferingQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<PagedResult<SupplierMaterialOfferingListResponse>>(
                validationResult.Errors);
        }

        IQueryable<SupplierMaterialOffering> query = _db
            .Set<SupplierMaterialOffering>()
            .AsNoTracking()
            .Where(x =>
                x.BusinessRelationship.OwningOrganizationId == owningOrganizationId
                && x.BusinessRelationship.BusinessRelationshipType.Code == BusinessRelationshipTypeCodes.Supplier);

        if (request.BusinessRelationshipId.HasValue)
        {
            query = query.Where(
                x => x.BusinessRelationshipId == request.BusinessRelationshipId.Value);
        }

        query = ApplyFilters(query, request);

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(
            query,
            request.SortBy,
            request.SortDirection);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(SupplierMaterialOfferingProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok(
            new PagedResult<SupplierMaterialOfferingListResponse>
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
    private static IQueryable<SupplierMaterialOffering> ApplyFilters(
        IQueryable<SupplierMaterialOffering> query,
        SupplierMaterialOfferingQueryRequest request)
    {
        if (request.Status.HasValue)
        {
            query = query.Where(
                x => x.Status == request.Status.Value);
        }

        if (request.MappingStatus.HasValue)
        {
            query = query.Where(
                x => x.MappingStatus == request.MappingStatus.Value);
        }

        if (request.MappedMaterialId.HasValue)
        {
            query = query.Where(
                x => x.MappedMaterialId == request.MappedMaterialId.Value);
        }

        if (request.OfferedMaterialTypeId.HasValue)
        {
            query = query.Where(
                x => x.OfferedMaterialTypeId == request.OfferedMaterialTypeId.Value);
        }

        if (request.BiologicalSourceId.HasValue)
        {
            var biologicalSourceId = request.BiologicalSourceId.Value;

            query = query.Where(
                x => x.BiologicalSourceLinks.Any(
                    link => link.BiologicalSourceClaim.BiologicalSourceId == biologicalSourceId));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern =
                $"%{request.Search.Trim()}%";

            query = query.Where(
                x =>
                    EF.Functions.ILike(x.OfferingKey, searchPattern)
                    || EF.Functions.ILike(x.Name, searchPattern)
                    || (x.SupplierSku != null && EF.Functions.ILike(x.SupplierSku, searchPattern))
                    || (x.DeclaredFormat != null && EF.Functions.ILike(x.DeclaredFormat, searchPattern))
                    || (x.MappedMaterial != null && EF.Functions.ILike(x.MappedMaterial.Name, searchPattern))
                    || (x.OfferedMaterialType != null && EF.Functions.ILike(x.OfferedMaterialType.Name, searchPattern))
                    || x.BiologicalSourceLinks.Any(
                        link =>
                            EF.Functions.ILike(link.BiologicalSourceClaim.DeclaredName, searchPattern)
                            || (link.BiologicalSourceClaim.BiologicalSource != null
                                && EF.Functions.ILike(link.BiologicalSourceClaim.BiologicalSource.Name, searchPattern))));
        }

        return query;
    }

    private static IQueryable<SupplierMaterialOffering> ApplySorting(
        IQueryable<SupplierMaterialOffering> query,
        SupplierMaterialOfferingSortField sortBy,
        SortDirection sortDirection)
    {
        return (sortBy, sortDirection) switch
        {
            (SupplierMaterialOfferingSortField.Name, SortDirection.Descending) =>
                query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),

            (SupplierMaterialOfferingSortField.Status, SortDirection.Ascending) =>
                query.OrderBy(x => x.Status).ThenBy(x => x.Name).ThenBy(x => x.Id),

            (SupplierMaterialOfferingSortField.Status, SortDirection.Descending) =>
                query.OrderByDescending(x => x.Status).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),

            (SupplierMaterialOfferingSortField.MappingStatus, SortDirection.Ascending) =>
                query.OrderBy(x => x.MappingStatus).ThenBy(x => x.Name).ThenBy(x => x.Id),

            (SupplierMaterialOfferingSortField.MappingStatus, SortDirection.Descending) =>
                query.OrderByDescending(x => x.MappingStatus).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),

            (SupplierMaterialOfferingSortField.FirstObservedUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.FirstObservedUtc).ThenBy(x => x.Id),

            (SupplierMaterialOfferingSortField.FirstObservedUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.FirstObservedUtc).ThenByDescending(x => x.Id),

            (SupplierMaterialOfferingSortField.LastObservedUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.LastObservedUtc).ThenBy(x => x.Id),

            (SupplierMaterialOfferingSortField.LastObservedUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.LastObservedUtc).ThenByDescending(x => x.Id),

            (SupplierMaterialOfferingSortField.LastVerifiedUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.LastVerifiedUtc).ThenBy(x => x.Id),

            (SupplierMaterialOfferingSortField.LastVerifiedUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.LastVerifiedUtc).ThenByDescending(x => x.Id),

            (SupplierMaterialOfferingSortField.CreateDateUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.CreateDateUtc).ThenBy(x => x.Id),

            (SupplierMaterialOfferingSortField.CreateDateUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.CreateDateUtc).ThenByDescending(x => x.Id),

            _ =>
                query.OrderBy(x => x.Name).ThenBy(x => x.Id)
        };
    }

    /// <summary>
    /// Kontrollerar om databasfelet kommer från en unik constraint.
    /// </summary>
    private static bool IsUniqueViolation(
        DbUpdateException exception)
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
    private static string? TrimToNull(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
