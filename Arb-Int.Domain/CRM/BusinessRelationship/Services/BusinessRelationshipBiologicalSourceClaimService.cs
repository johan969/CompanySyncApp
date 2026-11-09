using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Hanterar biologiska source-claims och deras evidens, mapping och geografi.
    /// </summary>
    public sealed class BusinessRelationshipBiologicalSourceClaimService : IBusinessRelationshipBiologicalSourceClaimService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;
        private readonly IBiologicalSourceClaimNormalizer _normalizer;
        private readonly ILogger<BusinessRelationshipBiologicalSourceClaimService> _logger;

        /// <summary>
        /// Skapar claim-servicen med databas-, validerings- och normaliseringsberoenden.
        /// </summary>
        public BusinessRelationshipBiologicalSourceClaimService(Arb_IntDbContext db, IValidationService validationService, IBiologicalSourceClaimNormalizer normalizer, ILogger<BusinessRelationshipBiologicalSourceClaimService> logger)
        {
            _db = db;
            _validationService = validationService;
            _normalizer = normalizer;
            _logger = logger;
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar claims för en affärsrelation.
        /// </summary>
        public async Task<Result<PagedResult<BiologicalSourceClaimListResponse>>> QueryAsync(
            Guid businessRelationshipId,
            BiologicalSourceClaimQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<BiologicalSourceClaimListResponse>>(
                    validationResult.Errors);
            }

            var relationshipExists = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == businessRelationshipId, cancellationToken);

            if (!relationshipExists)
            {
                return Result.Fail<PagedResult<BiologicalSourceClaimListResponse>>(
                    new NotFoundError(nameof(BusinessRelationship), businessRelationshipId));
            }

            IQueryable<BusinessRelationshipBiologicalSourceClaim> query = _db
                .Set<BusinessRelationshipBiologicalSourceClaim>()
                .AsNoTracking()
                .Where(x => x.BusinessRelationshipId == businessRelationshipId);

            query = ApplyFilters(query, request);

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(BiologicalSourceClaimProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<BiologicalSourceClaimListResponse>
                {
                    Items = items,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                });
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar claims över samtliga affärsrelationer
        /// som ägs av organisationen.
        /// </summary>
        public async Task<Result<PagedResult<BiologicalSourceClaimListResponse>>> QueryByOrganizationAsync(
            Guid owningOrganizationId,
            BiologicalSourceClaimQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<BiologicalSourceClaimListResponse>>(
                    validationResult.Errors);
            }

            IQueryable<BusinessRelationshipBiologicalSourceClaim> query = _db
                .Set<BusinessRelationshipBiologicalSourceClaim>()
                .AsNoTracking()
                .Where(x => x.BusinessRelationship.OwningOrganizationId == owningOrganizationId);

            if (request.BusinessRelationshipId.HasValue)
            {
                query = query.Where(x => x.BusinessRelationshipId == request.BusinessRelationshipId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.BusinessRelationshipTypeCode))
            {
                var typeCode = request.BusinessRelationshipTypeCode.Trim();
                query = query.Where(x => x.BusinessRelationship.BusinessRelationshipType.Code == typeCode);
            }

            query = ApplyFilters(query, request);

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(BiologicalSourceClaimProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<BiologicalSourceClaimListResponse>
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
        private static IQueryable<BusinessRelationshipBiologicalSourceClaim> ApplyFilters(
            IQueryable<BusinessRelationshipBiologicalSourceClaim> query,
            BiologicalSourceClaimQueryRequest request)
        {
            if (request.BiologicalSourceId.HasValue)
            {
                query = query.Where(x => x.BiologicalSourceId == request.BiologicalSourceId.Value);
            }

            if (request.RelationshipKind.HasValue)
            {
                query = query.Where(x => x.RelationshipKind == request.RelationshipKind.Value);
            }

            if (request.MappingStatus.HasValue)
            {
                query = query.Where(x => x.MappingStatus == request.MappingStatus.Value);
            }

            if (request.VerificationStatus.HasValue)
            {
                query = query.Where(x => x.VerificationStatus == request.VerificationStatus.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var pattern = $"%{request.Search.Trim()}%";

                query = query.Where(x =>
                    EF.Functions.ILike(x.DeclaredName, pattern)
                    || (x.Notes != null && EF.Functions.ILike(x.Notes, pattern))
                    || (x.BiologicalSource != null && EF.Functions.ILike(x.BiologicalSource.Name, pattern)));
            }

            return query;
        }

        /// <summary>
        /// Sorterar frågan och håller ordningen stabil via en explicit tiebreaker.
        /// </summary>
        private static IQueryable<BusinessRelationshipBiologicalSourceClaim> ApplySorting(
            IQueryable<BusinessRelationshipBiologicalSourceClaim> query,
            BiologicalSourceClaimSortField sortBy,
            SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (BiologicalSourceClaimSortField.RelationshipKind, SortDirection.Descending) => query.OrderByDescending(x => x.RelationshipKind).ThenByDescending(x => x.DeclaredName).ThenByDescending(x => x.Id),
                (BiologicalSourceClaimSortField.DeclaredName, SortDirection.Ascending) => query.OrderBy(x => x.DeclaredName).ThenBy(x => x.Id),
                (BiologicalSourceClaimSortField.DeclaredName, SortDirection.Descending) => query.OrderByDescending(x => x.DeclaredName).ThenByDescending(x => x.Id),
                (BiologicalSourceClaimSortField.MappingStatus, SortDirection.Ascending) => query.OrderBy(x => x.MappingStatus).ThenBy(x => x.DeclaredName).ThenBy(x => x.Id),
                (BiologicalSourceClaimSortField.MappingStatus, SortDirection.Descending) => query.OrderByDescending(x => x.MappingStatus).ThenByDescending(x => x.DeclaredName).ThenByDescending(x => x.Id),
                (BiologicalSourceClaimSortField.VerificationStatus, SortDirection.Ascending) => query.OrderBy(x => x.VerificationStatus).ThenBy(x => x.DeclaredName).ThenBy(x => x.Id),
                (BiologicalSourceClaimSortField.VerificationStatus, SortDirection.Descending) => query.OrderByDescending(x => x.VerificationStatus).ThenByDescending(x => x.DeclaredName).ThenByDescending(x => x.Id),
                (BiologicalSourceClaimSortField.FirstObservedUtc, SortDirection.Ascending) => query.OrderBy(x => x.FirstObservedUtc).ThenBy(x => x.Id),
                (BiologicalSourceClaimSortField.FirstObservedUtc, SortDirection.Descending) => query.OrderByDescending(x => x.FirstObservedUtc).ThenByDescending(x => x.Id),
                (BiologicalSourceClaimSortField.LastObservedUtc, SortDirection.Ascending) => query.OrderBy(x => x.LastObservedUtc).ThenBy(x => x.Id),
                (BiologicalSourceClaimSortField.LastObservedUtc, SortDirection.Descending) => query.OrderByDescending(x => x.LastObservedUtc).ThenByDescending(x => x.Id),
                (BiologicalSourceClaimSortField.LastVerifiedUtc, SortDirection.Ascending) => query.OrderBy(x => x.LastVerifiedUtc).ThenBy(x => x.Id),
                (BiologicalSourceClaimSortField.LastVerifiedUtc, SortDirection.Descending) => query.OrderByDescending(x => x.LastVerifiedUtc).ThenByDescending(x => x.Id),
                _ => query.OrderBy(x => x.RelationshipKind).ThenBy(x => x.DeclaredName).ThenBy(x => x.Id)
            };
        }

        /// <summary>
        /// Hämtar ett komplett claim med evidens och geografiska kopplingar.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> GetByIdAsync(
            Guid claimId,
            CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(claimId, cancellationToken);

            if (response is null)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaim), claimId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Validerar och skapar ett biologiskt source-claim.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> CreateAsync(
            Guid businessRelationshipId,
            CreateBiologicalSourceClaimRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(validationResult.Errors);
            }

            var relationshipExists = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == businessRelationshipId, cancellationToken);

            if (!relationshipExists)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationship), businessRelationshipId));
            }

            var biologicalSourceResult = await ValidateBiologicalSourceAsync(
                request.BiologicalSourceId,
                cancellationToken);

            if (biologicalSourceResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(biologicalSourceResult.Errors);
            }

            var normalizedDeclaredName = _normalizer.NormalizeDeclaredName(request.DeclaredName);

            var uniquenessResult = await ValidateClaimUniquenessAsync(
                businessRelationshipId,
                request.RelationshipKind,
                normalizedDeclaredName,
                request.BiologicalSourceId,
                excludeClaimId: null,
                cancellationToken);

            if (uniquenessResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(uniquenessResult.Errors);
            }

            var now = DateTimeOffset.UtcNow;
            var firstObservedUtc = request.FirstObservedUtc ?? request.LastObservedUtc ?? now;
            var lastObservedUtc = request.LastObservedUtc ?? firstObservedUtc;

            if (request.LastVerifiedUtc.HasValue && request.LastVerifiedUtc.Value < firstObservedUtc)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new ConflictError("LastVerifiedUtc cannot be earlier than FirstObservedUtc."));
            }

            var claim = new BusinessRelationshipBiologicalSourceClaim
            {
                Id = Guid.CreateVersion7(),
                BusinessRelationshipId = businessRelationshipId,
                DeclaredName = request.DeclaredName.Trim(),
                NormalizedDeclaredName = normalizedDeclaredName,
                BiologicalSourceId = request.BiologicalSourceId,
                RelationshipKind = request.RelationshipKind,
                MappingStatus = request.MappingStatus,
                MappingConfidence = request.MappingConfidence,
                VerificationStatus = request.VerificationStatus,
                FirstObservedUtc = firstObservedUtc,
                LastObservedUtc = lastObservedUtc,
                LastVerifiedUtc = request.LastVerifiedUtc,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<BusinessRelationshipBiologicalSourceClaim>().Add(claim);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Biological source claim {BiologicalSourceClaimId} created for business relationship {BusinessRelationshipId}. RelationshipKind={RelationshipKind}, BiologicalSourceId={BiologicalSourceId}, MappingStatus={MappingStatus}, VerificationStatus={VerificationStatus}",
                claim.Id,
                claim.BusinessRelationshipId,
                claim.RelationshipKind,
                claim.BiologicalSourceId,
                claim.MappingStatus,
                claim.VerificationStatus);

            return Result.Ok(await ProjectRequiredAsync(claim.Id, cancellationToken));
        }

        /// <summary>
        /// Uppdaterar claimens deklarerade namn, relationship kind och observationsintervall.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> UpdateAsync(
            Guid claimId,
            UpdateBiologicalSourceClaimRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(validationResult.Errors);
            }

            var claim = await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
                .SingleOrDefaultAsync(x => x.Id == claimId, cancellationToken);

            if (claim is null)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaim), claimId));
            }

            if (claim.LastVerifiedUtc.HasValue && claim.LastVerifiedUtc.Value < request.FirstObservedUtc)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new ConflictError("FirstObservedUtc cannot be later than the existing LastVerifiedUtc."));
            }

            var normalizedDeclaredName = _normalizer.NormalizeDeclaredName(request.DeclaredName);

            var uniquenessResult = await ValidateClaimUniquenessAsync(
                claim.BusinessRelationshipId,
                request.RelationshipKind,
                normalizedDeclaredName,
                claim.BiologicalSourceId,
                claim.Id,
                cancellationToken);

            if (uniquenessResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(uniquenessResult.Errors);
            }

            claim.DeclaredName = request.DeclaredName.Trim();
            claim.NormalizedDeclaredName = normalizedDeclaredName;
            claim.RelationshipKind = request.RelationshipKind;
            claim.FirstObservedUtc = request.FirstObservedUtc;
            claim.LastObservedUtc = request.LastObservedUtc;
            claim.Notes = TrimToNull(request.Notes);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Biological source claim {BiologicalSourceClaimId} updated for business relationship {BusinessRelationshipId}. RelationshipKind={RelationshipKind}",
                claim.Id,
                claim.BusinessRelationshipId,
                claim.RelationshipKind);

            return Result.Ok(await ProjectRequiredAsync(claim.Id, cancellationToken));
        }

        /// <summary>
        /// Ändrar mappingen och säkerställer att den nya kombinationen är unik inom affärsrelationen.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> SetMappingAsync(
            Guid claimId,
            SetBiologicalSourceClaimMappingRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(validationResult.Errors);
            }

            var claim = await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
                .SingleOrDefaultAsync(x => x.Id == claimId, cancellationToken);

            if (claim is null)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaim), claimId));
            }

            var biologicalSourceResult = await ValidateBiologicalSourceAsync(
                request.BiologicalSourceId,
                cancellationToken);

            if (biologicalSourceResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(biologicalSourceResult.Errors);
            }

            var uniquenessResult = await ValidateClaimUniquenessAsync(
                claim.BusinessRelationshipId,
                claim.RelationshipKind,
                claim.NormalizedDeclaredName,
                request.BiologicalSourceId,
                claim.Id,
                cancellationToken);

            if (uniquenessResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(uniquenessResult.Errors);
            }

            var previousBiologicalSourceId = claim.BiologicalSourceId;
            var previousMappingStatus = claim.MappingStatus;

            claim.BiologicalSourceId = request.BiologicalSourceId;
            claim.MappingStatus = request.MappingStatus;
            claim.MappingConfidence = request.MappingConfidence;

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Biological source claim {BiologicalSourceClaimId} mapping changed. PreviousBiologicalSourceId={PreviousBiologicalSourceId}, BiologicalSourceId={BiologicalSourceId}, PreviousMappingStatus={PreviousMappingStatus}, MappingStatus={MappingStatus}, MappingConfidence={MappingConfidence}",
                claim.Id,
                previousBiologicalSourceId,
                claim.BiologicalSourceId,
                previousMappingStatus,
                claim.MappingStatus,
                claim.MappingConfidence);

            return Result.Ok(await ProjectRequiredAsync(claim.Id, cancellationToken));
        }

        /// <summary>
        /// Ändrar claimens verifieringsstatus och eventuell verifieringstid.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> SetVerificationAsync(
            Guid claimId,
            SetBiologicalSourceClaimVerificationRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(validationResult.Errors);
            }

            var claim = await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
                .SingleOrDefaultAsync(x => x.Id == claimId, cancellationToken);

            if (claim is null)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaim), claimId));
            }

            if (request.LastVerifiedUtc.HasValue && request.LastVerifiedUtc.Value < claim.FirstObservedUtc)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new ConflictError("LastVerifiedUtc cannot be earlier than FirstObservedUtc."));
            }

            var previousVerificationStatus = claim.VerificationStatus;

            claim.VerificationStatus = request.VerificationStatus;
            claim.LastVerifiedUtc = request.LastVerifiedUtc;

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Biological source claim {BiologicalSourceClaimId} verification changed from {PreviousVerificationStatus} to {VerificationStatus}. LastVerifiedUtc={LastVerifiedUtc}",
                claim.Id,
                previousVerificationStatus,
                claim.VerificationStatus,
                claim.LastVerifiedUtc);

            return Result.Ok(await ProjectRequiredAsync(claim.Id, cancellationToken));
        }

        /// <summary>
        /// Lägger till en unik evidenspost på ett claim.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> AddEvidenceAsync(
            Guid claimId,
            CreateBiologicalSourceClaimEvidenceRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(validationResult.Errors);
            }

            var claimExists = await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == claimId, cancellationToken);

            if (!claimExists)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaim), claimId));
            }

            var sourceSystem = request.SourceSystem.Trim();
            var sourceRecordKey = request.SourceRecordKey.Trim();

            var duplicateExists = await _db.Set<BusinessRelationshipBiologicalSourceClaimEvidence>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.BusinessRelationshipBiologicalSourceClaimId == claimId
                        && x.SourceSystem == sourceSystem
                        && x.SourceRecordKey == sourceRecordKey,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new ConflictError("The claim already contains evidence with the same source system and record key."));
            }

            var evidence = new BusinessRelationshipBiologicalSourceClaimEvidence
            {
                Id = Guid.CreateVersion7(),
                BusinessRelationshipBiologicalSourceClaimId = claimId,
                EvidenceType = request.EvidenceType,
                SourceSystem = sourceSystem,
                SourceRecordKey = sourceRecordKey,
                SourceUrl = TrimToNull(request.SourceUrl),
                ObservedUtc = request.ObservedUtc ?? DateTimeOffset.UtcNow,
                Notes = TrimToNull(request.Notes),
                ContentHash = TrimToNull(request.ContentHash)
            };

            _db.Set<BusinessRelationshipBiologicalSourceClaimEvidence>().Add(evidence);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Evidence {BiologicalSourceClaimEvidenceId} added to biological source claim {BiologicalSourceClaimId}. EvidenceType={EvidenceType}, SourceSystem={SourceSystem}",
                evidence.Id,
                claimId,
                evidence.EvidenceType,
                evidence.SourceSystem);

            return Result.Ok(await ProjectRequiredAsync(claimId, cancellationToken));
        }

        /// <summary>
        /// Uppdaterar metadata för en befintlig evidenspost.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> UpdateEvidenceAsync(
            Guid evidenceId,
            UpdateBiologicalSourceClaimEvidenceRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(validationResult.Errors);
            }

            var evidence = await _db.Set<BusinessRelationshipBiologicalSourceClaimEvidence>()
                .SingleOrDefaultAsync(x => x.Id == evidenceId, cancellationToken);

            if (evidence is null)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaimEvidence), evidenceId));
            }

            evidence.EvidenceType = request.EvidenceType;
            evidence.SourceUrl = TrimToNull(request.SourceUrl);
            evidence.ObservedUtc = request.ObservedUtc;
            evidence.Notes = TrimToNull(request.Notes);
            evidence.ContentHash = TrimToNull(request.ContentHash);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Evidence {BiologicalSourceClaimEvidenceId} updated for biological source claim {BiologicalSourceClaimId}. EvidenceType={EvidenceType}",
                evidence.Id,
                evidence.BusinessRelationshipBiologicalSourceClaimId,
                evidence.EvidenceType);

            return Result.Ok(
                await ProjectRequiredAsync(
                    evidence.BusinessRelationshipBiologicalSourceClaimId,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort en evidenspost utan att ta bort claimen.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> DeleteEvidenceAsync(
            Guid evidenceId,
            CancellationToken cancellationToken = default)
        {
            var evidence = await _db.Set<BusinessRelationshipBiologicalSourceClaimEvidence>()
                .SingleOrDefaultAsync(x => x.Id == evidenceId, cancellationToken);

            if (evidence is null)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaimEvidence), evidenceId));
            }

            var claimId = evidence.BusinessRelationshipBiologicalSourceClaimId;

            _db.Set<BusinessRelationshipBiologicalSourceClaimEvidence>().Remove(evidence);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Evidence {BiologicalSourceClaimEvidenceId} deleted from biological source claim {BiologicalSourceClaimId}",
                evidence.Id,
                claimId);

            return Result.Ok(await ProjectRequiredAsync(claimId, cancellationToken));
        }

        /// <summary>
        /// Lägger till en geografisk plats och hanterar claimens primära region.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> AddRegionAsync(
            Guid claimId,
            CreateBiologicalSourceClaimRegionRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(validationResult.Errors);
            }

            var claimExists = await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == claimId, cancellationToken);

            if (!claimExists)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaim), claimId));
            }

            var locationExists = await _db.Set<GeographicLocation>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.GeographicLocationId, cancellationToken);

            if (!locationExists)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(GeographicLocation), request.GeographicLocationId));
            }

            var duplicateExists = await _db.Set<BusinessRelationshipBiologicalSourceClaimRegion>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.BusinessRelationshipBiologicalSourceClaimId == claimId
                        && x.GeographicLocationId == request.GeographicLocationId
                        && x.Role == request.Role,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new ConflictError("The claim already contains this geographic location with the selected role."));
            }

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryRegionAsync(
                    claimId,
                    excludeRegionId: null,
                    cancellationToken);
            }

            var region = new BusinessRelationshipBiologicalSourceClaimRegion
            {
                Id = Guid.CreateVersion7(),
                BusinessRelationshipBiologicalSourceClaimId = claimId,
                GeographicLocationId = request.GeographicLocationId,
                Role = request.Role,
                IsPrimary = request.IsPrimary,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<BusinessRelationshipBiologicalSourceClaimRegion>().Add(region);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Region {BiologicalSourceClaimRegionId} added to biological source claim {BiologicalSourceClaimId}. GeographicLocationId={GeographicLocationId}, Role={Role}, IsPrimary={IsPrimary}",
                region.Id,
                claimId,
                region.GeographicLocationId,
                region.Role,
                region.IsPrimary);

            return Result.Ok(await ProjectRequiredAsync(claimId, cancellationToken));
        }

        /// <summary>
        /// Uppdaterar roll, primärstatus och anteckningar för en geografisk claim-koppling.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> UpdateRegionAsync(
            Guid regionId,
            UpdateBiologicalSourceClaimRegionRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(validationResult.Errors);
            }

            var region = await _db.Set<BusinessRelationshipBiologicalSourceClaimRegion>()
                .SingleOrDefaultAsync(x => x.Id == regionId, cancellationToken);

            if (region is null)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaimRegion), regionId));
            }

            var duplicateExists = await _db.Set<BusinessRelationshipBiologicalSourceClaimRegion>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id != region.Id
                        && x.BusinessRelationshipBiologicalSourceClaimId == region.BusinessRelationshipBiologicalSourceClaimId
                        && x.GeographicLocationId == region.GeographicLocationId
                        && x.Role == request.Role,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new ConflictError("The claim already contains this geographic location with the selected role."));
            }

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryRegionAsync(
                    region.BusinessRelationshipBiologicalSourceClaimId,
                    region.Id,
                    cancellationToken);
            }

            var previousRole = region.Role;
            var wasPrimary = region.IsPrimary;

            region.Role = request.Role;
            region.IsPrimary = request.IsPrimary;
            region.Notes = TrimToNull(request.Notes);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Region {BiologicalSourceClaimRegionId} updated for biological source claim {BiologicalSourceClaimId}. PreviousRole={PreviousRole}, Role={Role}, WasPrimary={WasPrimary}, IsPrimary={IsPrimary}",
                region.Id,
                region.BusinessRelationshipBiologicalSourceClaimId,
                previousRole,
                region.Role,
                wasPrimary,
                region.IsPrimary);

            return Result.Ok(
                await ProjectRequiredAsync(
                    region.BusinessRelationshipBiologicalSourceClaimId,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort en geografisk koppling utan att påverka GeographicLocation.
        /// </summary>
        public async Task<Result<BiologicalSourceClaimResponse>> DeleteRegionAsync(
            Guid regionId,
            CancellationToken cancellationToken = default)
        {
            var region = await _db.Set<BusinessRelationshipBiologicalSourceClaimRegion>()
                .SingleOrDefaultAsync(x => x.Id == regionId, cancellationToken);

            if (region is null)
            {
                return Result.Fail<BiologicalSourceClaimResponse>(
                    new NotFoundError(nameof(BusinessRelationshipBiologicalSourceClaimRegion), regionId));
            }

            var claimId = region.BusinessRelationshipBiologicalSourceClaimId;

            _db.Set<BusinessRelationshipBiologicalSourceClaimRegion>().Remove(region);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Region {BiologicalSourceClaimRegionId} deleted from biological source claim {BiologicalSourceClaimId}. GeographicLocationId={GeographicLocationId}, Role={Role}, WasPrimary={WasPrimary}",
                region.Id,
                claimId,
                region.GeographicLocationId,
                region.Role,
                region.IsPrimary);

            return Result.Ok(await ProjectRequiredAsync(claimId, cancellationToken));
        }

        /// <summary>
        /// Kontrollerar att eventuell mappad biologisk källa finns.
        /// </summary>
        private async Task<Result> ValidateBiologicalSourceAsync(
            Guid? biologicalSourceId,
            CancellationToken cancellationToken)
        {
            if (!biologicalSourceId.HasValue)
            {
                return Result.Ok();
            }

            var exists = await _db.Set<BiologicalSource>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == biologicalSourceId.Value, cancellationToken);

            if (!exists)
            {
                return Result.Fail(
                    new NotFoundError(nameof(BiologicalSource), biologicalSourceId.Value));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar den unika claim-regel som gäller för mappade respektive omappade claims.
        /// </summary>
        private async Task<Result> ValidateClaimUniquenessAsync(
            Guid businessRelationshipId,
            BiologicalSourceRelationshipKind relationshipKind,
            string normalizedDeclaredName,
            Guid? biologicalSourceId,
            Guid? excludeClaimId,
            CancellationToken cancellationToken)
        {
            IQueryable<BusinessRelationshipBiologicalSourceClaim> query = _db
                .Set<BusinessRelationshipBiologicalSourceClaim>()
                .AsNoTracking()
                .Where(x =>
                    x.BusinessRelationshipId == businessRelationshipId
                    && x.RelationshipKind == relationshipKind);

            if (excludeClaimId.HasValue)
            {
                query = query.Where(x => x.Id != excludeClaimId.Value);
            }

            if (biologicalSourceId.HasValue)
            {
                query = query.Where(x => x.BiologicalSourceId == biologicalSourceId.Value);
            }
            else
            {
                query = query.Where(x =>
                    x.BiologicalSourceId == null
                    && x.NormalizedDeclaredName == normalizedDeclaredName);
            }

            var duplicateExists = await query.AnyAsync(cancellationToken);

            if (!duplicateExists)
            {
                return Result.Ok();
            }

            return Result.Fail(
                new ConflictError(
                    biologicalSourceId.HasValue
                        ? "The business relationship already contains this biological source with the selected relationship kind."
                        : "The business relationship already contains an unmapped claim with the same declared name and relationship kind."));
        }

        /// <summary>
        /// Tar bort primärstatus från övriga regioner på samma claim.
        /// </summary>
        private async Task DemoteCurrentPrimaryRegionAsync(
            Guid claimId,
            Guid? excludeRegionId,
            CancellationToken cancellationToken)
        {
            IQueryable<BusinessRelationshipBiologicalSourceClaimRegion> query = _db
                .Set<BusinessRelationshipBiologicalSourceClaimRegion>()
                .Where(x =>
                    x.BusinessRelationshipBiologicalSourceClaimId == claimId
                    && x.IsPrimary);

            if (excludeRegionId.HasValue)
            {
                query = query.Where(x => x.Id != excludeRegionId.Value);
            }

            await query.ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.IsPrimary, false),
                cancellationToken);
        }

        /// <summary>
        /// Projekterar ett komplett claim direkt från databasen.
        /// </summary>
        private async Task<BiologicalSourceClaimResponse?> ProjectAsync(
            Guid claimId,
            CancellationToken cancellationToken)
        {
            return await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
                .AsNoTracking()
                .Where(x => x.Id == claimId)
                .Select(BiologicalSourceClaimProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar ett sparat claim och kastar om det oväntat saknas.
        /// </summary>
        private async Task<BiologicalSourceClaimResponse> ProjectRequiredAsync(
            Guid claimId,
            CancellationToken cancellationToken)
        {
            return await ProjectAsync(claimId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"BusinessRelationshipBiologicalSourceClaim '{claimId}' was saved but could not be projected.");
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