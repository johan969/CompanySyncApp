using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Hanterar CRM-aktiviteter och deras deltagare.
    /// </summary>
    public sealed class CrmActivityService : ICrmActivityService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;
        private readonly ILogger<CrmActivityService> _logger;

        /// <summary>
        /// Skapar CRM-aktivitetsservicen med databas- och valideringsberoenden.
        /// </summary>
        public CrmActivityService(Arb_IntDbContext db, IValidationService validationService, ILogger<CrmActivityService> logger)
        {
            _db = db;
            _validationService = validationService;
            _logger = logger;
        }

        /// <summary>
        /// Hämtar en CRM-aktivitet direkt som ett projekterat response-objekt.
        /// </summary>
        public async Task<Result<CrmActivityResponse>> GetByIdAsync(Guid crmActivityId, CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(crmActivityId, cancellationToken);

            if (response is null)
            {
                return Result.Fail<CrmActivityResponse>(
                    new NotFoundError(nameof(CrmActivity), crmActivityId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar CRM-aktiviteter.
        /// </summary>
        public async Task<Result<PagedResult<CrmActivityListResponse>>> QueryAsync(
            CrmActivityQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<CrmActivityListResponse>>(validationResult.Errors);
            }

            IQueryable<CrmActivity> query = _db.Set<CrmActivity>().AsNoTracking();

            if (request.RegardingPartyId.HasValue)
            {
                query = query.Where(x => x.RegardingPartyId == request.RegardingPartyId.Value);
            }

            if (request.BusinessRelationshipId.HasValue)
            {
                query = query.Where(x => x.BusinessRelationshipId == request.BusinessRelationshipId.Value);
            }

            if (request.ActivityType.HasValue)
            {
                query = query.Where(x => x.ActivityType == request.ActivityType.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            if (request.Direction.HasValue)
            {
                query = query.Where(x => x.Direction == request.Direction.Value);
            }

            if (request.OwnerUserId.HasValue)
            {
                query = query.Where(x => x.OwnerUserId == request.OwnerUserId.Value);
            }

            if (request.OccurredFromUtc.HasValue)
            {
                query = query.Where(x => x.OccurredUtc >= request.OccurredFromUtc.Value);
            }

            if (request.OccurredToUtc.HasValue)
            {
                query = query.Where(x => x.OccurredUtc <= request.OccurredToUtc.Value);
            }

            if (request.DueBeforeUtc.HasValue)
            {
                query = query.Where(x => x.DueUtc != null && x.DueUtc <= request.DueBeforeUtc.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchPattern = $"%{request.Search.Trim()}%";

                query = query.Where(x =>
                    EF.Functions.ILike(x.Subject, searchPattern)
                    || EF.Functions.ILike(x.Description, searchPattern)
                    || (x.ExternalSystem != null && EF.Functions.ILike(x.ExternalSystem, searchPattern))
                    || (x.ExternalRecordId != null && EF.Functions.ILike(x.ExternalRecordId, searchPattern)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(CrmActivityProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(new PagedResult<CrmActivityListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
        }

        /// <summary>
        /// Validerar kontext och extern identitet och skapar en CRM-aktivitet.
        /// </summary>
        public async Task<Result<CrmActivityResponse>> CreateAsync(
            CreateCrmActivityRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<CrmActivityResponse>(validationResult.Errors);
            }

            var contextResult = await ValidateActivityContextAsync(
                request.RegardingPartyId,
                request.BusinessRelationshipId,
                cancellationToken);

            if (contextResult.IsFailed)
            {
                return Result.Fail<CrmActivityResponse>(contextResult.Errors);
            }

            var externalReferenceResult = await ValidateExternalReferenceAsync(
                request.ExternalSystem,
                request.ExternalRecordId,
                excludeActivityId: null,
                cancellationToken);

            if (externalReferenceResult.IsFailed)
            {
                return Result.Fail<CrmActivityResponse>(externalReferenceResult.Errors);
            }

            var activity = new CrmActivity
            {
                Id = Guid.CreateVersion7(),
                RegardingPartyId = request.RegardingPartyId,
                BusinessRelationshipId = request.BusinessRelationshipId,
                ActivityType = request.ActivityType,
                Status = request.Status,
                Direction = request.Direction,
                Subject = request.Subject.Trim(),
                Description = request.Description.Trim(),
                OccurredUtc = request.OccurredUtc,
                DueUtc = request.DueUtc,
                CompletedUtc = request.Status == CrmActivityStatus.Completed ? request.CompletedUtc ?? DateTimeOffset.UtcNow : null,
                OwnerUserId = request.OwnerUserId,
                ExternalSystem = TrimToNull(request.ExternalSystem),
                ExternalRecordId = TrimToNull(request.ExternalRecordId)
            };

            _db.Set<CrmActivity>().Add(activity);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "CRM activity {CrmActivityId} created for party {PartyId}. ActivityType={ActivityType}, Status={Status}, BusinessRelationshipId={BusinessRelationshipId}",
                activity.Id,
                activity.RegardingPartyId,
                activity.ActivityType,
                activity.Status,
                activity.BusinessRelationshipId);

            return Result.Ok(await ProjectRequiredAsync(activity.Id, cancellationToken));
        }

        /// <summary>
        /// Uppdaterar aktivitetens metadata utan att ändra dess Party- eller BusinessRelationship-kontext.
        /// </summary>
        public async Task<Result<CrmActivityResponse>> UpdateAsync(
            Guid crmActivityId,
            UpdateCrmActivityRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<CrmActivityResponse>(validationResult.Errors);
            }

            var activity = await _db.Set<CrmActivity>()
                .SingleOrDefaultAsync(x => x.Id == crmActivityId, cancellationToken);

            if (activity is null)
            {
                return Result.Fail<CrmActivityResponse>(
                    new NotFoundError(nameof(CrmActivity), crmActivityId));
            }

            var externalReferenceResult = await ValidateExternalReferenceAsync(
                request.ExternalSystem,
                request.ExternalRecordId,
                activity.Id,
                cancellationToken);

            if (externalReferenceResult.IsFailed)
            {
                return Result.Fail<CrmActivityResponse>(externalReferenceResult.Errors);
            }

            activity.ActivityType = request.ActivityType;
            activity.Direction = request.Direction;
            activity.Subject = request.Subject.Trim();
            activity.Description = request.Description.Trim();
            activity.OccurredUtc = request.OccurredUtc;
            activity.DueUtc = request.DueUtc;
            activity.OwnerUserId = request.OwnerUserId;
            activity.ExternalSystem = TrimToNull(request.ExternalSystem);
            activity.ExternalRecordId = TrimToNull(request.ExternalRecordId);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "CRM activity {CrmActivityId} metadata updated",
                activity.Id);

            return Result.Ok(await ProjectRequiredAsync(activity.Id, cancellationToken));
        }

        /// <summary>
        /// Ändrar aktivitetens status och håller CompletedUtc synkroniserad med Completed.
        /// </summary>
        public async Task<Result<CrmActivityResponse>> ChangeStatusAsync(
            Guid crmActivityId,
            ChangeCrmActivityStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<CrmActivityResponse>(validationResult.Errors);
            }

            var activity = await _db.Set<CrmActivity>()
                .SingleOrDefaultAsync(x => x.Id == crmActivityId, cancellationToken);

            if (activity is null)
            {
                return Result.Fail<CrmActivityResponse>(
                    new NotFoundError(nameof(CrmActivity), crmActivityId));
            }

            var previousStatus = activity.Status;

            activity.Status = request.Status;
            activity.CompletedUtc = request.Status == CrmActivityStatus.Completed
                ? request.CompletedUtc ?? DateTimeOffset.UtcNow
                : null;

            await _db.SaveChangesAsync(cancellationToken);

            if (previousStatus != activity.Status)
            {
                _logger.LogInformation(
                    "CRM activity {CrmActivityId} status changed from {PreviousStatus} to {NewStatus}",
                    activity.Id,
                    previousStatus,
                    activity.Status);
            }

            return Result.Ok(await ProjectRequiredAsync(activity.Id, cancellationToken));
        }

        /// <summary>
        /// Lägger till en Party som deltagare i aktiviteten.
        /// </summary>
        public async Task<Result<CrmActivityResponse>> AddParticipantAsync(
            Guid crmActivityId,
            CreateCrmActivityParticipantRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<CrmActivityResponse>(validationResult.Errors);
            }

            var activityExists = await _db.Set<CrmActivity>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == crmActivityId, cancellationToken);

            if (!activityExists)
            {
                return Result.Fail<CrmActivityResponse>(
                    new NotFoundError(nameof(CrmActivity), crmActivityId));
            }

            var partyExists = await _db.Set<Party>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.PartyId, cancellationToken);

            if (!partyExists)
            {
                return Result.Fail<CrmActivityResponse>(
                    new NotFoundError(nameof(Party), request.PartyId));
            }

            var duplicateExists = await _db.Set<CrmActivityParticipant>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.CrmActivityId == crmActivityId
                        && x.PartyId == request.PartyId
                        && x.Role == request.Role,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<CrmActivityResponse>(
                    new ConflictError(
                        "The party already has this participant role on the CRM activity."));
            }

            var participant = new CrmActivityParticipant
            {
                Id = Guid.CreateVersion7(),
                CrmActivityId = crmActivityId,
                PartyId = request.PartyId,
                Role = request.Role
            };

            _db.Set<CrmActivityParticipant>().Add(participant);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Party {PartyId} added to CRM activity {CrmActivityId} as participant {CrmActivityParticipantId} with role {ParticipantRole}",
                participant.PartyId,
                participant.CrmActivityId,
                participant.Id,
                participant.Role);

            return Result.Ok(await ProjectRequiredAsync(crmActivityId, cancellationToken));
        }

        /// <summary>
        /// Ändrar rollen för en befintlig aktivitetsdeltagare.
        /// </summary>
        public async Task<Result<CrmActivityResponse>> UpdateParticipantAsync(
            Guid participantId,
            UpdateCrmActivityParticipantRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<CrmActivityResponse>(validationResult.Errors);
            }

            var participant = await _db.Set<CrmActivityParticipant>()
                .SingleOrDefaultAsync(x => x.Id == participantId, cancellationToken);

            if (participant is null)
            {
                return Result.Fail<CrmActivityResponse>(
                    new NotFoundError(nameof(CrmActivityParticipant), participantId));
            }

            var duplicateExists = await _db.Set<CrmActivityParticipant>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id != participant.Id
                        && x.CrmActivityId == participant.CrmActivityId
                        && x.PartyId == participant.PartyId
                        && x.Role == request.Role,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<CrmActivityResponse>(
                    new ConflictError(
                        "The party already has this participant role on the CRM activity."));
            }

            var previousRole = participant.Role;
            participant.Role = request.Role;

            await _db.SaveChangesAsync(cancellationToken);

            if (previousRole != participant.Role)
            {
                _logger.LogInformation(
                    "CRM activity participant {CrmActivityParticipantId} on activity {CrmActivityId} changed role from {PreviousParticipantRole} to {ParticipantRole}. PartyId={PartyId}",
                    participant.Id,
                    participant.CrmActivityId,
                    previousRole,
                    participant.Role,
                    participant.PartyId);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    participant.CrmActivityId,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort en deltagarkoppling utan att påverka deltagande Party.
        /// </summary>
        public async Task<Result<CrmActivityResponse>> DeleteParticipantAsync(
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            var participant = await _db.Set<CrmActivityParticipant>()
                .SingleOrDefaultAsync(x => x.Id == participantId, cancellationToken);

            if (participant is null)
            {
                return Result.Fail<CrmActivityResponse>(
                    new NotFoundError(nameof(CrmActivityParticipant), participantId));
            }

            var activityId = participant.CrmActivityId;
            var partyId = participant.PartyId;
            var participantRole = participant.Role;

            _db.Set<CrmActivityParticipant>().Remove(participant);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "CRM activity participant {CrmActivityParticipantId} removed from activity {CrmActivityId}. PartyId={PartyId}, ParticipantRole={ParticipantRole}",
                participant.Id,
                activityId,
                partyId,
                participantRole);

            return Result.Ok(await ProjectRequiredAsync(activityId, cancellationToken));
        }

        /// <summary>
        /// Kontrollerar att RegardingParty finns och att eventuell affärsrelation gäller samma motpart.
        /// </summary>
        private async Task<Result> ValidateActivityContextAsync(
            Guid regardingPartyId,
            Guid? businessRelationshipId,
            CancellationToken cancellationToken)
        {
            var partyExists = await _db.Set<Party>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == regardingPartyId, cancellationToken);

            if (!partyExists)
            {
                return Result.Fail(
                    new NotFoundError(nameof(Party), regardingPartyId));
            }

            if (!businessRelationshipId.HasValue)
            {
                return Result.Ok();
            }

            var relationship = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .Where(x => x.Id == businessRelationshipId.Value)
                .Select(x => new
                {
                    x.CounterpartyId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (relationship is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(BusinessRelationship),
                        businessRelationshipId.Value));
            }

            if (relationship.CounterpartyId != regardingPartyId)
            {
                return Result.Fail(
                    new ConflictError(
                        "The CRM activity regarding party must match the business relationship counterparty."));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar att eventuell extern aktivitetidentitet inte redan används av en annan aktivitet.
        /// </summary>
        private async Task<Result> ValidateExternalReferenceAsync(
            string? externalSystem,
            string? externalRecordId,
            Guid? excludeActivityId,
            CancellationToken cancellationToken)
        {
            var system = TrimToNull(externalSystem);
            var recordId = TrimToNull(externalRecordId);

            if (system is null && recordId is null)
            {
                return Result.Ok();
            }

            IQueryable<CrmActivity> query = _db.Set<CrmActivity>()
                .AsNoTracking()
                .Where(x =>
                    x.ExternalSystem == system
                    && x.ExternalRecordId == recordId);

            if (excludeActivityId.HasValue)
            {
                query = query.Where(x => x.Id != excludeActivityId.Value);
            }

            if (!await query.AnyAsync(cancellationToken))
            {
                return Result.Ok();
            }

            return Result.Fail(
                new ConflictError(
                    "A CRM activity with the same external system and record identifier already exists."));
        }

        /// <summary>
        /// Projekterar en CRM-aktivitet direkt från databasen.
        /// </summary>
        private async Task<CrmActivityResponse?> ProjectAsync(
            Guid crmActivityId,
            CancellationToken cancellationToken)
        {
            return await _db.Set<CrmActivity>()
                .AsNoTracking()
                .Where(x => x.Id == crmActivityId)
                .Select(CrmActivityProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad CRM-aktivitet och kastar om den oväntat saknas.
        /// </summary>
        private async Task<CrmActivityResponse> ProjectRequiredAsync(
            Guid crmActivityId,
            CancellationToken cancellationToken)
        {
            return await ProjectAsync(crmActivityId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"CrmActivity '{crmActivityId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Lägger på stabil sortering innan paginering utförs.
        /// </summary>
        private static IQueryable<CrmActivity> ApplySorting(
            IQueryable<CrmActivity> query,
            CrmActivitySortField sortBy,
            SortDirection sortDirection)
        {
            return (sortBy, sortDirection) switch
            {
                (CrmActivitySortField.OccurredUtc, SortDirection.Ascending) =>
                    query.OrderBy(x => x.OccurredUtc).ThenBy(x => x.Id),

                (CrmActivitySortField.OccurredUtc, SortDirection.Descending) =>
                    query.OrderByDescending(x => x.OccurredUtc).ThenByDescending(x => x.Id),

                (CrmActivitySortField.DueUtc, SortDirection.Ascending) =>
                    query.OrderBy(x => x.DueUtc).ThenBy(x => x.Id),

                (CrmActivitySortField.DueUtc, SortDirection.Descending) =>
                    query.OrderByDescending(x => x.DueUtc).ThenByDescending(x => x.Id),

                (CrmActivitySortField.Subject, SortDirection.Ascending) =>
                    query.OrderBy(x => x.Subject).ThenBy(x => x.Id),

                (CrmActivitySortField.Subject, SortDirection.Descending) =>
                    query.OrderByDescending(x => x.Subject).ThenByDescending(x => x.Id),

                (CrmActivitySortField.CreateDateUtc, SortDirection.Ascending) =>
                    query.OrderBy(x => x.CreateDateUtc).ThenBy(x => x.Id),

                (CrmActivitySortField.CreateDateUtc, SortDirection.Descending) =>
                    query.OrderByDescending(x => x.CreateDateUtc).ThenByDescending(x => x.Id),

                _ =>
                    query.OrderByDescending(x => x.OccurredUtc).ThenByDescending(x => x.Id)
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