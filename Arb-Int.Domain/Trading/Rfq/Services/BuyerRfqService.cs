using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Trading.Snapshots;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar BuyerRfq-roten och dess revisionslivscykel.
/// </summary>
public sealed class BuyerRfqService : IBuyerRfqService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly IBuyerRfqRevisionSnapshotService _snapshotService;
    private readonly ILogger<BuyerRfqService> _logger;

    /// <summary>
    /// Skapar RFQ-servicen med databas-, validerings- och snapshotberoenden.
    /// </summary>
    public BuyerRfqService(
        Arb_IntDbContext db,
        IValidationService validationService,
        IBuyerRfqRevisionSnapshotService snapshotService,
        ILogger<BuyerRfqService> logger)
    {
        _db = db;
        _validationService = validationService;
        _snapshotService = snapshotService;
        _logger = logger;
    }

    /// <summary>
    /// Hämtar en BuyerRfq med dess revisionsmetadata.
    /// </summary>
    public async Task<Result<BuyerRfqResponse>> GetByIdAsync(Guid buyerRfqId, CancellationToken cancellationToken = default)
    {
        var response = await ProjectAsync(buyerRfqId, cancellationToken);

        if (response is null)
        {
            return Result.Fail<BuyerRfqResponse>(
                new NotFoundError(nameof(BuyerRfq), buyerRfqId));
        }

        return Result.Ok(response);
    }

    /// <summary>
    /// Hämtar en enskild RFQ-revision.
    /// </summary>
    public async Task<Result<BuyerRfqRevisionResponse>> GetRevisionByIdAsync(
        Guid buyerRfqRevisionId,
        CancellationToken cancellationToken = default)
    {
        var response = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqRevisionId)
            .Select(BuyerRfqProjections.Revision)
            .SingleOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            return Result.Fail<BuyerRfqRevisionResponse>(
                new NotFoundError(nameof(BuyerRfqRevision), buyerRfqRevisionId));
        }

        return Result.Ok(response);
    }

    /// <summary>
    /// Söker RFQ:er som ägs av en viss intern organisation.
    /// </summary>
    public async Task<Result<PagedResult<BuyerRfqListResponse>>> QueryAsync(
        Guid owningOrganizationId,
        BuyerRfqQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<PagedResult<BuyerRfqListResponse>>(validationResult.Errors);
        }

        var organizationExists = await _db.Set<Organization>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == owningOrganizationId, cancellationToken);

        if (!organizationExists)
        {
            return Result.Fail<PagedResult<BuyerRfqListResponse>>(
                new NotFoundError(nameof(Organization), owningOrganizationId));
        }

        IQueryable<BuyerRfq> query = _db.Set<BuyerRfq>()
            .AsNoTracking()
            .Where(x => x.BuyerBusinessRelationship.OwningOrganizationId == owningOrganizationId);

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(x => x.Priority == request.Priority.Value);
        }

        if (request.Visibility.HasValue)
        {
            query = query.Where(x => x.Visibility == request.Visibility.Value);
        }

        if (request.OwnerUserId.HasValue)
        {
            query = query.Where(x => x.OwnerUserId == request.OwnerUserId.Value);
        }

        if (request.BuyerPartyId.HasValue)
        {
            query = query.Where(x => x.BuyerBusinessRelationship.CounterpartyId == request.BuyerPartyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern = $"%{request.Search.Trim()}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.RfqNumber, searchPattern)
                || EF.Functions.ILike(x.Name, searchPattern)
                || EF.Functions.ILike(x.BuyerBusinessRelationship.Counterparty.Name, searchPattern)
                || (x.BuyerReference != null && EF.Functions.ILike(x.BuyerReference, searchPattern)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, request.SortBy, request.SortDirection);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(BuyerRfqProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok(
            new PagedResult<BuyerRfqListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }

    /// <summary>
    /// Skapar en BuyerRfq i Draft med revision 1 i Draft.
    /// </summary>
    public async Task<Result<BuyerRfqResponse>> CreateAsync(
        Guid buyerBusinessRelationshipId,
        CreateBuyerRfqRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(validationResult.Errors);
        }

        var relationshipResult = await ValidateBuyerRelationshipAsync(
            buyerBusinessRelationshipId,
            cancellationToken);

        if (relationshipResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(relationshipResult.Errors);
        }

        var rfqNumber = request.RfqNumber.Trim();

        var rfqNumberExists = await _db.Set<BuyerRfq>()
            .AsNoTracking()
            .AnyAsync(x => x.RfqNumber == rfqNumber, cancellationToken);

        if (rfqNumberExists)
        {
            return Result.Fail<BuyerRfqResponse>(
                new ConflictError($"BuyerRfq number '{rfqNumber}' already exists."));
        }

        var rfq = new BuyerRfq
        {
            Id = Guid.CreateVersion7(),
            BuyerBusinessRelationshipId = buyerBusinessRelationshipId,
            BuyerDemandId = null,
            RfqNumber = rfqNumber,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Status = BuyerRfqStatus.Draft,
            Priority = request.Priority,
            Visibility = request.Visibility,
            BuyerReference = TrimToNull(request.BuyerReference),
            OwnerUserId = request.OwnerUserId,
            LatestRevisionNumber = 1,
            OpenedAtUtc = null,
            ClosedAtUtc = null,
            InternalNotes = TrimToNull(request.InternalNotes)
        };

        var revision = CreateInitialRevision(
            rfq.Id,
            request.RevisionSummary,
            request.BuyerOriginalText,
            request.InternalInterpretation,
            request.ResponseRequestedByUtc);

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        _db.Set<BuyerRfq>().Add(rfq);
        _db.Set<BuyerRfqRevision>().Add(revision);

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Buyer RFQ {BuyerRfqId} revision {BuyerRfqRevisionId} created in Draft for business relationship {BusinessRelationshipId}",
            rfq.Id,
            revision.Id,
            buyerBusinessRelationshipId);

        return Result.Ok(await ProjectRequiredAsync(rfq.Id, cancellationToken));
    }

    /// <summary>
    /// Konverterar en BuyerDemand till en draft-RFQ och historiserar demandens statusändring.
    /// </summary>
    public async Task<Result<BuyerRfqResponse>> CreateFromDemandAsync(
        Guid buyerDemandId,
        CreateBuyerRfqFromDemandRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(validationResult.Errors);
        }

        var demand = await _db.Set<BuyerDemand>()
            .Include(x => x.BusinessRelationship)
                .ThenInclude(x => x.BusinessRelationshipType)
            .SingleOrDefaultAsync(x => x.Id == buyerDemandId, cancellationToken);

        if (demand is null)
        {
            return Result.Fail<BuyerRfqResponse>(
                new NotFoundError(nameof(BuyerDemand), buyerDemandId));
        }

        if (demand.BusinessRelationship.BusinessRelationshipType.Code != BusinessRelationshipTypeCodes.Buyer)
        {
            return Result.Fail<BuyerRfqResponse>(
                new ConflictError(
                    "BuyerRfq can only be created from a BuyerDemand belonging to a buyer relationship."));
        }

        if (demand.RowVersion != request.BuyerDemandRowVersion)
        {
            return Result.Fail<BuyerRfqResponse>(
                new ConcurrencyError(nameof(BuyerDemand), demand.Id));
        }

        var rfqNumber = request.RfqNumber.Trim();

        var rfqNumberExists = await _db.Set<BuyerRfq>()
            .AsNoTracking()
            .AnyAsync(x => x.RfqNumber == rfqNumber, cancellationToken);

        if (rfqNumberExists)
        {
            return Result.Fail<BuyerRfqResponse>(
                new ConflictError($"BuyerRfq number '{rfqNumber}' already exists."));
        }

        var changedAtUtc = DateTimeOffset.UtcNow;

        var rfq = new BuyerRfq
        {
            Id = Guid.CreateVersion7(),
            BuyerBusinessRelationshipId = demand.BusinessRelationshipId,
            BuyerDemandId = demand.Id,
            RfqNumber = rfqNumber,
            Name = TrimToNull(request.Name) ?? demand.Name,
            Description = TrimToNull(request.Description) ?? demand.Description,
            Status = BuyerRfqStatus.Draft,
            Priority = request.Priority,
            Visibility = request.Visibility,
            BuyerReference = TrimToNull(request.BuyerReference),
            OwnerUserId = request.OwnerUserId ?? demand.OwnerUserId,
            LatestRevisionNumber = 1,
            OpenedAtUtc = null,
            ClosedAtUtc = null,
            InternalNotes = TrimToNull(request.InternalNotes)
        };

        var revision = CreateInitialRevision(
            rfq.Id,
            request.RevisionSummary,
            request.BuyerOriginalText,
            request.InternalInterpretation,
            request.ResponseRequestedByUtc);

        var shouldChangeDemandStatus = demand.Status != BuyerDemandStatus.ConvertedToRfq;

        if (shouldChangeDemandStatus)
        {
            var stateHistory = new BuyerDemandStateHistory
            {
                Id = Guid.CreateVersion7(),
                BuyerDemandId = demand.Id,
                FromKnowledgeLevel = demand.KnowledgeLevel,
                ToKnowledgeLevel = demand.KnowledgeLevel,
                FromStatus = demand.Status,
                ToStatus = BuyerDemandStatus.ConvertedToRfq,
                Confidence = demand.Confidence,
                ChangedAtUtc = changedAtUtc,
                ChangedByUserId = request.ConvertedByUserId,
                Notes = TrimToNull(request.ConversionNotes)
            };

            demand.Status = BuyerDemandStatus.ConvertedToRfq;

            _db.Set<BuyerDemandStateHistory>().Add(stateHistory);
        }

        _db.Entry(demand)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerDemandRowVersion;

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        _db.Set<BuyerRfq>().Add(rfq);
        _db.Set<BuyerRfqRevision>().Add(revision);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            _logger.LogWarning(
                exception,
                "Concurrency conflict while converting Buyer demand {BuyerDemandId} to Buyer RFQ",
                demand.Id);

            return Result.Fail<BuyerRfqResponse>(
                new ConcurrencyError(nameof(BuyerDemand), demand.Id));
        }

        _logger.LogInformation(
            "Buyer demand {BuyerDemandId} converted to Buyer RFQ {BuyerRfqId} revision {BuyerRfqRevisionId} for business relationship {BusinessRelationshipId}. DemandStatusChanged={DemandStatusChanged}",
            demand.Id,
            rfq.Id,
            revision.Id,
            demand.BusinessRelationshipId,
            shouldChangeDemandStatus);

        return Result.Ok(await ProjectRequiredAsync(rfq.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar RFQ-rotens metadata med optimistic concurrency.
    /// </summary>
    public async Task<Result<BuyerRfqResponse>> UpdateAsync(
        Guid buyerRfqId,
        UpdateBuyerRfqRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(validationResult.Errors);
        }

        var rfq = await _db.Set<BuyerRfq>()
            .SingleOrDefaultAsync(x => x.Id == buyerRfqId, cancellationToken);

        if (rfq is null)
        {
            return Result.Fail<BuyerRfqResponse>(
                new NotFoundError(nameof(BuyerRfq), buyerRfqId));
        }

        rfq.Name = request.Name.Trim();
        rfq.Description = request.Description.Trim();
        rfq.Priority = request.Priority;
        rfq.Visibility = request.Visibility;
        rfq.BuyerReference = TrimToNull(request.BuyerReference);
        rfq.OwnerUserId = request.OwnerUserId;
        rfq.InternalNotes = TrimToNull(request.InternalNotes);

        var saveResult = await SaveWithConcurrencyAsync(
            rfq,
            rfq.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(saveResult.Errors);
        }

        _logger.LogInformation(
            "Buyer RFQ {BuyerRfqId} metadata updated",
            rfq.Id);

        return Result.Ok(await ProjectRequiredAsync(rfq.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar metadata på en revision så länge den fortfarande är Draft.
    /// </summary>
    public async Task<Result<BuyerRfqRevisionResponse>> UpdateRevisionAsync(
        Guid buyerRfqRevisionId,
        UpdateBuyerRfqRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRevisionResponse>(validationResult.Errors);
        }

        var revision = await _db.Set<BuyerRfqRevision>()
            .SingleOrDefaultAsync(x => x.Id == buyerRfqRevisionId, cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerRfqRevisionResponse>(
                new NotFoundError(nameof(BuyerRfqRevision), buyerRfqRevisionId));
        }

        var editableResult = ValidateRevisionIsEditable(revision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRevisionResponse>(editableResult.Errors);
        }

        revision.Summary = TrimToNull(request.Summary);
        revision.BuyerOriginalText = TrimToNull(request.BuyerOriginalText);
        revision.InternalInterpretation = TrimToNull(request.InternalInterpretation);
        revision.ResponseRequestedByUtc = request.ResponseRequestedByUtc;

        var saveResult = await SaveWithConcurrencyAsync(
            revision,
            revision.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRevisionResponse>(saveResult.Errors);
        }

        _logger.LogInformation(
            "Buyer RFQ revision {BuyerRfqRevisionId} metadata updated",
            revision.Id);

        return Result.Ok(
            await ProjectRevisionRequiredAsync(
                revision.Id,
                cancellationToken));
    }

    /// <summary>
    /// Ändrar RFQ:ns root-status enligt den definierade livscykeln.
    /// </summary>
    public async Task<Result<BuyerRfqResponse>> ChangeStatusAsync(
        Guid buyerRfqId,
        ChangeBuyerRfqStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(validationResult.Errors);
        }

        var rfq = await _db.Set<BuyerRfq>()
            .SingleOrDefaultAsync(x => x.Id == buyerRfqId, cancellationToken);

        if (rfq is null)
        {
            return Result.Fail<BuyerRfqResponse>(
                new NotFoundError(nameof(BuyerRfq), buyerRfqId));
        }

        if (rfq.Status == request.Status)
        {
            return Result.Ok(await ProjectRequiredAsync(rfq.Id, cancellationToken));
        }

        var transitionResult = ValidateStatusTransition(
            rfq.Status,
            request.Status);

        if (transitionResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(transitionResult.Errors);
        }

        var previousStatus = rfq.Status;
        var changedAtUtc = request.ChangedAtUtc ?? DateTimeOffset.UtcNow;

        rfq.Status = request.Status;

        if (request.Status == BuyerRfqStatus.Active)
        {
            rfq.OpenedAtUtc ??= changedAtUtc;
            rfq.ClosedAtUtc = null;
        }

        if (request.Status is BuyerRfqStatus.Closed or BuyerRfqStatus.Cancelled)
        {
            rfq.ClosedAtUtc = changedAtUtc;
        }

        var saveResult = await SaveWithConcurrencyAsync(
            rfq,
            rfq.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(saveResult.Errors);
        }

        _logger.LogInformation(
            "Buyer RFQ {BuyerRfqId} status changed from {PreviousStatus} to {NewStatus}",
            rfq.Id,
            previousStatus,
            rfq.Status);

        return Result.Ok(await ProjectRequiredAsync(rfq.Id, cancellationToken));
    }

    /// <summary>
    /// Avbryter en revision som fortfarande är Draft.
    /// </summary>
    public async Task<Result<BuyerRfqRevisionResponse>> CancelRevisionAsync(
        Guid buyerRfqRevisionId,
        CancelBuyerRfqRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRevisionResponse>(validationResult.Errors);
        }

        var revision = await _db.Set<BuyerRfqRevision>()
            .SingleOrDefaultAsync(x => x.Id == buyerRfqRevisionId, cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerRfqRevisionResponse>(
                new NotFoundError(nameof(BuyerRfqRevision), buyerRfqRevisionId));
        }

        if (revision.Status != BuyerRfqRevisionStatus.Draft)
        {
            return Result.Fail<BuyerRfqRevisionResponse>(
                new ConflictError(
                    $"BuyerRfqRevision with status '{revision.Status}' cannot be cancelled through the draft cancellation operation."));
        }

        revision.Status = BuyerRfqRevisionStatus.Cancelled;

        var saveResult = await SaveWithConcurrencyAsync(
            revision,
            revision.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRevisionResponse>(saveResult.Errors);
        }

        _logger.LogInformation(
            "Buyer RFQ revision {BuyerRfqRevisionId} cancelled",
            revision.Id);

        return Result.Ok(
            await ProjectRevisionRequiredAsync(
                revision.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kontrollerar revisionsstatus, root-status och serviceinvariants inför release.
    /// </summary>
    public async Task<Result<BuyerRfqRevisionReleaseReadinessResponse>> GetReleaseReadinessAsync(
        Guid buyerRfqRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revision = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Include(x => x.BuyerRfq)
            .Include(x => x.Lines)
                .ThenInclude(x => x.MaterialOptions)
            .AsSplitQuery()
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqRevisionId,
                cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerRfqRevisionReleaseReadinessResponse>(
                new NotFoundError(
                    nameof(BuyerRfqRevision),
                    buyerRfqRevisionId));
        }

        var issues = new List<BuyerRfqReleaseIssueResponse>();

        if (revision.Status != BuyerRfqRevisionStatus.Draft)
        {
            issues.Add(
                CreateReleaseIssue(
                    "revision.not-draft",
                    $"Revisionen har status '{revision.Status}' och kan därför inte släppas."));
        }

        if (revision.BuyerRfq.Status == BuyerRfqStatus.OnHold)
        {
            issues.Add(
                CreateReleaseIssue(
                    "rfq.on-hold",
                    "RFQ:n är OnHold och måste återaktiveras innan en ny revision kan släppas."));
        }

        if (revision.BuyerRfq.Status is BuyerRfqStatus.Closed or BuyerRfqStatus.Cancelled)
        {
            issues.Add(
                CreateReleaseIssue(
                    "rfq.terminal",
                    $"RFQ:n har terminal status '{revision.BuyerRfq.Status}'."));
        }

        if (revision.RevisionNumber != revision.BuyerRfq.LatestRevisionNumber)
        {
            issues.Add(
                CreateReleaseIssue(
                    "revision.not-latest",
                    "Endast RFQ:ns senaste revision får släppas."));
        }

        if (revision.Lines.Count == 0)
        {
            issues.Add(
                CreateReleaseIssue(
                    "revision.no-lines",
                    "Revisionen måste innehålla minst en RFQ-rad innan den kan släppas."));
        }

        if (revision.SnapshotJson is not null || revision.SnapshotHash is not null)
        {
            issues.Add(
                CreateReleaseIssue(
                    "revision.draft-has-snapshot",
                    "En draft-revision får inte redan innehålla ett release-snapshot."));
        }

        var draftCount = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .CountAsync(
                x => x.BuyerRfqId == revision.BuyerRfqId
                    && x.Status == BuyerRfqRevisionStatus.Draft,
                cancellationToken);

        if (draftCount != 1)
        {
            issues.Add(
                CreateReleaseIssue(
                    "revision.invalid-draft-count",
                    $"RFQ:n har {draftCount} draft-revisioner. Exakt en förväntas."));
        }

        var releasedCount = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .CountAsync(
                x => x.BuyerRfqId == revision.BuyerRfqId
                    && x.Status == BuyerRfqRevisionStatus.ReleasedForMatching,
                cancellationToken);

        if (releasedCount > 1)
        {
            issues.Add(
                CreateReleaseIssue(
                    "revision.multiple-released",
                    "RFQ:n har fler än en aktuell ReleasedForMatching-revision."));
        }

        if (revision.RevisionNumber > 1 && releasedCount == 0)
        {
            issues.Add(
                CreateReleaseIssue(
                    "revision.missing-current-release",
                    "En senare draft-revision förväntar en tidigare aktuell ReleasedForMatching-revision."));
        }

        await ValidateDemandScopeProvenanceAsync(
            revision,
            issues,
            cancellationToken);

        await ValidateClaimProvenanceAsync(
            revision,
            issues,
            cancellationToken);

        _logger.LogDebug(
            "Buyer RFQ revision {BuyerRfqRevisionId} release readiness evaluated. IsReady={IsReady}, IssueCount={IssueCount}, IssueCodes={IssueCodes}",
            revision.Id,
            issues.Count == 0,
            issues.Count,
            string.Join(",", issues.Select(x => x.Code)));

        return Result.Ok(
            new BuyerRfqRevisionReleaseReadinessResponse
            {
                BuyerRfqRevisionId = revision.Id,
                IsReady = issues.Count == 0,
                Issues = issues
            });
    }

    /// <summary>
    /// Skapar ett immutable snapshot och släpper en draft-revision för matching.
    /// <para>
    /// Eventuell tidigare ReleasedForMatching-revision blir Superseded först när den nya revisionen faktiskt släpps.<br />
    /// RFQ-roten blir Active och OpenedAtUtc sätts första gången en revision släpps.
    /// </para>
    /// </summary>
    public async Task<Result<BuyerRfqResponse>> ReleaseRevisionAsync(
        Guid buyerRfqRevisionId,
        ReleaseBuyerRfqRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(
                validationResult.Errors);
        }

        var readinessResult = await GetReleaseReadinessAsync(
            buyerRfqRevisionId,
            cancellationToken);

        if (readinessResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(
                readinessResult.Errors);
        }

        if (!readinessResult.Value.IsReady)
        {
            _logger.LogInformation(
                "Buyer RFQ revision {BuyerRfqRevisionId} release blocked by readiness validation. IssueCount={IssueCount}, IssueCodes={IssueCodes}",
                buyerRfqRevisionId,
                readinessResult.Value.Issues.Count,
                string.Join(",", readinessResult.Value.Issues.Select(x => x.Code)));

            var errors = readinessResult.Value.Issues
                .Select(x => (IError)new ConflictError($"[{x.Code}] {x.Message}"));

            return Result.Fail<BuyerRfqResponse>(
                errors);
        }

        var revision = await _db.Set<BuyerRfqRevision>()
            .Include(x => x.BuyerRfq)
            .SingleAsync(
                x => x.Id == buyerRfqRevisionId,
                cancellationToken);

        var rfq = revision.BuyerRfq;

        if (revision.RowVersion != request.BuyerRfqRevisionRowVersion)
        {
            _logger.LogWarning(
                "Concurrency conflict detected before release for Buyer RFQ revision {BuyerRfqRevisionId}",
                revision.Id);

            return Result.Fail<BuyerRfqResponse>(
                new ConcurrencyError(
                    nameof(BuyerRfqRevision),
                    revision.Id));
        }

        if (rfq.RowVersion != request.BuyerRfqRowVersion)
        {
            _logger.LogWarning(
                "Concurrency conflict detected before release for Buyer RFQ {BuyerRfqId}",
                rfq.Id);

            return Result.Fail<BuyerRfqResponse>(
                new ConcurrencyError(
                    nameof(BuyerRfq),
                    rfq.Id));
        }

        var snapshot = await _snapshotService.CreateAsync(
            revision.Id,
            cancellationToken);

        var releasedAtUtc = DateTimeOffset.UtcNow;

        await using var transaction = await _db.Database.BeginTransactionAsync(
            cancellationToken);

        try
        {
            var previousReleased = await _db.Set<BuyerRfqRevision>()
                .SingleOrDefaultAsync(
                    x => x.BuyerRfqId == rfq.Id
                        && x.Id != revision.Id
                        && x.Status == BuyerRfqRevisionStatus.ReleasedForMatching,
                    cancellationToken);

            /*
             * Den gamla released-revisionen måste ändras först.
             * Det gör att det partiella unika indexet för ReleasedForMatching
             * inte kolliderar när den nya revisionen släpps.
             */
            if (previousReleased is not null)
            {
                previousReleased.Status = BuyerRfqRevisionStatus.Superseded;
                previousReleased.SupersededAtUtc = releasedAtUtc;

                await _db.SaveChangesAsync(
                    cancellationToken);
            }

            _db.Entry(revision)
                .Property(x => x.RowVersion)
                .OriginalValue = request.BuyerRfqRevisionRowVersion;

            _db.Entry(rfq)
                .Property(x => x.RowVersion)
                .OriginalValue = request.BuyerRfqRowVersion;

            revision.Status = BuyerRfqRevisionStatus.ReleasedForMatching;
            revision.ReleasedAtUtc = releasedAtUtc;
            revision.ReleasedByUserId = request.ReleasedByUserId;
            revision.SupersededAtUtc = null;
            revision.SnapshotJson = snapshot.SnapshotJson;
            revision.SnapshotHash = snapshot.SnapshotHash;

            rfq.Status = BuyerRfqStatus.Active;
            rfq.OpenedAtUtc ??= releasedAtUtc;
            rfq.ClosedAtUtc = null;

            await _db.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Buyer RFQ {BuyerRfqId} revision {BuyerRfqRevisionId} released for matching by user {ReleasedByUserId}. SupersededRevisionId={SupersededRevisionId}",
                rfq.Id,
                revision.Id,
                request.ReleasedByUserId,
                previousReleased?.Id);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _logger.LogWarning(
                exception,
                "Concurrency conflict while releasing Buyer RFQ {BuyerRfqId} revision {BuyerRfqRevisionId}",
                rfq.Id,
                revision.Id);

            return Result.Fail<BuyerRfqResponse>(
                CreateConcurrencyError(
                    exception,
                    revision.Id));
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                rfq.Id,
                cancellationToken));
    }

    /// <summary>
    /// Verifierar att ett sparat snapshot fortfarande motsvarar revisionens sparade hash.
    /// </summary>
    public async Task<Result<BuyerRfqSnapshotVerificationResponse>> VerifyRevisionSnapshotAsync(
        Guid buyerRfqRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revision = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqRevisionId)
            .Select(x => new
            {
                x.Id,
                x.SnapshotJson,
                x.SnapshotHash
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerRfqSnapshotVerificationResponse>(
                new NotFoundError(
                    nameof(BuyerRfqRevision),
                    buyerRfqRevisionId));
        }

        if (revision.SnapshotJson is null || revision.SnapshotHash is null)
        {
            return Result.Fail<BuyerRfqSnapshotVerificationResponse>(
                new ConflictError(
                    "BuyerRfqRevision does not contain a released snapshot."));
        }

        var computedHash = _snapshotService.ComputeHash(
            revision.SnapshotJson);

        var isValid = string.Equals(
            revision.SnapshotHash,
            computedHash,
            StringComparison.OrdinalIgnoreCase);

        if (isValid)
        {
            _logger.LogDebug(
                "Buyer RFQ revision {BuyerRfqRevisionId} snapshot verification passed",
                revision.Id);
        }
        else
        {
            _logger.LogWarning(
                "Buyer RFQ revision {BuyerRfqRevisionId} snapshot verification failed",
                revision.Id);
        }

        return Result.Ok(
            new BuyerRfqSnapshotVerificationResponse
            {
                BuyerRfqRevisionId = revision.Id,
                IsValid = isValid,
                StoredHash = revision.SnapshotHash,
                ComputedHash = computedHash
            });
    }

    /// <summary>
    /// Deep-clonar aktuell ReleasedForMatching-revision till nästa draft-revision.
    /// </summary>
    public async Task<Result<BuyerRfqResponse>> CreateNextRevisionAsync(
        Guid buyerRfqId,
        CreateNextBuyerRfqRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqResponse>(
                validationResult.Errors);
        }

        var rfq = await _db.Set<BuyerRfq>()
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqId,
                cancellationToken);

        if (rfq is null)
        {
            return Result.Fail<BuyerRfqResponse>(
                new NotFoundError(
                    nameof(BuyerRfq),
                    buyerRfqId));
        }

        if (rfq.RowVersion != request.BuyerRfqRowVersion)
        {
            return Result.Fail<BuyerRfqResponse>(
                new ConcurrencyError(
                    nameof(BuyerRfq),
                    rfq.Id));
        }

        if (rfq.Status is BuyerRfqStatus.Closed or BuyerRfqStatus.Cancelled)
        {
            return Result.Fail<BuyerRfqResponse>(
                new ConflictError(
                    $"BuyerRfq with status '{rfq.Status}' cannot create another revision."));
        }

        var existingDraft = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .AnyAsync(
                x => x.BuyerRfqId == rfq.Id
                    && x.Status == BuyerRfqRevisionStatus.Draft,
                cancellationToken);

        if (existingDraft)
        {
            return Result.Fail<BuyerRfqResponse>(
                new ConflictError(
                    "BuyerRfq already has a draft revision."));
        }

        var releasedRevisionIds = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Where(x =>
                x.BuyerRfqId == rfq.Id
                && x.Status == BuyerRfqRevisionStatus.ReleasedForMatching)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (releasedRevisionIds.Count != 1)
        {
            return Result.Fail<BuyerRfqResponse>(
                new ConflictError(
                    $"Expected exactly one ReleasedForMatching revision but found {releasedRevisionIds.Count}."));
        }

        var maxRevisionNumber = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Where(x => x.BuyerRfqId == rfq.Id)
            .MaxAsync(
                x => (int?)x.RevisionNumber,
                cancellationToken)
            ?? 0;

        if (maxRevisionNumber != rfq.LatestRevisionNumber)
        {
            return Result.Fail<BuyerRfqResponse>(
                new ConflictError(
                    "BuyerRfq.LatestRevisionNumber does not match the highest persisted revision number."));
        }

        var source = await LoadRevisionGraphAsync(
            releasedRevisionIds[0],
            cancellationToken);

        var nextRevisionNumber = maxRevisionNumber + 1;

        var newRevision = CloneRevision(
            source,
            nextRevisionNumber);

        _db.Entry(rfq)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerRfqRowVersion;

        rfq.LatestRevisionNumber = nextRevisionNumber;

        await using var transaction = await _db.Database.BeginTransactionAsync(
            cancellationToken);

        try
        {
            _db.Set<BuyerRfqRevision>()
                .Add(newRevision);

            await _db.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _logger.LogWarning(
                exception,
                "Concurrency conflict while creating next revision for Buyer RFQ {BuyerRfqId} from revision {SourceBuyerRfqRevisionId}",
                rfq.Id,
                source.Id);

            return Result.Fail<BuyerRfqResponse>(
                CreateConcurrencyError(
                    exception,
                    source.Id));
        }
        catch (DbUpdateException exception)
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _logger.LogWarning(
                exception,
                "Concurrent draft or revision-number conflict while creating next revision for Buyer RFQ {BuyerRfqId}",
                rfq.Id);

            return Result.Fail<BuyerRfqResponse>(
                new ConflictError(
                    "Another draft or revision number was created concurrently."));
        }

        _logger.LogInformation(
            "Buyer RFQ {BuyerRfqId} revision {BuyerRfqRevisionId} created as Draft from released revision {SourceBuyerRfqRevisionId}. RevisionNumber={RevisionNumber}",
            rfq.Id,
            newRevision.Id,
            source.Id,
            newRevision.RevisionNumber);

        return Result.Ok(
            await ProjectRequiredAsync(
                rfq.Id,
                cancellationToken));
    }

    /// <summary>
    /// Läser hela revisionsgrafen som behövs för deep clone.
    /// </summary>
    private async Task<BuyerRfqRevision> LoadRevisionGraphAsync(
        Guid buyerRfqRevisionId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Include(x => x.Lines)
                .ThenInclude(x => x.MaterialOptions)
            .Include(x => x.Requirements)
            .Include(x => x.Deliveries)
                .ThenInclude(x => x.LineAllocations)
            .AsSplitQuery()
            .SingleAsync(
                x => x.Id == buyerRfqRevisionId,
                cancellationToken);
    }

    /// <summary>
    /// Deep-clonar revisionsmetadata, lines, material options, requirements, deliveries och allocations.
    /// </summary>
    private static BuyerRfqRevision CloneRevision(
        BuyerRfqRevision source,
        int revisionNumber)
    {
        var newRevision = new BuyerRfqRevision
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqId = source.BuyerRfqId,
            RevisionNumber = revisionNumber,
            Status = BuyerRfqRevisionStatus.Draft,
            Summary = source.Summary,
            BuyerOriginalText = source.BuyerOriginalText,
            InternalInterpretation = source.InternalInterpretation,
            ResponseRequestedByUtc = source.ResponseRequestedByUtc,
            ReleasedAtUtc = null,
            ReleasedByUserId = null,
            SupersededAtUtc = null,
            SnapshotHash = null,
            SnapshotJson = null
        };

        var lineIdMap = new Dictionary<Guid, Guid>();

        foreach (var sourceLine in source.Lines.OrderBy(x => x.LineNumber))
        {
            var newLine = CloneLine(
                sourceLine,
                newRevision.Id);

            lineIdMap.Add(
                sourceLine.Id,
                newLine.Id);

            foreach (var sourceOption in sourceLine.MaterialOptions)
            {
                newLine.MaterialOptions.Add(
                    CloneMaterialOption(
                        sourceOption,
                        newLine.Id));
            }

            newRevision.Lines.Add(
                newLine);
        }

        foreach (var sourceRequirement in source.Requirements)
        {
            newRevision.Requirements.Add(
                CloneRequirement(
                    sourceRequirement,
                    newRevision.Id,
                    lineIdMap));
        }

        foreach (var sourceDelivery in source.Deliveries.OrderBy(x => x.DeliveryNumber))
        {
            var newDelivery = CloneDelivery(
                sourceDelivery,
                newRevision.Id);

            foreach (var sourceAllocation in sourceDelivery.LineAllocations)
            {
                if (!lineIdMap.TryGetValue(
                    sourceAllocation.BuyerRfqLineId,
                    out var newLineId))
                {
                    throw new InvalidOperationException(
                        $"BuyerRfqLine '{sourceAllocation.BuyerRfqLineId}' could not be mapped while cloning delivery allocations.");
                }

                newDelivery.LineAllocations.Add(
                    new BuyerRfqDeliveryLineAllocation
                    {
                        Id = Guid.CreateVersion7(),
                        BuyerRfqRevisionId = newRevision.Id,
                        BuyerRfqDeliveryId = newDelivery.Id,
                        BuyerRfqLineId = newLineId,
                        Quantity = sourceAllocation.Quantity,
                        QuantityUnitId = sourceAllocation.QuantityUnitId
                    });
            }

            newRevision.Deliveries.Add(
                newDelivery);
        }

        return newRevision;
    }

    /// <summary>
    /// Klonar en RFQ-rad utan tracking- eller concurrency-metadata.
    /// </summary>
    private static BuyerRfqLine CloneLine(
        BuyerRfqLine source,
        Guid newRevisionId)
    {
        return new BuyerRfqLine
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqRevisionId = newRevisionId,
            BuyerDemandScopeId = source.BuyerDemandScopeId,
            LineNumber = source.LineNumber,
            Name = source.Name,
            Description = source.Description,
            BuyerLineReference = source.BuyerLineReference,
            BuyerOriginalText = source.BuyerOriginalText,
            MaterialId = source.MaterialId,
            MaterialTypeId = source.MaterialTypeId,
            IntendedUseCharacteristicId = source.IntendedUseCharacteristicId,
            RequiredQuantity = source.RequiredQuantity,
            QuantityUnitId = source.QuantityUnitId,
            LowerQuantityTolerancePercent = source.LowerQuantityTolerancePercent,
            UpperQuantityTolerancePercent = source.UpperQuantityTolerancePercent,
            AllowPartialFulfillment = source.AllowPartialFulfillment,
            AllowMultipleSuppliers = source.AllowMultipleSuppliers,
            AllowMultipleOrigins = source.AllowMultipleOrigins,
            AllowMultipleLots = source.AllowMultipleLots,
            RequireSingleSupplier = source.RequireSingleSupplier,
            RequireSingleOrigin = source.RequireSingleOrigin,
            RequireSingleLot = source.RequireSingleLot,
            RequireUniformCertification = source.RequireUniformCertification,
            RequireUniformSpecification = source.RequireUniformSpecification,
            MaximumSupplierCount = source.MaximumSupplierCount,
            InternalNotes = source.InternalNotes
        };
    }

    /// <summary>
    /// Klonar ett materialalternativ till en ny RFQ-rad.
    /// </summary>
    private static BuyerRfqLineMaterialOption CloneMaterialOption(
        BuyerRfqLineMaterialOption source,
        Guid newLineId)
    {
        return new BuyerRfqLineMaterialOption
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqLineId = newLineId,
            OptionKind = source.OptionKind,
            MaterialId = source.MaterialId,
            MaterialTypeId = source.MaterialTypeId,
            BiologicalSourceId = source.BiologicalSourceId,
            BiologicalSourceClaimId = source.BiologicalSourceClaimId,
            BuyerText = source.BuyerText,
            AlternativeGroupKey = source.AlternativeGroupKey,
            MinimumMappingConfidence = source.MinimumMappingConfidence,
            SortOrder = source.SortOrder
        };
    }

    /// <summary>
    /// Klonar ett leveranstillfälle utan dess allocations.
    /// </summary>
    private static BuyerRfqDelivery CloneDelivery(
        BuyerRfqDelivery source,
        Guid newRevisionId)
    {
        return new BuyerRfqDelivery
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqRevisionId = newRevisionId,
            DeliveryNumber = source.DeliveryNumber,
            DestinationAddressId = source.DestinationAddressId,
            DestinationLocationId = source.DestinationLocationId,
            RequestedDeliveryFromUtc = source.RequestedDeliveryFromUtc,
            RequestedDeliveryToUtc = source.RequestedDeliveryToUtc,
            IncotermRuleId = source.IncotermRuleId,
            NamedPlace = source.NamedPlace,
            MinimumTemperatureCelsius = source.MinimumTemperatureCelsius,
            MaximumTemperatureCelsius = source.MaximumTemperatureCelsius,
            PackagingNotes = source.PackagingNotes,
            DeliveryNotes = source.DeliveryNotes,
            SortOrder = source.SortOrder
        };
    }

    /// <summary>
    /// Klonar rätt konkret TPH-requirement och mappar eventuell line till den nya revisionen.
    /// </summary>
    private static BuyerRfqRequirement CloneRequirement(
        BuyerRfqRequirement source,
        Guid newRevisionId,
        IReadOnlyDictionary<Guid, Guid> lineIdMap)
    {
        Guid? newLineId = source.BuyerRfqLineId.HasValue
            ? lineIdMap[source.BuyerRfqLineId.Value]
            : null;

        BuyerRfqRequirement clone = source switch
        {
            BuyerRfqMaterialRequirement x => new BuyerRfqMaterialRequirement
            {
                Name = x.Name,
                Description = x.Description,
                MaterialId = x.MaterialId,
                MaterialTypeId = x.MaterialTypeId,
                MatchMode = x.MatchMode,
                AllowSubstitutes = x.AllowSubstitutes
            },

            BuyerRfqBiologicalSourceRequirement x => new BuyerRfqBiologicalSourceRequirement
            {
                Name = x.Name,
                Description = x.Description,
                BiologicalSourceId = x.BiologicalSourceId,
                Operator = x.Operator
            },

            BuyerRfqOriginRequirement x => new BuyerRfqOriginRequirement
            {
                Name = x.Name,
                Description = x.Description,
                GeographicLocationId = x.GeographicLocationId,
                CountryId = x.CountryId,
                OriginRole = x.OriginRole,
                Operator = x.Operator
            },

            BuyerRfqCertificationRequirement x => new BuyerRfqCertificationRequirement
            {
                Name = x.Name,
                Description = x.Description,
                CertificationSchemeId = x.CertificationSchemeId,
                Operator = x.Operator,
                RequiredScope = x.RequiredScope,
                CertificationSchemeJurisdictionId = x.CertificationSchemeJurisdictionId,
                MustBeValidAtDelivery = x.MustBeValidAtDelivery
            },

            BuyerRfqDocumentRequirement x => new BuyerRfqDocumentRequirement
            {
                Name = x.Name,
                Description = x.Description,
                DocumentTypeId = x.DocumentTypeId,
                Operator = x.Operator,
                MustBeLotSpecific = x.MustBeLotSpecific,
                MustBeIssuedByAccreditedParty = x.MustBeIssuedByAccreditedParty,
                MaximumAgeDays = x.MaximumAgeDays
            },

            BuyerRfqConstituentRequirement x => new BuyerRfqConstituentRequirement
            {
                Name = x.Name,
                Description = x.Description,
                ConstituentId = x.ConstituentId,
                Operator = x.Operator,
                MinimumValue = x.MinimumValue,
                MaximumValue = x.MaximumValue,
                TargetValue = x.TargetValue,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Basis = x.Basis,
                AnalysisMethod = x.AnalysisMethod,
                MustBeVerifiedForEveryLot = x.MustBeVerifiedForEveryLot,
                AccreditedLaboratoryRequired = x.AccreditedLaboratoryRequired
            },

            BuyerRfqCharacteristicRequirement x => new BuyerRfqCharacteristicRequirement
            {
                Name = x.Name,
                Description = x.Description,
                CharacteristicId = x.CharacteristicId,
                Operator = x.Operator,
                ExpectedTextValue = x.ExpectedTextValue,
                ExpectedNumericValue = x.ExpectedNumericValue,
                UnitOfMeasureId = x.UnitOfMeasureId
            },

            BuyerRfqProcessRequirement x => new BuyerRfqProcessRequirement
            {
                Name = x.Name,
                Description = x.Description,
                ProcessTypeId = x.ProcessTypeId,
                Mode = x.Mode,
                ProcessParametersJson = x.ProcessParametersJson
            },

            BuyerRfqCommercialRequirement x => new BuyerRfqCommercialRequirement
            {
                Name = x.Name,
                Description = x.Description,
                Kind = x.Kind,
                Operator = x.Operator,
                NumericValue = x.NumericValue,
                TextValue = x.TextValue,
                UnitOfMeasureId = x.UnitOfMeasureId,
                CurrencyId = x.CurrencyId
            },

            BuyerRfqLogisticsRequirement x => new BuyerRfqLogisticsRequirement
            {
                Name = x.Name,
                Description = x.Description,
                Kind = x.Kind,
                Operator = x.Operator,
                NumericValue = x.NumericValue,
                TextValue = x.TextValue,
                UnitOfMeasureId = x.UnitOfMeasureId,
                IncotermRuleId = x.IncotermRuleId
            },

            _ => throw new InvalidOperationException(
                $"Unsupported BuyerRfqRequirement type '{source.GetType().Name}'.")
        };

        clone.Id = Guid.CreateVersion7();
        clone.BuyerRfqRevisionId = newRevisionId;
        clone.BuyerRfqLineId = newLineId;
        clone.Level = source.Level;
        clone.Weight = source.Weight;
        clone.AlternativeGroupKey = source.AlternativeGroupKey;
        clone.IsActive = source.IsActive;
        clone.SortOrder = source.SortOrder;
        clone.BuyerOriginalText = source.BuyerOriginalText;
        clone.InternalNotes = source.InternalNotes;

        return clone;
    }

    /// <summary>
    /// Översätter en EF concurrency-konflikt till rätt domänentitet.
    /// </summary>
    private static ConcurrencyError CreateConcurrencyError(
        DbUpdateConcurrencyException exception,
        Guid fallbackRevisionId)
    {
        var entity = exception.Entries
            .Select(x => x.Entity)
            .FirstOrDefault();

        return entity switch
        {
            BuyerRfq rfq => new ConcurrencyError(nameof(BuyerRfq), rfq.Id),
            BuyerRfqRevision revision => new ConcurrencyError(nameof(BuyerRfqRevision), revision.Id),
            _ => new ConcurrencyError(nameof(BuyerRfqRevision), fallbackRevisionId)
        };
    }

    /// <summary>
    /// Skapar ett strukturerat release-problem.
    /// </summary>
    private static BuyerRfqReleaseIssueResponse CreateReleaseIssue(string code, string message)
    {
        return new BuyerRfqReleaseIssueResponse
        {
            Code = code,
            Message = message
        };
    }

    /// <summary>
    /// Kontrollerar att eventuella BuyerDemandScope-referenser fortfarande hör till RFQ:ns demand och buyer-relation.
    /// </summary>
    private async Task ValidateDemandScopeProvenanceAsync(
        BuyerRfqRevision revision,
        ICollection<BuyerRfqReleaseIssueResponse> issues,
        CancellationToken cancellationToken)
    {
        var scopeIds = revision.Lines
            .Where(x => x.BuyerDemandScopeId.HasValue)
            .Select(x => x.BuyerDemandScopeId!.Value)
            .Distinct()
            .ToList();

        if (scopeIds.Count == 0)
        {
            return;
        }

        if (!revision.BuyerRfq.BuyerDemandId.HasValue)
        {
            issues.Add(
                CreateReleaseIssue(
                    "scope.rfq-has-no-demand",
                    "Revisionen innehåller demand-scope-referenser men RFQ:n saknar BuyerDemandId."));

            return;
        }

        var validScopeIds = await _db.Set<BuyerDemandScope>()
            .AsNoTracking()
            .Where(x =>
                scopeIds.Contains(x.Id)
                && x.BuyerDemandId == revision.BuyerRfq.BuyerDemandId.Value
                && x.BusinessRelationshipId == revision.BuyerRfq.BuyerBusinessRelationshipId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (validScopeIds.Count != scopeIds.Count)
        {
            issues.Add(
                CreateReleaseIssue(
                    "scope.invalid-provenance",
                    "Minst en BuyerDemandScope-referens hör inte till RFQ:ns BuyerDemand och buyer-relation."));
        }
    }

    /// <summary>
    /// Kontrollerar att eventuella biological-source-claims fortfarande hör till RFQ:ns buyer-relation.
    /// </summary>
    private async Task ValidateClaimProvenanceAsync(
        BuyerRfqRevision revision,
        ICollection<BuyerRfqReleaseIssueResponse> issues,
        CancellationToken cancellationToken)
    {
        var claimIds = revision.Lines
            .SelectMany(x => x.MaterialOptions)
            .Where(x => x.BiologicalSourceClaimId.HasValue)
            .Select(x => x.BiologicalSourceClaimId!.Value)
            .Distinct()
            .ToList();

        if (claimIds.Count == 0)
        {
            return;
        }

        var validClaimIds = await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
            .AsNoTracking()
            .Where(x =>
                claimIds.Contains(x.Id)
                && x.BusinessRelationshipId == revision.BuyerRfq.BuyerBusinessRelationshipId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (validClaimIds.Count != claimIds.Count)
        {
            issues.Add(
                CreateReleaseIssue(
                    "claim.invalid-provenance",
                    "Minst en BiologicalSourceClaim-referens hör inte till RFQ:ns buyer-relation."));
        }
    }

    /// <summary>
    /// Kontrollerar att relationen är en buyer-relation.
    /// </summary>
    private async Task<Result> ValidateBuyerRelationshipAsync(
        Guid buyerBusinessRelationshipId,
        CancellationToken cancellationToken)
    {
        var relationship = await _db.Set<BusinessRelationship>()
            .AsNoTracking()
            .Where(x => x.Id == buyerBusinessRelationshipId)
            .Select(x => new
            {
                RelationshipTypeCode = x.BusinessRelationshipType.Code
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (relationship is null)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(BusinessRelationship),
                    buyerBusinessRelationshipId));
        }

        if (relationship.RelationshipTypeCode != BusinessRelationshipTypeCodes.Buyer)
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerRfq can only be created on a buyer business relationship."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar att revisionen är en redigerbar Draft.
    /// </summary>
    private static Result ValidateRevisionIsEditable(
        BuyerRfqRevision revision)
    {
        if (revision.Status != BuyerRfqRevisionStatus.Draft)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerRfqRevision with status '{revision.Status}' cannot be modified."));
        }

        if (revision.ReleasedAtUtc.HasValue)
        {
            return Result.Fail(
                new ConflictError(
                    "A released BuyerRfqRevision cannot be modified."));
        }

        if (revision.SupersededAtUtc.HasValue)
        {
            return Result.Fail(
                new ConflictError(
                    "A superseded BuyerRfqRevision cannot be modified."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar tillåtna manuella statusövergångar på RFQ-roten.
    /// <para>
    /// Draft till Active sker inte här utan när en revision släpps för matching.<br />
    /// Closed och Cancelled behandlas som terminala statusar.
    /// </para>
    /// </summary>
    private static Result ValidateStatusTransition(
        BuyerRfqStatus currentStatus,
        BuyerRfqStatus targetStatus)
    {
        var allowed = (currentStatus, targetStatus) switch
        {
            (BuyerRfqStatus.Draft, BuyerRfqStatus.Cancelled) => true,

            (BuyerRfqStatus.Active, BuyerRfqStatus.OnHold) => true,
            (BuyerRfqStatus.Active, BuyerRfqStatus.Closed) => true,
            (BuyerRfqStatus.Active, BuyerRfqStatus.Cancelled) => true,

            (BuyerRfqStatus.OnHold, BuyerRfqStatus.Active) => true,
            (BuyerRfqStatus.OnHold, BuyerRfqStatus.Closed) => true,
            (BuyerRfqStatus.OnHold, BuyerRfqStatus.Cancelled) => true,

            _ => false
        };

        if (allowed)
        {
            return Result.Ok();
        }

        return Result.Fail(
            new ConflictError(
                $"BuyerRfq cannot transition from '{currentStatus}' to '{targetStatus}' through this operation."));
    }

    /// <summary>
    /// Skapar revision 1 som en redigerbar Draft.
    /// </summary>
    private static BuyerRfqRevision CreateInitialRevision(
        Guid buyerRfqId,
        string? summary,
        string? buyerOriginalText,
        string? internalInterpretation,
        DateTimeOffset? responseRequestedByUtc)
    {
        return new BuyerRfqRevision
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqId = buyerRfqId,
            RevisionNumber = 1,
            Status = BuyerRfqRevisionStatus.Draft,
            Summary = TrimToNull(summary),
            BuyerOriginalText = TrimToNull(buyerOriginalText),
            InternalInterpretation = TrimToNull(internalInterpretation),
            ResponseRequestedByUtc = responseRequestedByUtc,
            ReleasedAtUtc = null,
            ReleasedByUserId = null,
            SupersededAtUtc = null,
            SnapshotHash = null,
            SnapshotJson = null
        };
    }

    /// <summary>
    /// Sparar ett RowVersion-skyddat objekt med optimistic concurrency.
    /// </summary>
    private async Task<Result> SaveWithConcurrencyAsync<TEntity>(
        TEntity entity,
        Guid entityId,
        uint expectedRowVersion,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        _db.Entry(entity)
            .Property("RowVersion")
            .OriginalValue = expectedRowVersion;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(
                exception,
                "Concurrency conflict while saving {EntityType} {EntityId}",
                typeof(TEntity).Name,
                entityId);

            return Result.Fail(
                new ConcurrencyError(
                    typeof(TEntity).Name,
                    entityId));
        }
    }

    /// <summary>
    /// Projekterar en BuyerRfq direkt från databasen.
    /// </summary>
    private async Task<BuyerRfqResponse?> ProjectAsync(
        Guid buyerRfqId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerRfq>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqId)
            .Select(BuyerRfqProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar en sparad BuyerRfq och kastar om den oväntat saknas.
    /// </summary>
    private async Task<BuyerRfqResponse> ProjectRequiredAsync(
        Guid buyerRfqId,
        CancellationToken cancellationToken)
    {
        return await ProjectAsync(buyerRfqId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"BuyerRfq '{buyerRfqId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Projekterar en sparad RFQ-revision och kastar om den oväntat saknas.
    /// </summary>
    private async Task<BuyerRfqRevisionResponse> ProjectRevisionRequiredAsync(
        Guid buyerRfqRevisionId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqRevisionId)
            .Select(BuyerRfqProjections.Revision)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException(
                $"BuyerRfqRevision '{buyerRfqRevisionId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Lägger på stabil sortering innan paginering utförs.
    /// </summary>
    private static IQueryable<BuyerRfq> ApplySorting(
        IQueryable<BuyerRfq> query,
        BuyerRfqSortField sortBy,
        SortDirection sortDirection)
    {
        return (sortBy, sortDirection) switch
        {
            (BuyerRfqSortField.RfqNumber, SortDirection.Descending) =>
                query.OrderByDescending(x => x.RfqNumber).ThenByDescending(x => x.Id),

            (BuyerRfqSortField.Name, SortDirection.Ascending) =>
                query.OrderBy(x => x.Name).ThenBy(x => x.Id),

            (BuyerRfqSortField.Name, SortDirection.Descending) =>
                query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),

            (BuyerRfqSortField.OpenedAtUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.OpenedAtUtc).ThenBy(x => x.Id),

            (BuyerRfqSortField.OpenedAtUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.OpenedAtUtc).ThenByDescending(x => x.Id),

            (BuyerRfqSortField.CreateDateUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.CreateDateUtc).ThenBy(x => x.Id),

            (BuyerRfqSortField.CreateDateUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.CreateDateUtc).ThenByDescending(x => x.Id),

            (BuyerRfqSortField.LatestRevisionNumber, SortDirection.Ascending) =>
                query.OrderBy(x => x.LatestRevisionNumber).ThenBy(x => x.Id),

            (BuyerRfqSortField.LatestRevisionNumber, SortDirection.Descending) =>
                query.OrderByDescending(x => x.LatestRevisionNumber).ThenByDescending(x => x.Id),

            _ =>
                query.OrderBy(x => x.RfqNumber).ThenBy(x => x.Id)
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