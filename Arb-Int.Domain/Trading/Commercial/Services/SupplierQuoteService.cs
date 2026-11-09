using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Trading.Snapshots;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar supplier quotes, revisioner och övergången från redigerbar Draft till immutable Received.
/// </summary>
public sealed class SupplierQuoteService : ISupplierQuoteService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ISupplierQuoteReferenceValidator _referenceValidator;
    private readonly ISupplierQuoteRevisionSnapshotService _snapshotService;
    private readonly ILogger<SupplierQuoteService> _logger;

    /// <summary>
    /// Skapar supplier-quote-servicen med nödvändiga domänberoenden.
    /// </summary>
    public SupplierQuoteService(
        Arb_IntDbContext db,
        IValidationService validationService,
        ISupplierQuoteReferenceValidator referenceValidator,
        ISupplierQuoteRevisionSnapshotService snapshotService,
        ILogger<SupplierQuoteService> logger)
    {
        _db = db;
        _validationService = validationService;
        _referenceValidator = referenceValidator;
        _snapshotService = snapshotService;
        _logger = logger;
    }

    /// <summary>
    /// Hämtar en supplier quote med revisionshistorik.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> GetByIdAsync(
        Guid supplierQuoteId,
        CancellationToken cancellationToken = default)
    {
        var response = await ProjectQuoteAsync(supplierQuoteId, cancellationToken);

        return response is null
            ? Result.Fail<SupplierQuoteResponse>(new NotFoundError(nameof(SupplierQuote), supplierQuoteId))
            : Result.Ok(response);
    }

    /// <summary>
    /// Söker, filtrerar, sorterar och paginerar supplier quotes för en intern organisation.
    /// </summary>
    public async Task<Result<PagedResult<SupplierQuoteListResponse>>> QueryAsync(
        Guid owningOrganizationId,
        SupplierQuoteQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<PagedResult<SupplierQuoteListResponse>>(
                validationResult.Errors);
        }

        var organizationExists = await _db.Set<Organization>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == owningOrganizationId,
                cancellationToken);

        if (!organizationExists)
        {
            return Result.Fail<PagedResult<SupplierQuoteListResponse>>(
                new NotFoundError(
                    nameof(Organization),
                    owningOrganizationId));
        }

        IQueryable<SupplierQuote> query = _db.Set<SupplierQuote>()
            .AsNoTracking()
            .Where(x =>
                x.SupplierBusinessRelationship.OwningOrganizationId == owningOrganizationId);

        if (request.SupplierBusinessRelationshipId.HasValue)
        {
            query = query.Where(x =>
                x.SupplierBusinessRelationshipId == request.SupplierBusinessRelationshipId.Value);
        }

        if (request.SupplierPartyId.HasValue)
        {
            query = query.Where(x =>
                x.SupplierBusinessRelationship.CounterpartyId == request.SupplierPartyId.Value);
        }

        if (request.BuyerRfqId.HasValue)
        {
            query = query.Where(x =>
                x.BuyerRfqRevision != null
                && x.BuyerRfqRevision.BuyerRfqId == request.BuyerRfqId.Value);
        }

        if (request.BuyerRfqRevisionId.HasValue)
        {
            query = query.Where(x =>
                x.BuyerRfqRevisionId == request.BuyerRfqRevisionId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == request.Status.Value);
        }

        if (request.OwnerUserId.HasValue)
        {
            query = query.Where(x =>
                x.OwnerUserId == request.OwnerUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern =
                $"%{request.Search.Trim()}%";

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.QuoteNumber,
                    searchPattern)
                || EF.Functions.ILike(
                    x.Name,
                    searchPattern)
                || EF.Functions.ILike(
                    x.SupplierBusinessRelationship.Counterparty.Name,
                    searchPattern)
                || (x.SupplierReference != null
                    && EF.Functions.ILike(
                        x.SupplierReference,
                        searchPattern))
                || (x.BuyerRfqRevision != null
                    && EF.Functions.ILike(
                        x.BuyerRfqRevision.BuyerRfq.RfqNumber,
                        searchPattern)));
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
            .Select(SupplierQuoteProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok(
            new PagedResult<SupplierQuoteListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }

    /// <summary>
    /// Hämtar en specifik supplier-quote-revision.
    /// </summary>
    public async Task<Result<SupplierQuoteRevisionResponse>> GetRevisionByIdAsync(
        Guid supplierQuoteRevisionId,
        CancellationToken cancellationToken = default)
    {
        var response = await ProjectRevisionAsync(supplierQuoteRevisionId, cancellationToken);

        return response is null
            ? Result.Fail<SupplierQuoteRevisionResponse>(new NotFoundError(nameof(SupplierQuoteRevision), supplierQuoteRevisionId))
            : Result.Ok(response);
    }

    /// <summary>
    /// Skapar quote-roten och revision 1 som Draft.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> CreateAsync(
        Guid supplierBusinessRelationshipId,
        CreateSupplierQuoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(validation.Errors);
        }

        var supplierExists = await _db.Set<BusinessRelationship>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == supplierBusinessRelationshipId, cancellationToken);

        if (!supplierExists)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(nameof(BusinessRelationship), supplierBusinessRelationshipId));
        }

        /*
         * Modellen/configen säger inte vilken BusinessRelationshipType som krävs.
         * Därför görs ingen påhittad "måste vara Supplier"-typkontroll här.
         */

        if (request.BuyerRfqRevisionId.HasValue)
        {
            var rfqRevisionExists = await _db.Set<BuyerRfqRevision>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.BuyerRfqRevisionId.Value, cancellationToken);

            if (!rfqRevisionExists)
            {
                return Result.Fail<SupplierQuoteResponse>(
                    new NotFoundError(nameof(BuyerRfqRevision), request.BuyerRfqRevisionId.Value));
            }
        }

        var revisionReferences = await _referenceValidator.ValidateRevisionReferencesAsync(
            request.CurrencyId,
            request.IncotermRuleId,
            cancellationToken);

        if (revisionReferences.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(revisionReferences.Errors);
        }

        var quoteNumber = request.QuoteNumber.Trim();

        var duplicateQuoteNumber = await _db.Set<SupplierQuote>()
            .AsNoTracking()
            .AnyAsync(x => x.QuoteNumber == quoteNumber, cancellationToken);

        if (duplicateQuoteNumber)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError($"SupplierQuote number '{quoteNumber}' already exists."));
        }

        var quote = new SupplierQuote
        {
            Id = Guid.CreateVersion7(),
            SupplierBusinessRelationshipId = supplierBusinessRelationshipId,
            BuyerRfqRevisionId = request.BuyerRfqRevisionId,
            QuoteNumber = quoteNumber,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Status = SupplierQuoteStatus.Draft,
            LatestRevisionNumber = 1,
            SupplierReference = TrimToNull(request.SupplierReference),
            ReceivedAtUtc = null,
            OwnerUserId = request.OwnerUserId,
            InternalNotes = TrimToNull(request.InternalNotes)
        };

        var revision = new SupplierQuoteRevision
        {
            Id = Guid.CreateVersion7(),
            SupplierQuoteId = quote.Id,
            SupplierBusinessRelationshipId = supplierBusinessRelationshipId,
            RevisionNumber = 1,
            Status = SupplierQuoteRevisionStatus.Draft,
            CurrencyId = request.CurrencyId,
            IncotermRuleId = request.IncotermRuleId,
            NamedPlace = TrimToNull(request.NamedPlace),
            PaymentTerms = TrimToNull(request.PaymentTerms),
            LeadTimeDays = request.LeadTimeDays,
            ValidFromUtc = request.ValidFromUtc,
            ValidToUtc = request.ValidToUtc,
            ReceivedAtUtc = null,
            OriginalDocumentReference = TrimToNull(request.OriginalDocumentReference),
            SnapshotJson = null,
            SnapshotHash = null,
            InternalNotes = TrimToNull(request.RevisionInternalNotes)
        };

        quote.Revisions.Add(revision);
        _db.Set<SupplierQuote>().Add(quote);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            _logger.LogWarning(
                exception,
                "Concurrent unique-key conflict while creating Supplier quote with number {SupplierQuoteNumber}",
                quoteNumber);

            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError($"SupplierQuote number '{quoteNumber}' already exists."));
        }

        _logger.LogInformation(
            "Supplier quote {SupplierQuoteId} revision {SupplierQuoteRevisionId} created in Draft for business relationship {BusinessRelationshipId}. BuyerRfqRevisionId={BuyerRfqRevisionId}",
            quote.Id,
            revision.Id,
            quote.SupplierBusinessRelationshipId,
            quote.BuyerRfqRevisionId);

        return Result.Ok(
            await ProjectRequiredQuoteAsync(
                quote.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar quote-rootens metadata utan att ändra supplier, RFQ eller QuoteNumber.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> UpdateAsync(
        Guid supplierQuoteId,
        UpdateSupplierQuoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(validation.Errors);
        }

        var quote = await _db.Set<SupplierQuote>()
            .SingleOrDefaultAsync(x => x.Id == supplierQuoteId, cancellationToken);

        if (quote is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(nameof(SupplierQuote), supplierQuoteId));
        }

        _db.Entry(quote)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        quote.Name = request.Name.Trim();
        quote.Description = request.Description.Trim();
        quote.SupplierReference = TrimToNull(request.SupplierReference);
        quote.OwnerUserId = request.OwnerUserId;
        quote.InternalNotes = TrimToNull(request.InternalNotes);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(
                exception,
                "Concurrency conflict while updating Supplier quote {SupplierQuoteId}",
                quote.Id);

            return Result.Fail<SupplierQuoteResponse>(
                new ConcurrencyError(nameof(SupplierQuote), quote.Id));
        }

        _logger.LogInformation(
            "Supplier quote {SupplierQuoteId} metadata updated",
            quote.Id);

        return Result.Ok(
            await ProjectRequiredQuoteAsync(
                quote.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar revisionens kommersiella huvudvillkor så länge revisionen är Draft.
    /// </summary>
    public async Task<Result<SupplierQuoteRevisionResponse>> UpdateRevisionAsync(
        Guid supplierQuoteRevisionId,
        UpdateSupplierQuoteRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(validation.Errors);
        }

        var revision = await _db.Set<SupplierQuoteRevision>()
            .SingleOrDefaultAsync(x => x.Id == supplierQuoteRevisionId, cancellationToken);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                new NotFoundError(nameof(SupplierQuoteRevision), supplierQuoteRevisionId));
        }

        var writableResult = ValidateRevisionIsWritable(revision);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(writableResult.Errors);
        }

        var referencesResult = await _referenceValidator.ValidateRevisionReferencesAsync(
            request.CurrencyId,
            request.IncotermRuleId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(referencesResult.Errors);
        }

        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        revision.CurrencyId = request.CurrencyId;
        revision.IncotermRuleId = request.IncotermRuleId;
        revision.NamedPlace = TrimToNull(request.NamedPlace);
        revision.PaymentTerms = TrimToNull(request.PaymentTerms);
        revision.LeadTimeDays = request.LeadTimeDays;
        revision.ValidFromUtc = request.ValidFromUtc;
        revision.ValidToUtc = request.ValidToUtc;
        revision.OriginalDocumentReference = TrimToNull(request.OriginalDocumentReference);
        revision.InternalNotes = TrimToNull(request.InternalNotes);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(
                exception,
                "Concurrency conflict while updating Supplier quote revision {SupplierQuoteRevisionId}",
                revision.Id);

            return Result.Fail<SupplierQuoteRevisionResponse>(
                new ConcurrencyError(nameof(SupplierQuoteRevision), revision.Id));
        }

        _logger.LogInformation(
            "Supplier quote revision {SupplierQuoteRevisionId} metadata updated",
            revision.Id);

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                revision.Id,
                cancellationToken));
    }

    /// <summary>
    /// Deep-klonar senaste frysta revision till nästa Draft utan att ändra källrevisionens status.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> CreateNextRevisionAsync(
        Guid supplierQuoteId,
        CreateNextSupplierQuoteRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(validation.Errors);
        }

        var quote = await _db.Set<SupplierQuote>()
            .Include(x => x.Revisions)
                .ThenInclude(x => x.Lines)
            .SingleOrDefaultAsync(x => x.Id == supplierQuoteId, cancellationToken);

        if (quote is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(nameof(SupplierQuote), supplierQuoteId));
        }

        if (quote.Revisions.Any(x => x.Status == SupplierQuoteRevisionStatus.Draft))
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError("SupplierQuote already has a Draft revision."));
        }

        if (quote.Status == SupplierQuoteStatus.UnderReview)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError(
                    "A new SupplierQuoteRevision cannot be created while the current revision is UnderReview."));
        }

        var source = quote.Revisions
            .SingleOrDefault(x => x.RevisionNumber == quote.LatestRevisionNumber);

        if (source is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError("LatestRevisionNumber does not reference an existing SupplierQuoteRevision."));
        }

        if (source.Status == SupplierQuoteRevisionStatus.Draft)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError("Latest SupplierQuoteRevision is already Draft."));
        }

        if (string.IsNullOrWhiteSpace(source.SnapshotJson)
            || string.IsNullOrWhiteSpace(source.SnapshotHash))
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError("Latest SupplierQuoteRevision is not frozen and cannot be used as clone source."));
        }

        var computedSourceHash = _snapshotService.ComputeHash(source.SnapshotJson);

        if (!string.Equals(
                computedSourceHash,
                source.SnapshotHash,
                StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Supplier quote {SupplierQuoteId} cannot create a new revision because source revision {SourceSupplierQuoteRevisionId} snapshot verification failed",
                quote.Id,
                source.Id);

            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError("Latest SupplierQuoteRevision has an invalid snapshot hash."));
        }

        var nextRevisionNumber = quote.LatestRevisionNumber + 1;

        var nextRevision = new SupplierQuoteRevision
        {
            Id = Guid.CreateVersion7(),
            SupplierQuoteId = quote.Id,
            SupplierBusinessRelationshipId = quote.SupplierBusinessRelationshipId,
            RevisionNumber = nextRevisionNumber,
            Status = SupplierQuoteRevisionStatus.Draft,
            CurrencyId = source.CurrencyId,
            IncotermRuleId = source.IncotermRuleId,
            NamedPlace = source.NamedPlace,
            PaymentTerms = source.PaymentTerms,
            LeadTimeDays = source.LeadTimeDays,
            ValidFromUtc = source.ValidFromUtc,
            ValidToUtc = source.ValidToUtc,
            ReceivedAtUtc = null,

            /*
             * Dokumentreferensen hör till den gamla supplier-revisionens provenance
             * och ska därför inte ärvas till nästa draft.
             */
            OriginalDocumentReference = null,
            SnapshotJson = null,
            SnapshotHash = null,

            /*
             * Revision-specifika interna anteckningar kopieras inte automatiskt.
             */
            InternalNotes = null
        };

        foreach (var sourceLine in source.Lines.OrderBy(x => x.LineNumber))
        {
            nextRevision.Lines.Add(new SupplierQuoteLine
            {
                Id = Guid.CreateVersion7(),
                SupplierQuoteRevisionId = nextRevision.Id,
                SupplierBusinessRelationshipId = quote.SupplierBusinessRelationshipId,
                BuyerRfqLineId = sourceLine.BuyerRfqLineId,
                Kind = sourceLine.Kind,
                SupplierMaterialOfferingId = sourceLine.SupplierMaterialOfferingId,
                SupplierProcessCapabilityId = sourceLine.SupplierProcessCapabilityId,
                SupplySolutionStepId = sourceLine.SupplySolutionStepId,
                InventoryLotId = sourceLine.InventoryLotId,
                LineNumber = sourceLine.LineNumber,
                Name = sourceLine.Name,
                Description = sourceLine.Description,
                OfferedQuantity = sourceLine.OfferedQuantity,
                QuantityUnitId = sourceLine.QuantityUnitId,
                UnitPrice = sourceLine.UnitPrice,
                FixedFee = sourceLine.FixedFee,
                MinimumOrderQuantity = sourceLine.MinimumOrderQuantity,
                AvailableFromUtc = sourceLine.AvailableFromUtc,
                AvailableToUtc = sourceLine.AvailableToUtc,
                IncotermRuleId = sourceLine.IncotermRuleId,
                NamedPlace = sourceLine.NamedPlace,
                PriceBasis = sourceLine.PriceBasis,
                SupplierLineReference = sourceLine.SupplierLineReference,
                InternalNotes = sourceLine.InternalNotes
            });
        }

        _db.Entry(quote)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRowVersion;

        quote.LatestRevisionNumber = nextRevisionNumber;
        quote.Revisions.Add(nextRevision);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(
                exception,
                "Concurrency conflict while creating next revision for Supplier quote {SupplierQuoteId} from revision {SourceSupplierQuoteRevisionId}",
                quote.Id,
                source.Id);

            return Result.Fail<SupplierQuoteResponse>(
                new ConcurrencyError(nameof(SupplierQuote), quote.Id));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            _logger.LogWarning(
                exception,
                "Concurrent Draft-revision conflict while creating next revision for Supplier quote {SupplierQuoteId}",
                quote.Id);

            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError("Another Draft SupplierQuoteRevision was created concurrently."));
        }

        _logger.LogInformation(
            "Supplier quote {SupplierQuoteId} revision {SupplierQuoteRevisionId} created as Draft from revision {SourceSupplierQuoteRevisionId}. RevisionNumber={RevisionNumber}, LineCount={LineCount}",
            quote.Id,
            nextRevision.Id,
            source.Id,
            nextRevision.RevisionNumber,
            nextRevision.Lines.Count);

        return Result.Ok(
            await ProjectRequiredQuoteAsync(
                quote.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kontrollerar revisionens status, fullständighet och provenance innan ReceiveRevision.
    /// </summary>
    public async Task<Result<SupplierQuoteRevisionReceiveReadinessResponse>> GetReceiveReadinessAsync(
        Guid supplierQuoteRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revision = await _db.Set<SupplierQuoteRevision>()
            .AsNoTracking()
            .Include(x => x.SupplierQuote)
            .Include(x => x.Lines)
            .AsSplitQuery()
            .SingleOrDefaultAsync(x => x.Id == supplierQuoteRevisionId, cancellationToken);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteRevisionReceiveReadinessResponse>(
                new NotFoundError(nameof(SupplierQuoteRevision), supplierQuoteRevisionId));
        }

        var issues = new List<MatchingReadinessIssueResponse>();

        if (revision.Status != SupplierQuoteRevisionStatus.Draft)
        {
            AddIssue(
                issues,
                "RevisionNotDraft",
                $"SupplierQuoteRevision has status '{revision.Status}'.",
                revision.Id);
        }

        if (revision.RevisionNumber != revision.SupplierQuote.LatestRevisionNumber)
        {
            AddIssue(
                issues,
                "RevisionNotLatest",
                "Only the latest SupplierQuoteRevision can be received.",
                revision.Id);
        }

        if (revision.ReceivedAtUtc.HasValue
            || revision.SnapshotJson is not null
            || revision.SnapshotHash is not null)
        {
            AddIssue(
                issues,
                "RevisionAlreadyFrozen",
                "Draft SupplierQuoteRevision already contains received snapshot data.",
                revision.Id);
        }

        if (revision.Lines.Count == 0)
        {
            AddIssue(
                issues,
                "RevisionHasNoLines",
                "SupplierQuoteRevision must contain at least one SupplierQuoteLine.",
                revision.Id);
        }

        var revisionReferences = await _referenceValidator.ValidateRevisionReferencesAsync(
            revision.CurrencyId,
            revision.IncotermRuleId,
            cancellationToken);

        foreach (var error in revisionReferences.Errors)
        {
            AddIssue(
                issues,
                "InvalidRevisionReference",
                error.Message,
                revision.Id);
        }

        foreach (var line in revision.Lines.OrderBy(x => x.LineNumber))
        {
            var lineReferences = await _referenceValidator.ValidateLineReferencesAsync(
                revision.SupplierBusinessRelationshipId,
                revision.SupplierQuote.BuyerRfqRevisionId,
                line.BuyerRfqLineId,
                line.SupplierMaterialOfferingId,
                line.SupplierProcessCapabilityId,
                line.SupplySolutionStepId,
                line.InventoryLotId,
                line.QuantityUnitId,
                line.IncotermRuleId,
                cancellationToken);

            foreach (var error in lineReferences.Errors)
            {
                AddIssue(
                    issues,
                    "InvalidLineReference",
                    $"Line {line.LineNumber}: {error.Message}",
                    line.Id);
            }
        }

        _logger.LogDebug(
            "Supplier quote revision {SupplierQuoteRevisionId} receive readiness evaluated. IsReady={IsReady}, IssueCount={IssueCount}, IssueCodes={IssueCodes}",
            revision.Id,
            issues.Count == 0,
            issues.Count,
            string.Join(",", issues.Select(x => x.Code)));

        return Result.Ok(
            new SupplierQuoteRevisionReceiveReadinessResponse
            {
                SupplierQuoteRevisionId = revision.Id,
                IsReady = issues.Count == 0,
                Issues = issues
            });
    }

    /// <summary>
    /// Fryser senaste Draft-revision, supersede:ar eventuell aktuell Received-revision och markerar quoten som Received.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> ReceiveRevisionAsync(
        Guid supplierQuoteRevisionId,
        ReceiveSupplierQuoteRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(validation.Errors);
        }

        var revision = await _db.Set<SupplierQuoteRevision>()
            .Include(x => x.SupplierQuote)
            .SingleOrDefaultAsync(x => x.Id == supplierQuoteRevisionId, cancellationToken);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(nameof(SupplierQuoteRevision), supplierQuoteRevisionId));
        }

        /*
         * Retry efter en lyckad receive-operation får returnera befintligt resultat.
         */
        if (revision.Status == SupplierQuoteRevisionStatus.Received
            && !string.IsNullOrWhiteSpace(revision.SnapshotJson)
            && !string.IsNullOrWhiteSpace(revision.SnapshotHash))
        {
            var computedHash = _snapshotService.ComputeHash(revision.SnapshotJson);

            if (!string.Equals(
                    computedHash,
                    revision.SnapshotHash,
                    StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Stored Supplier quote revision {SupplierQuoteRevisionId} snapshot verification failed during idempotent receive retry",
                    revision.Id);

                return Result.Fail<SupplierQuoteResponse>(
                    new ConflictError("Stored SupplierQuoteRevision snapshot is invalid."));
            }

            return Result.Ok(
                await ProjectRequiredQuoteAsync(
                    revision.SupplierQuoteId,
                    cancellationToken));
        }

        if (revision.Status != SupplierQuoteRevisionStatus.Draft)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError(
                    $"SupplierQuoteRevision with status '{revision.Status}' cannot be received."));
        }

        var readiness = await GetReceiveReadinessAsync(
            revision.Id,
            cancellationToken);

        if (readiness.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(readiness.Errors);
        }

        if (!readiness.Value.IsReady)
        {
            _logger.LogInformation(
                "Supplier quote revision {SupplierQuoteRevisionId} receive blocked by readiness validation. IssueCount={IssueCount}, IssueCodes={IssueCodes}",
                revision.Id,
                readiness.Value.Issues.Count,
                string.Join(",", readiness.Value.Issues.Select(x => x.Code)));

            return Result.Fail<SupplierQuoteResponse>(
                readiness.Value.Issues.Select(
                    x => new ConflictError($"{x.Code}: {x.Message}")));
        }

        var snapshot = await _snapshotService.CreateAsync(
            revision.Id,
            cancellationToken);

        var quote = revision.SupplierQuote;
        var receivedAtUtc = request.ReceivedAtUtc ?? DateTimeOffset.UtcNow;

        _db.Entry(quote)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRowVersion;

        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRevisionRowVersion;

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await SupersedePreviousCurrentRevisionAsync(
                quote,
                revision,
                receivedAtUtc,
                cancellationToken);

            revision.Status = SupplierQuoteRevisionStatus.Received;
            revision.ReceivedAtUtc = receivedAtUtc;
            revision.SnapshotJson = snapshot.Json;
            revision.SnapshotHash = snapshot.Hash;
            revision.ActionDateUtc = receivedAtUtc;

            /*
             * En ny supplier-revision innebär att root-workflow åter befinner sig i Received.
             * Review/Accept/Reject hanteras i nästa lifecycle-slice.
             */
            quote.Status = SupplierQuoteStatus.Received;
            quote.ReceivedAtUtc ??= receivedAtUtc;
            quote.ActionDateUtc = receivedAtUtc;

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            _logger.LogWarning(
                exception,
                "Concurrency conflict while receiving Supplier quote {SupplierQuoteId} revision {SupplierQuoteRevisionId}",
                quote.Id,
                revision.Id);

            var error = exception.Entries.Any(x => x.Entity is SupplierQuote)
                ? new ConcurrencyError(nameof(SupplierQuote), quote.Id)
                : new ConcurrencyError(nameof(SupplierQuoteRevision), revision.Id);

            return Result.Fail<SupplierQuoteResponse>(error);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);

            _logger.LogWarning(
                exception,
                "Concurrent current-revision conflict while receiving Supplier quote {SupplierQuoteId} revision {SupplierQuoteRevisionId}",
                quote.Id,
                revision.Id);

            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError("Another SupplierQuoteRevision became current concurrently."));
        }

        _logger.LogInformation(
            "Supplier quote {SupplierQuoteId} revision {SupplierQuoteRevisionId} received and frozen. RevisionNumber={RevisionNumber}, ReceivedAtUtc={ReceivedAtUtc}",
            quote.Id,
            revision.Id,
            revision.RevisionNumber,
            revision.ReceivedAtUtc);

        return Result.Ok(
            await ProjectRequiredQuoteAsync(
                quote.Id,
                cancellationToken));
    }

    /// <summary>
    /// Verifierar att ett sparat supplier-quote-snapshot fortfarande matchar sin SHA-256-hash.
    /// </summary>
    public async Task<Result<SupplierQuoteRevisionSnapshotVerificationResponse>> VerifyRevisionSnapshotAsync(
        Guid supplierQuoteRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revision = await _db.Set<SupplierQuoteRevision>()
            .AsNoTracking()
            .Where(x => x.Id == supplierQuoteRevisionId)
            .Select(x => new
            {
                x.Id,
                x.SnapshotJson,
                x.SnapshotHash
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteRevisionSnapshotVerificationResponse>(
                new NotFoundError(nameof(SupplierQuoteRevision), supplierQuoteRevisionId));
        }

        if (string.IsNullOrWhiteSpace(revision.SnapshotJson)
            || string.IsNullOrWhiteSpace(revision.SnapshotHash))
        {
            return Result.Fail<SupplierQuoteRevisionSnapshotVerificationResponse>(
                new ConflictError("SupplierQuoteRevision has not been received and frozen."));
        }

        var computedHash = _snapshotService.ComputeHash(revision.SnapshotJson);
        var isValid = string.Equals(
            revision.SnapshotHash,
            computedHash,
            StringComparison.OrdinalIgnoreCase);

        if (isValid)
        {
            _logger.LogDebug(
                "Supplier quote revision {SupplierQuoteRevisionId} snapshot verification passed",
                revision.Id);
        }
        else
        {
            _logger.LogWarning(
                "Supplier quote revision {SupplierQuoteRevisionId} snapshot verification failed",
                revision.Id);
        }

        return Result.Ok(
            new SupplierQuoteRevisionSnapshotVerificationResponse
            {
                SupplierQuoteRevisionId = revision.Id,
                StoredHash = revision.SnapshotHash,
                ComputedHash = computedHash,
                IsValid = isValid
            });
    }

    /// <summary>
    /// Fryser senaste Draft-revision och supersede:ar eventuell tidigare aktuell kommersiell revision.
    /// </summary>
    private async Task SupersedePreviousCurrentRevisionAsync(
        SupplierQuote quote,
        SupplierQuoteRevision newRevision,
        DateTimeOffset actionDateUtc,
        CancellationToken cancellationToken)
    {
        var previousCurrentRevision = await _db.Set<SupplierQuoteRevision>()
            .SingleOrDefaultAsync(
                x => x.SupplierQuoteId == quote.Id
                    && x.Id != newRevision.Id
                    && (
                        x.Status == SupplierQuoteRevisionStatus.Received
                        || x.Status == SupplierQuoteRevisionStatus.AcceptedInternally
                    ),
                cancellationToken);

        if (previousCurrentRevision is null)
        {
            return;
        }

        previousCurrentRevision.Status = SupplierQuoteRevisionStatus.Superseded;
        previousCurrentRevision.ActionDateUtc = actionDateUtc;

        /*
         * Sparas innan nya revisionen sätts till Received så att partial unique-indexet
         * aldrig behöver se två current commercial revisions samtidigt.
         */
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Kontrollerar att revisionen fortfarande får modifieras.
    /// </summary>
    private static Result ValidateRevisionIsWritable(SupplierQuoteRevision revision)
    {
        if (revision.Status != SupplierQuoteRevisionStatus.Draft)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplierQuoteRevision with status '{revision.Status}' is immutable."));
        }

        if (revision.ReceivedAtUtc.HasValue
            || revision.SnapshotJson is not null
            || revision.SnapshotHash is not null)
        {
            return Result.Fail(
                new ConflictError(
                    "SupplierQuoteRevision contains received snapshot data and is immutable."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Lägger till ett readiness-problem.
    /// </summary>
    private static void AddIssue(
        ICollection<MatchingReadinessIssueResponse> issues,
        string code,
        string message,
        Guid? entityId = null)
    {
        issues.Add(
            new MatchingReadinessIssueResponse
            {
                Code = code,
                Message = message,
                EntityId = entityId
            });
    }

    /// <summary>
    /// Projekterar en supplier quote.
    /// </summary>
    private Task<SupplierQuoteResponse?> ProjectQuoteAsync(
        Guid supplierQuoteId,
        CancellationToken cancellationToken)
    {
        return _db.Set<SupplierQuote>()
            .AsNoTracking()
            .Where(x => x.Id == supplierQuoteId)
            .Select(SupplierQuoteProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar en supplier-quote-revision.
    /// </summary>
    private Task<SupplierQuoteRevisionResponse?> ProjectRevisionAsync(
        Guid supplierQuoteRevisionId,
        CancellationToken cancellationToken)
    {
        return _db.Set<SupplierQuoteRevision>()
            .AsNoTracking()
            .Where(x => x.Id == supplierQuoteRevisionId)
            .Select(SupplierQuoteProjections.RevisionDetails)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar en sparad supplier quote och kastar om den oväntat saknas.
    /// </summary>
    private async Task<SupplierQuoteResponse> ProjectRequiredQuoteAsync(
        Guid supplierQuoteId,
        CancellationToken cancellationToken)
    {
        return await ProjectQuoteAsync(supplierQuoteId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"SupplierQuote '{supplierQuoteId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Projekterar en sparad revision och kastar om den oväntat saknas.
    /// </summary>
    private async Task<SupplierQuoteRevisionResponse> ProjectRequiredRevisionAsync(
        Guid supplierQuoteRevisionId,
        CancellationToken cancellationToken)
    {
        return await ProjectRevisionAsync(supplierQuoteRevisionId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"SupplierQuoteRevision '{supplierQuoteRevisionId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Lägger på stabil sortering innan paginering utförs.
    /// </summary>
    private static IQueryable<SupplierQuote> ApplySorting(
        IQueryable<SupplierQuote> query,
        SupplierQuoteSortField sortBy,
        SortDirection sortDirection)
    {
        return (sortBy, sortDirection) switch
        {
            (SupplierQuoteSortField.QuoteNumber, SortDirection.Descending) =>
                query.OrderByDescending(x => x.QuoteNumber)
                    .ThenByDescending(x => x.Id),

            (SupplierQuoteSortField.Name, SortDirection.Ascending) =>
                query.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id),

            (SupplierQuoteSortField.Name, SortDirection.Descending) =>
                query.OrderByDescending(x => x.Name)
                    .ThenByDescending(x => x.Id),

            (SupplierQuoteSortField.SupplierName, SortDirection.Ascending) =>
                query.OrderBy(x => x.SupplierBusinessRelationship.Counterparty.Name)
                    .ThenBy(x => x.Id),

            (SupplierQuoteSortField.SupplierName, SortDirection.Descending) =>
                query.OrderByDescending(x => x.SupplierBusinessRelationship.Counterparty.Name)
                    .ThenByDescending(x => x.Id),

            (SupplierQuoteSortField.ReceivedAtUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.ReceivedAtUtc)
                    .ThenBy(x => x.Id),

            (SupplierQuoteSortField.ReceivedAtUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.ReceivedAtUtc)
                    .ThenByDescending(x => x.Id),

            (SupplierQuoteSortField.CreateDateUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.CreateDateUtc)
                    .ThenBy(x => x.Id),

            (SupplierQuoteSortField.CreateDateUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.CreateDateUtc)
                    .ThenByDescending(x => x.Id),

            (SupplierQuoteSortField.LatestRevisionNumber, SortDirection.Ascending) =>
                query.OrderBy(x => x.LatestRevisionNumber)
                    .ThenBy(x => x.Id),

            (SupplierQuoteSortField.LatestRevisionNumber, SortDirection.Descending) =>
                query.OrderByDescending(x => x.LatestRevisionNumber)
                    .ThenByDescending(x => x.Id),

            _ =>
                query.OrderBy(x => x.QuoteNumber)
                    .ThenBy(x => x.Id)
        };
    }

    /// <summary>
    /// Kontrollerar om PostgreSQL rapporterade en unique violation.
    /// </summary>
    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };
    }

    /// <summary>
    /// Trimmar text och ersätter whitespace med null.
    /// </summary>
    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}