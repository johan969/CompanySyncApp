using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Hanterar affärsrelationer, referensdata och explicita stegbyten.
    /// </summary>
    public sealed class BusinessRelationshipService : IBusinessRelationshipService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;
        private readonly ILogger<BusinessRelationshipService> _logger;

        /// <summary>
        /// Skapar CRM-servicen med databas- och valideringsberoenden.
        /// </summary>
        public BusinessRelationshipService(Arb_IntDbContext db, IValidationService validationService, ILogger<BusinessRelationshipService> logger)
        {
            _db = db;
            _validationService = validationService;
            _logger = logger;
        }

        /// <summary>
        /// Hämtar en affärsrelation direkt som ett projekterat response-objekt.
        /// </summary>
        public async Task<Result<BusinessRelationshipResponse>> GetByIdAsync(Guid businessRelationshipId, CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(businessRelationshipId, cancellationToken);

            if (response is null)
            {
                return Result.Fail<BusinessRelationshipResponse>(
                    new NotFoundError(nameof(BusinessRelationship), businessRelationshipId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar affärsrelationer.
        /// </summary>
        public async Task<Result<PagedResult<BusinessRelationshipListResponse>>> QueryAsync(
            Guid owningOrganizationId,
            BusinessRelationshipQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<BusinessRelationshipListResponse>>(validationResult.Errors);
            }

            var organizationExists = await _db.Set<Organization>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == owningOrganizationId, cancellationToken);

            if (!organizationExists)
            {
                return Result.Fail<PagedResult<BusinessRelationshipListResponse>>(
                    new NotFoundError(nameof(Organization), owningOrganizationId));
            }

            IQueryable<BusinessRelationship> query = _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .Where(x => x.OwningOrganizationId == owningOrganizationId);

            if (request.BusinessRelationshipTypeId.HasValue)
            {
                query = query.Where(x => x.BusinessRelationshipTypeId == request.BusinessRelationshipTypeId.Value);
            }

            if (request.BusinessRelationshipStageId.HasValue)
            {
                query = query.Where(x => x.BusinessRelationshipStageId == request.BusinessRelationshipStageId.Value);
            }

            if (request.Priority.HasValue)
            {
                query = query.Where(x => x.Priority == request.Priority.Value);
            }

            if (request.OwnerUserId.HasValue)
            {
                query = query.Where(x => x.OwnerUserId == request.OwnerUserId.Value);
            }

            if (request.IsClosed.HasValue)
            {
                query = query.Where(x => x.BusinessRelationshipStage.IsClosed == request.IsClosed.Value);
            }

            if (request.NextActionBeforeUtc.HasValue)
            {
                query = query.Where(x => x.NextActionUtc != null && x.NextActionUtc <= request.NextActionBeforeUtc.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchPattern = $"%{request.Search.Trim()}%";

                query = query.Where(x =>
                    EF.Functions.ILike(x.Counterparty.Name, searchPattern)
                    || (x.LeadSource != null && EF.Functions.ILike(x.LeadSource, searchPattern)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(BusinessRelationshipProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(new PagedResult<BusinessRelationshipListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
        }

        /// <summary>
        /// Hämtar aktiva eller samtliga typer av affärsrelationer.
        /// </summary>
        public async Task<Result<IReadOnlyList<BusinessRelationshipTypeResponse>>> GetTypesAsync(
            bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<BusinessRelationshipType> query = _db.Set<BusinessRelationshipType>()
                .AsNoTracking();

            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            var types = await query
                .OrderBy(x => x.Name)
                .Select(x => new BusinessRelationshipTypeResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<BusinessRelationshipTypeResponse>>(types);
        }

        /// <summary>
        /// Hämtar samtliga steg för en viss affärsrelationstyp.
        /// </summary>
        public async Task<Result<IReadOnlyList<BusinessRelationshipStageResponse>>> GetStagesAsync(
            int businessRelationshipTypeId,
            CancellationToken cancellationToken = default)
        {
            var typeExists = await _db.Set<BusinessRelationshipType>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == businessRelationshipTypeId, cancellationToken);

            if (!typeExists)
            {
                return Result.Fail<IReadOnlyList<BusinessRelationshipStageResponse>>(
                    new NotFoundError(nameof(BusinessRelationshipType), businessRelationshipTypeId));
            }

            var stages = await _db.Set<BusinessRelationshipStage>()
                .AsNoTracking()
                .Where(x => x.BusinessRelationshipTypeId == businessRelationshipTypeId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Select(x => new BusinessRelationshipStageResponse
                {
                    Id = x.Id,
                    BusinessRelationshipTypeId = x.BusinessRelationshipTypeId,
                    Code = x.Code,
                    Name = x.Name,
                    SortOrder = x.SortOrder,
                    IsClosed = x.IsClosed,
                    IsSuccessful = x.IsSuccessful
                })
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<BusinessRelationshipStageResponse>>(stages);
        }

        /// <summary>
        /// Hämtar stegförändringar för en affärsrelation i kronologisk ordning.
        /// </summary>
        public async Task<Result<IReadOnlyList<BusinessRelationshipStageHistoryResponse>>> GetStageHistoryAsync(
            Guid businessRelationshipId,
            CancellationToken cancellationToken = default)
        {
            var relationshipExists = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == businessRelationshipId, cancellationToken);

            if (!relationshipExists)
            {
                return Result.Fail<IReadOnlyList<BusinessRelationshipStageHistoryResponse>>(
                    new NotFoundError(nameof(BusinessRelationship), businessRelationshipId));
            }

            var history = await _db.Set<BusinessRelationshipStageHistory>()
                .AsNoTracking()
                .Where(x => x.BusinessRelationshipId == businessRelationshipId)
                .OrderBy(x => x.ChangedUtc)
                .ThenBy(x => x.Id)
                .Select(BusinessRelationshipProjections.StageHistory)
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<BusinessRelationshipStageHistoryResponse>>(history);
        }

        /// <summary>
        /// Validerar och skapar en ny affärsrelation med en första stage history-rad.
        /// </summary>
        public async Task<Result<BusinessRelationshipResponse>> CreateAsync(
            Guid owningOrganizationId,
            CreateBusinessRelationshipRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BusinessRelationshipResponse>(validationResult.Errors);
            }

            var referenceResult = await ValidateCreateReferencesAsync(
                owningOrganizationId,
                request.CounterpartyId,
                request.BusinessRelationshipTypeId,
                request.BusinessRelationshipStageId,
                cancellationToken);

            if (referenceResult.IsFailed)
            {
                return Result.Fail<BusinessRelationshipResponse>(referenceResult.Errors);
            }

            var duplicateExists = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.OwningOrganizationId == owningOrganizationId
                        && x.CounterpartyId == request.CounterpartyId
                        && x.BusinessRelationshipTypeId == request.BusinessRelationshipTypeId,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<BusinessRelationshipResponse>(
                    new ConflictError(
                        "A business relationship already exists for this owning organization, counterparty and relationship type."));
            }

            var stage = await _db.Set<BusinessRelationshipStage>()
                .AsNoTracking()
                .SingleAsync(x => x.Id == request.BusinessRelationshipStageId, cancellationToken);

            var firstObservedUtc = request.FirstObservedUtc ?? DateTimeOffset.UtcNow;

            var relationship = new BusinessRelationship
            {
                Id = Guid.CreateVersion7(),
                OwningOrganizationId = owningOrganizationId,
                CounterpartyId = request.CounterpartyId,
                BusinessRelationshipTypeId = request.BusinessRelationshipTypeId,
                BusinessRelationshipStageId = request.BusinessRelationshipStageId,
                Priority = request.Priority ?? RelationshipPriority.Normal,
                Score = request.Score,
                LeadSource = TrimToNull(request.LeadSource),
                OwnerUserId = request.OwnerUserId,
                FirstObservedUtc = firstObservedUtc,
                FirstContactedUtc = request.FirstContactedUtc,
                LastContactedUtc = request.LastContactedUtc,
                NextActionUtc = request.NextActionUtc,
                ClosedUtc = stage.IsClosed ? firstObservedUtc : null,
                Notes = TrimToNull(request.Notes)
            };

            var history = new BusinessRelationshipStageHistory
            {
                Id = Guid.CreateVersion7(),
                BusinessRelationshipId = relationship.Id,
                FromStageId = null,
                ToStageId = request.BusinessRelationshipStageId,
                ChangedUtc = firstObservedUtc
            };

            _db.Set<BusinessRelationship>().Add(relationship);
            _db.Set<BusinessRelationshipStageHistory>().Add(history);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Business relationship {BusinessRelationshipId} created between owning organization {OwningOrganizationId} and counterparty {CounterpartyId}. RelationshipTypeId={BusinessRelationshipTypeId}, StageId={BusinessRelationshipStageId}",
                relationship.Id,
                relationship.OwningOrganizationId,
                relationship.CounterpartyId,
                relationship.BusinessRelationshipTypeId,
                relationship.BusinessRelationshipStageId);

            return Result.Ok(await ProjectRequiredAsync(relationship.Id, cancellationToken));
        }

        /// <summary>
        /// Validerar och uppdaterar metadata utan att ändra relationens identitet eller steg.
        /// </summary>
        public async Task<Result<BusinessRelationshipResponse>> UpdateAsync(
            Guid businessRelationshipId,
            UpdateBusinessRelationshipRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BusinessRelationshipResponse>(validationResult.Errors);
            }

            var relationship = await _db.Set<BusinessRelationship>()
                .SingleOrDefaultAsync(x => x.Id == businessRelationshipId, cancellationToken);

            if (relationship is null)
            {
                return Result.Fail<BusinessRelationshipResponse>(
                    new NotFoundError(nameof(BusinessRelationship), businessRelationshipId));
            }

            relationship.Priority = request.Priority;
            relationship.Score = request.Score;
            relationship.LeadSource = TrimToNull(request.LeadSource);
            relationship.OwnerUserId = request.OwnerUserId;
            relationship.FirstContactedUtc = request.FirstContactedUtc;
            relationship.LastContactedUtc = request.LastContactedUtc;
            relationship.NextActionUtc = request.NextActionUtc;
            relationship.Notes = TrimToNull(request.Notes);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Business relationship {BusinessRelationshipId} metadata updated",
                relationship.Id);

            return Result.Ok(await ProjectRequiredAsync(relationship.Id, cancellationToken));
        }

        /// <summary>
        /// Byter steg och sparar förändringen i BusinessRelationshipStageHistory.
        /// </summary>
        public async Task<Result<BusinessRelationshipResponse>> ChangeStageAsync(
            Guid businessRelationshipId,
            ChangeBusinessRelationshipStageRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BusinessRelationshipResponse>(validationResult.Errors);
            }

            var relationship = await _db.Set<BusinessRelationship>()
                .SingleOrDefaultAsync(x => x.Id == businessRelationshipId, cancellationToken);

            if (relationship is null)
            {
                return Result.Fail<BusinessRelationshipResponse>(
                    new NotFoundError(nameof(BusinessRelationship), businessRelationshipId));
            }

            var targetStage = await _db.Set<BusinessRelationshipStage>()
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    x => x.Id == request.BusinessRelationshipStageId
                        && x.BusinessRelationshipTypeId == relationship.BusinessRelationshipTypeId,
                    cancellationToken);

            if (targetStage is null)
            {
                return Result.Fail<BusinessRelationshipResponse>(
                    new NotFoundError(
                        nameof(BusinessRelationshipStage),
                        request.BusinessRelationshipStageId));
            }

            if (relationship.BusinessRelationshipStageId == targetStage.Id)
            {
                return Result.Ok(await ProjectRequiredAsync(relationship.Id, cancellationToken));
            }

            var changedUtc = request.ChangedUtc ?? DateTimeOffset.UtcNow;

            if (changedUtc < relationship.FirstObservedUtc)
            {
                return Result.Fail<BusinessRelationshipResponse>(
                    new ConflictError(
                        "A business relationship stage cannot change before the relationship was first observed."));
            }

            var previousStageId = relationship.BusinessRelationshipStageId;

            relationship.BusinessRelationshipStageId = targetStage.Id;

            if (targetStage.IsClosed)
            {
                relationship.ClosedUtc ??= changedUtc;
                relationship.NextActionUtc = null;
            }
            else
            {
                relationship.ClosedUtc = null;
            }

            var history = new BusinessRelationshipStageHistory
            {
                Id = Guid.CreateVersion7(),
                BusinessRelationshipId = relationship.Id,
                FromStageId = previousStageId,
                ToStageId = targetStage.Id,
                ChangedUtc = changedUtc,
                ChangedByUserId = request.ChangedByUserId,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<BusinessRelationshipStageHistory>().Add(history);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Business relationship {BusinessRelationshipId} stage changed from {PreviousBusinessRelationshipStageId} to {BusinessRelationshipStageId} by user {ChangedByUserId}. IsClosed={IsClosed}",
                relationship.Id,
                previousStageId,
                targetStage.Id,
                request.ChangedByUserId,
                targetStage.IsClosed);

            return Result.Ok(await ProjectRequiredAsync(relationship.Id, cancellationToken));
        }

        /// <summary>
        /// Kontrollerar de relationer som krävs när en ny affärsrelation skapas.
        /// </summary>
        private async Task<Result> ValidateCreateReferencesAsync(
            Guid owningOrganizationId,
            Guid counterpartyId,
            int relationshipTypeId,
            int relationshipStageId,
            CancellationToken cancellationToken)
        {
            var owningOrganization = await _db.Set<Organization>()
                .AsNoTracking()
                .Where(x => x.Id == owningOrganizationId)
                .Select(x => new
                {
                    x.Id,
                    x.IsInternal
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (owningOrganization is null)
            {
                return Result.Fail(new NotFoundError(nameof(Organization), owningOrganizationId));
            }

            if (owningOrganization.IsInternal != true)
            {
                return Result.Fail(
                    new ConflictError(
                        "The owning organization must be an internal organization."));
            }

            if (owningOrganizationId == counterpartyId)
            {
                return Result.Fail(
                    new ConflictError(
                        "The owning organization cannot be its own counterparty."));
            }

            var counterpartyExists = await _db.Set<Party>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == counterpartyId, cancellationToken);

            if (!counterpartyExists)
            {
                return Result.Fail(new NotFoundError(nameof(Party), counterpartyId));
            }

            var relationshipType = await _db.Set<BusinessRelationshipType>()
                .AsNoTracking()
                .Where(x => x.Id == relationshipTypeId)
                .Select(x => new
                {
                    x.Id,
                    x.IsActive
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (relationshipType is null)
            {
                return Result.Fail(
                    new NotFoundError(nameof(BusinessRelationshipType), relationshipTypeId));
            }

            if (!relationshipType.IsActive)
            {
                return Result.Fail(
                    new ConflictError(
                        "An inactive business relationship type cannot be used for a new relationship."));
            }

            var stageExists = await _db.Set<BusinessRelationshipStage>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == relationshipStageId
                        && x.BusinessRelationshipTypeId == relationshipTypeId,
                    cancellationToken);

            if (!stageExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "The selected business relationship stage does not belong to the selected relationship type."));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Projekterar en affärsrelation direkt från databasen.
        /// </summary>
        private async Task<BusinessRelationshipResponse?> ProjectAsync(
            Guid businessRelationshipId,
            CancellationToken cancellationToken)
        {
            return await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .Where(x => x.Id == businessRelationshipId)
                .Select(BusinessRelationshipProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad affärsrelation och kastar om den oväntat saknas.
        /// </summary>
        private async Task<BusinessRelationshipResponse> ProjectRequiredAsync(
            Guid businessRelationshipId,
            CancellationToken cancellationToken)
        {
            return await ProjectAsync(businessRelationshipId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"BusinessRelationship '{businessRelationshipId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Lägger på stabil sortering innan paginering utförs.
        /// </summary>
        private static IQueryable<BusinessRelationship> ApplySorting(
            IQueryable<BusinessRelationship> query,
            BusinessRelationshipSortField sortBy,
            SortDirection sortDirection)
        {
            return (sortBy, sortDirection) switch
            {
                (BusinessRelationshipSortField.CounterpartyName, SortDirection.Descending) =>
                    query.OrderByDescending(x => x.Counterparty.Name).ThenByDescending(x => x.Id),

                (BusinessRelationshipSortField.FirstObservedUtc, SortDirection.Ascending) =>
                    query.OrderBy(x => x.FirstObservedUtc).ThenBy(x => x.Id),

                (BusinessRelationshipSortField.FirstObservedUtc, SortDirection.Descending) =>
                    query.OrderByDescending(x => x.FirstObservedUtc).ThenByDescending(x => x.Id),

                (BusinessRelationshipSortField.LastContactedUtc, SortDirection.Ascending) =>
                    query.OrderBy(x => x.LastContactedUtc).ThenBy(x => x.Id),

                (BusinessRelationshipSortField.LastContactedUtc, SortDirection.Descending) =>
                    query.OrderByDescending(x => x.LastContactedUtc).ThenByDescending(x => x.Id),

                (BusinessRelationshipSortField.NextActionUtc, SortDirection.Ascending) =>
                    query.OrderBy(x => x.NextActionUtc).ThenBy(x => x.Id),

                (BusinessRelationshipSortField.NextActionUtc, SortDirection.Descending) =>
                    query.OrderByDescending(x => x.NextActionUtc).ThenByDescending(x => x.Id),

                (BusinessRelationshipSortField.Score, SortDirection.Ascending) =>
                    query.OrderBy(x => x.Score).ThenBy(x => x.Id),

                (BusinessRelationshipSortField.Score, SortDirection.Descending) =>
                    query.OrderByDescending(x => x.Score).ThenByDescending(x => x.Id),

                _ =>
                    query.OrderBy(x => x.Counterparty.Name).ThenBy(x => x.Id)
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