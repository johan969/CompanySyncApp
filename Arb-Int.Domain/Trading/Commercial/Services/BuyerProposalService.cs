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
/// Hanterar buyer-proposal-roten och versionsövergången från Draft till immutable Issued.
/// </summary>
public sealed class BuyerProposalService : IBuyerProposalService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly IBuyerProposalReferenceValidator _referenceValidator;
    private readonly IBuyerProposalRevisionSnapshotService _snapshotService;
    private readonly ISupplySolutionCandidateSnapshotService _candidateSnapshotService;
    private readonly ILogger<BuyerProposalService> _logger;

    /// <summary>
    /// Skapar proposal-servicen med validation-, provenance- och snapshot-beroenden.
    /// </summary>
    public BuyerProposalService(
        Arb_IntDbContext db,
        IValidationService validationService,
        IBuyerProposalReferenceValidator referenceValidator,
        IBuyerProposalRevisionSnapshotService snapshotService,
        ISupplySolutionCandidateSnapshotService candidateSnapshotService,
        ILogger<BuyerProposalService> logger)
    {
        _db = db;
        _validationService = validationService;
        _referenceValidator = referenceValidator;
        _snapshotService = snapshotService;
        _candidateSnapshotService = candidateSnapshotService;
        _logger = logger;
    }

    /// <summary>
    /// Hämtar proposalet med revisionshistorik.
    /// </summary>
    public async Task<Result<BuyerProposalResponse>> GetByIdAsync(
        Guid buyerProposalId,
        CancellationToken cancellationToken = default)
    {
        var response = await ProjectProposalAsync(
            buyerProposalId,
            cancellationToken);

        return response is null
            ? Result.Fail<BuyerProposalResponse>(
                new NotFoundError(
                    nameof(BuyerProposal),
                    buyerProposalId))
            : Result.Ok(response);
    }

    /// <summary>
    /// Söker, filtrerar, sorterar och paginerar buyer proposals för en intern organisation.
    /// </summary>
    public async Task<Result<PagedResult<BuyerProposalListResponse>>> QueryAsync(
        Guid owningOrganizationId,
        BuyerProposalQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<PagedResult<BuyerProposalListResponse>>(
                validationResult.Errors);
        }

        var organizationExists = await _db.Set<Organization>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == owningOrganizationId,
                cancellationToken);

        if (!organizationExists)
        {
            return Result.Fail<PagedResult<BuyerProposalListResponse>>(
                new NotFoundError(
                    nameof(Organization),
                    owningOrganizationId));
        }

        IQueryable<BuyerProposal> query = _db.Set<BuyerProposal>()
            .AsNoTracking()
            .Where(x =>
                x.BuyerBusinessRelationship.OwningOrganizationId == owningOrganizationId);

        if (request.BuyerBusinessRelationshipId.HasValue)
        {
            query = query.Where(x =>
                x.BuyerBusinessRelationshipId == request.BuyerBusinessRelationshipId.Value);
        }

        if (request.BuyerPartyId.HasValue)
        {
            query = query.Where(x =>
                x.BuyerBusinessRelationship.CounterpartyId == request.BuyerPartyId.Value);
        }

        if (request.BuyerRfqId.HasValue)
        {
            query = query.Where(x =>
                x.BuyerRfqId == request.BuyerRfqId.Value);
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
                    x.ProposalNumber,
                    searchPattern)
                || EF.Functions.ILike(
                    x.Name,
                    searchPattern)
                || EF.Functions.ILike(
                    x.BuyerBusinessRelationship.Counterparty.Name,
                    searchPattern)
                || EF.Functions.ILike(
                    x.BuyerRfq.RfqNumber,
                    searchPattern));
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
            .Select(BuyerProposalProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok(
            new PagedResult<BuyerProposalListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }

    /// <summary>
    /// Hämtar en specifik proposal-revision.
    /// </summary>
    public async Task<Result<BuyerProposalRevisionResponse>> GetRevisionByIdAsync(
        Guid buyerProposalRevisionId,
        CancellationToken cancellationToken = default)
    {
        var response = await ProjectRevisionAsync(
            buyerProposalRevisionId,
            cancellationToken);

        return response is null
            ? Result.Fail<BuyerProposalRevisionResponse>(
                new NotFoundError(
                    nameof(BuyerProposalRevision),
                    buyerProposalRevisionId))
            : Result.Ok(response);
    }

    /// <summary>
    /// Skapar proposal-roten och revision 1 från en ReleasedForMatching RFQ-revision.
    /// </summary>
    public async Task<Result<BuyerProposalResponse>> CreateAsync(
        Guid buyerRfqId,
        CreateBuyerProposalRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                validation.Errors);
        }

        var rfq = await _db.Set<BuyerRfq>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqId)
            .Select(x => new
            {
                x.Id,
                x.BuyerBusinessRelationshipId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (rfq is null)
        {
            return Result.Fail<BuyerProposalResponse>(
                new NotFoundError(
                    nameof(BuyerRfq),
                    buyerRfqId));
        }

        var rfqRevisionResult = await _referenceValidator.ValidateBuyerRfqRevisionAsync(
            rfq.Id,
            request.BuyerRfqRevisionId,
            cancellationToken);

        if (rfqRevisionResult.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                rfqRevisionResult.Errors);
        }

        var currencyResult = await _referenceValidator.ValidateCurrencyAsync(
            request.CurrencyId,
            cancellationToken);

        if (currencyResult.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                currencyResult.Errors);
        }

        var disclosureResult = await _referenceValidator.ValidateDisclosureReferenceAsync(
            rfq.BuyerBusinessRelationshipId,
            request.DisclosurePackageRevisionId,
            cancellationToken);

        if (disclosureResult.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                disclosureResult.Errors);
        }

        var proposalNumber = request.ProposalNumber.Trim();

        var duplicate = await _db.Set<BuyerProposal>()
            .AsNoTracking()
            .AnyAsync(
                x => x.ProposalNumber == proposalNumber,
                cancellationToken);

        if (duplicate)
        {
            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    $"BuyerProposal number '{proposalNumber}' already exists."));
        }

        var proposal = new BuyerProposal
        {
            Id = Guid.CreateVersion7(),
            BuyerBusinessRelationshipId = rfq.BuyerBusinessRelationshipId,
            BuyerRfqId = rfq.Id,
            ProposalNumber = proposalNumber,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Status = BuyerProposalStatus.Draft,
            LatestRevisionNumber = 1,
            OwnerUserId = request.OwnerUserId,
            IssuedAtUtc = null,
            ClosedAtUtc = null,
            InternalNotes = TrimToNull(request.InternalNotes)
        };

        var revision = new BuyerProposalRevision
        {
            Id = Guid.CreateVersion7(),
            BuyerProposalId = proposal.Id,
            BuyerRfqId = rfq.Id,
            BuyerRfqRevisionId = request.BuyerRfqRevisionId,
            RevisionNumber = 1,
            Status = BuyerProposalRevisionStatus.Draft,
            CurrencyId = request.CurrencyId,
            ValidFromUtc = request.ValidFromUtc,
            ValidToUtc = request.ValidToUtc,
            CommercialTerms = TrimToNull(request.CommercialTerms),
            BuyerFacingNotes = TrimToNull(request.BuyerFacingNotes),
            InternalNotes = TrimToNull(request.RevisionInternalNotes),
            DisclosurePackageRevisionId = request.DisclosurePackageRevisionId,
            IssuedAtUtc = null,
            IssuedByUserId = null,
            SnapshotJson = null,
            SnapshotHash = null
        };

        proposal.Revisions.Add(revision);

        _db.Set<BuyerProposal>().Add(proposal);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            _logger.LogWarning(
                exception,
                "Concurrent unique-key conflict while creating Buyer proposal with number {BuyerProposalNumber}",
                proposalNumber);

            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    $"BuyerProposal number '{proposalNumber}' already exists."));
        }

        _logger.LogInformation(
            "Buyer proposal {BuyerProposalId} revision {BuyerProposalRevisionId} created in Draft for Buyer RFQ {BuyerRfqId} revision {BuyerRfqRevisionId}. BusinessRelationshipId={BusinessRelationshipId}",
            proposal.Id,
            revision.Id,
            proposal.BuyerRfqId,
            revision.BuyerRfqRevisionId,
            proposal.BuyerBusinessRelationshipId);

        return Result.Ok(
            await ProjectRequiredProposalAsync(
                proposal.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar root-metadata utan att ändra RFQ, buyer-relation eller ProposalNumber.
    /// </summary>
    public async Task<Result<BuyerProposalResponse>> UpdateAsync(
        Guid buyerProposalId,
        UpdateBuyerProposalRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                validation.Errors);
        }

        var proposal = await _db.Set<BuyerProposal>()
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalId,
                cancellationToken);

        if (proposal is null)
        {
            return Result.Fail<BuyerProposalResponse>(
                new NotFoundError(
                    nameof(BuyerProposal),
                    buyerProposalId));
        }

        _db.Entry(proposal)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        proposal.Name = request.Name.Trim();
        proposal.Description = request.Description.Trim();
        proposal.OwnerUserId = request.OwnerUserId;
        proposal.InternalNotes = TrimToNull(request.InternalNotes);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(
                exception,
                "Concurrency conflict while updating Buyer proposal {BuyerProposalId}",
                proposal.Id);

            return Result.Fail<BuyerProposalResponse>(
                new ConcurrencyError(
                    nameof(BuyerProposal),
                    proposal.Id));
        }

        _logger.LogInformation(
            "Buyer proposal {BuyerProposalId} metadata updated",
            proposal.Id);

        return Result.Ok(
            await ProjectRequiredProposalAsync(
                proposal.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar kommersiella huvudvillkor på en Draft proposal-revision.
    /// <para>
    /// Revisionens CurrencyId är styrande även för samtliga options.<br />
    /// Revision och options uppdateras därför atomärt i samma databastransaktion.
    /// </para>
    /// </summary>
    public async Task<Result<BuyerProposalRevisionResponse>> UpdateRevisionAsync(
        Guid buyerProposalRevisionId,
        UpdateBuyerProposalRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                validation.Errors);
        }

        var revision = await _db.Set<BuyerProposalRevision>()
            .Include(x => x.BuyerProposal)
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalRevisionId,
                cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new NotFoundError(
                    nameof(BuyerProposalRevision),
                    buyerProposalRevisionId));
        }

        var writableResult = ValidateRevisionIsWritable(
            revision);

        if (writableResult.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                writableResult.Errors);
        }

        var currencyResult = await _referenceValidator.ValidateCurrencyAsync(
            request.CurrencyId,
            cancellationToken);

        if (currencyResult.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                currencyResult.Errors);
        }

        var disclosureResult = await _referenceValidator.ValidateDisclosureReferenceAsync(
            revision.BuyerProposal.BuyerBusinessRelationshipId,
            request.DisclosurePackageRevisionId,
            cancellationToken);

        if (disclosureResult.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                disclosureResult.Errors);
        }

        /*
         * Requestens RowVersion används som originalvärde även om entiteten
         * hämtats med ett senare xmin-värde i den aktuella DbContext-instansen.
         */
        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        await using var transaction = await _db.Database.BeginTransactionAsync(
            cancellationToken);

        try
        {
            revision.CurrencyId = request.CurrencyId;
            revision.ValidFromUtc = request.ValidFromUtc;
            revision.ValidToUtc = request.ValidToUtc;
            revision.CommercialTerms = TrimToNull(request.CommercialTerms);
            revision.BuyerFacingNotes = TrimToNull(request.BuyerFacingNotes);
            revision.InternalNotes = TrimToNull(request.InternalNotes);
            revision.DisclosurePackageRevisionId = request.DisclosurePackageRevisionId;

            /*
             * CurrencyId på optionen är härledd från parent-revisionen.
             * Alla befintliga Draft-options synkroniseras när revisionens valuta ändras.
             *
             * ExecuteUpdateAsync kör SQL direkt och väntar inte på SaveChangesAsync.
             * Transaktionen gör därför option-uppdateringen och revision-uppdateringen atomära.
             */
            await _db.Set<BuyerProposalOption>()
                .Where(x => x.BuyerProposalRevisionId == revision.Id)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            x => x.CurrencyId,
                            request.CurrencyId)
                        .SetProperty(
                            x => x.ActionDateUtc,
                            DateTimeOffset.UtcNow),
                    cancellationToken);

            TouchRevision(
                revision);

            /*
             * SaveChanges använder revisionens RowVersion som concurrency-guard.
             *
             * Om exempelvis en samtidig line-mutation hunnit TouchRevision efter att
             * requesten lästes får denna UPDATE en DbUpdateConcurrencyException.
             * Option-uppdateringen ovan rullas då också tillbaka av transaktionen.
             */
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
                "Concurrency conflict while updating Buyer proposal revision {BuyerProposalRevisionId}",
                revision.Id);

            return Result.Fail<BuyerProposalRevisionResponse>(
                new ConcurrencyError(
                    nameof(BuyerProposalRevision),
                    revision.Id));
        }

        _logger.LogInformation(
            "Buyer proposal revision {BuyerProposalRevisionId} metadata updated",
            revision.Id);

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                revision.Id,
                cancellationToken));
    }

    /// <summary>
    /// Skapar nästa Draft-revision och deep-klonar options endast när RFQ-revisionen är oförändrad.
    /// </summary>
    public async Task<Result<BuyerProposalResponse>> CreateNextRevisionAsync(
        Guid buyerProposalId,
        CreateNextBuyerProposalRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                validation.Errors);
        }

        var proposal = await _db.Set<BuyerProposal>()
            .Include(x => x.Revisions)
                .ThenInclude(x => x.Options)
                    .ThenInclude(x => x.Lines)
            .AsSplitQuery()
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalId,
                cancellationToken);

        if (proposal is null)
        {
            return Result.Fail<BuyerProposalResponse>(
                new NotFoundError(
                    nameof(BuyerProposal),
                    buyerProposalId));
        }

        if (proposal.Status != BuyerProposalStatus.Issued)
        {
            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    $"A new revision can only be created for an Issued BuyerProposal. Current status is '{proposal.Status}'."));
        }

        if (proposal.Revisions.Any(
                x => x.Status == BuyerProposalRevisionStatus.Draft))
        {
            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    "BuyerProposal already has a Draft revision."));
        }

        var targetRfqResult = await _referenceValidator.ValidateBuyerRfqRevisionAsync(
            proposal.BuyerRfqId,
            request.BuyerRfqRevisionId,
            cancellationToken);

        if (targetRfqResult.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                targetRfqResult.Errors);
        }

        var source = proposal.Revisions
            .SingleOrDefault(
                x => x.RevisionNumber == proposal.LatestRevisionNumber);

        if (source is null
            || source.Status != BuyerProposalRevisionStatus.Issued)
        {
            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    "Latest BuyerProposalRevision must be Issued before a new Draft can be created."));
        }

        if (string.IsNullOrWhiteSpace(source.SnapshotJson)
            || string.IsNullOrWhiteSpace(source.SnapshotHash))
        {
            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    "Latest BuyerProposalRevision has no immutable snapshot."));
        }

        var sourceHash = _snapshotService.ComputeHash(
            source.SnapshotJson);

        if (!string.Equals(
                sourceHash,
                source.SnapshotHash,
                StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Buyer proposal {BuyerProposalId} cannot create a new revision because source revision {SourceBuyerProposalRevisionId} snapshot verification failed",
                proposal.Id,
                source.Id);

            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    "Latest BuyerProposalRevision snapshot hash is invalid."));
        }

        var pendingAcceptanceExists = await _db.Set<BuyerProposalAcceptance>()
            .AsNoTracking()
            .AnyAsync(
                x => x.BuyerProposalRevisionId == source.Id
                    && x.Status == BuyerProposalAcceptanceStatus.PendingInternalApproval,
                cancellationToken);

        if (pendingAcceptanceExists)
        {
            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    "A new BuyerProposalRevision cannot be created while the current issued revision has an acceptance pending internal approval."));
        }

        var nextRevisionNumber =
            proposal.LatestRevisionNumber + 1;

        var nextRevision = new BuyerProposalRevision
        {
            Id = Guid.CreateVersion7(),
            BuyerProposalId = proposal.Id,
            BuyerRfqId = proposal.BuyerRfqId,
            BuyerRfqRevisionId = request.BuyerRfqRevisionId,
            RevisionNumber = nextRevisionNumber,
            Status = BuyerProposalRevisionStatus.Draft,
            CurrencyId = source.CurrencyId,
            ValidFromUtc = source.ValidFromUtc,
            ValidToUtc = source.ValidToUtc,
            CommercialTerms = source.CommercialTerms,
            BuyerFacingNotes = source.BuyerFacingNotes,
            InternalNotes = null,

            /*
             * Disclosure måste väljas och valideras explicit för varje ny proposal-revision.
             */
            DisclosurePackageRevisionId = null,

            IssuedAtUtc = null,
            IssuedByUserId = null,
            SnapshotJson = null,
            SnapshotHash = null
        };

        /*
         * Gamla RFQ-line-ID:n får endast klonas om proposalet fortfarande
         * bygger på exakt samma RFQ-revision.
         */
        if (source.BuyerRfqRevisionId == request.BuyerRfqRevisionId)
        {
            foreach (var sourceOption in source.Options
                         .OrderBy(x => x.OptionNumber))
            {
                var nextOption = new BuyerProposalOption
                {
                    Id = Guid.CreateVersion7(),
                    BuyerProposalRevisionId = nextRevision.Id,
                    SupplySolutionCandidateId = sourceOption.SupplySolutionCandidateId,
                    OptionNumber = sourceOption.OptionNumber,
                    Name = sourceOption.Name,
                    Description = sourceOption.Description,
                    Status = BuyerProposalOptionStatus.Draft,
                    CurrencyId = source.CurrencyId,
                    EstimatedLeadTimeDays = sourceOption.EstimatedLeadTimeDays,
                    BuyerFacingSummary = sourceOption.BuyerFacingSummary,

                    /*
                     * Margin-snapshot ska räknas om för den nya kommersiella revisionen.
                     */
                    InternalMarginSnapshotJson = null,
                    TotalNetAmount = 0m,
                    TotalTaxAmount = null,
                    TotalGrossAmount = 0m
                };

                foreach (var sourceLine in sourceOption.Lines
                             .OrderBy(x => x.LineNumber))
                {
                    nextOption.Lines.Add(
                        new BuyerProposalLine
                        {
                            Id = Guid.CreateVersion7(),
                            BuyerProposalOptionId = nextOption.Id,
                            BuyerRfqLineId = sourceLine.BuyerRfqLineId,
                            SupplySolutionLineId = sourceLine.SupplySolutionLineId,
                            MaterialId = sourceLine.MaterialId,
                            ProductId = sourceLine.ProductId,
                            LineNumber = sourceLine.LineNumber,
                            Name = sourceLine.Name,
                            Description = sourceLine.Description,
                            Quantity = sourceLine.Quantity,
                            QuantityUnitId = sourceLine.QuantityUnitId,
                            UnitPrice = sourceLine.UnitPrice,
                            NetAmount = sourceLine.NetAmount,
                            TaxPercent = sourceLine.TaxPercent,
                            TaxAmount = sourceLine.TaxAmount,
                            GrossAmount = sourceLine.GrossAmount,
                            EstimatedDeliveryFromUtc = sourceLine.EstimatedDeliveryFromUtc,
                            EstimatedDeliveryToUtc = sourceLine.EstimatedDeliveryToUtc,
                            BuyerFacingNotes = sourceLine.BuyerFacingNotes
                        });
                }

                RecalculateOptionTotals(
                    nextOption);

                nextRevision.Options.Add(
                    nextOption);
            }
        }

        _db.Entry(proposal)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerProposalRowVersion;

        proposal.LatestRevisionNumber =
            nextRevisionNumber;

        proposal.Revisions.Add(
            nextRevision);

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(
                exception,
                "Concurrency conflict while creating next revision for Buyer proposal {BuyerProposalId} from revision {SourceBuyerProposalRevisionId}",
                proposal.Id,
                source.Id);

            return Result.Fail<BuyerProposalResponse>(
                new ConcurrencyError(
                    nameof(BuyerProposal),
                    proposal.Id));
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            _logger.LogWarning(
                exception,
                "Concurrent Draft-revision conflict while creating next revision for Buyer proposal {BuyerProposalId}",
                proposal.Id);

            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    "Another Draft BuyerProposalRevision was created concurrently."));
        }

        _logger.LogInformation(
            "Buyer proposal {BuyerProposalId} revision {BuyerProposalRevisionId} created as Draft from issued revision {SourceBuyerProposalRevisionId}. RevisionNumber={RevisionNumber}, BuyerRfqRevisionId={BuyerRfqRevisionId}, OptionCount={OptionCount}",
            proposal.Id,
            nextRevision.Id,
            source.Id,
            nextRevision.RevisionNumber,
            nextRevision.BuyerRfqRevisionId,
            nextRevision.Options.Count);

        return Result.Ok(
            await ProjectRequiredProposalAsync(
                proposal.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kontrollerar hela proposal-aggregatet och alla externa provenance-referenser före issue.
    /// </summary>
    public async Task<Result<BuyerProposalIssueReadinessResponse>> GetIssueReadinessAsync(
        Guid buyerProposalRevisionId,
        DateTimeOffset evaluatedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var revision = await _db.Set<BuyerProposalRevision>()
            .AsNoTracking()
            .Include(x => x.BuyerProposal)
            .Include(x => x.Options)
                .ThenInclude(x => x.Lines)
            .AsSplitQuery()
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalRevisionId,
                cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerProposalIssueReadinessResponse>(
                new NotFoundError(
                    nameof(BuyerProposalRevision),
                    buyerProposalRevisionId));
        }

        var proposal =
            revision.BuyerProposal;

        var issues =
            new List<BuyerProposalReadinessIssueResponse>();

        if (proposal.Status is not (
            BuyerProposalStatus.Draft
            or BuyerProposalStatus.Issued))
        {
            AddIssue(
                issues,
                "ProposalStatusNotIssuable",
                $"BuyerProposal with status '{proposal.Status}' cannot issue a new revision.",
                proposal.Id);
        }

        if (revision.Status != BuyerProposalRevisionStatus.Draft)
        {
            AddIssue(
                issues,
                "RevisionNotDraft",
                $"BuyerProposalRevision has status '{revision.Status}'.",
                revision.Id);
        }

        if (revision.RevisionNumber != proposal.LatestRevisionNumber)
        {
            AddIssue(
                issues,
                "RevisionNotLatest",
                "Only the latest BuyerProposalRevision can be issued.",
                revision.Id);
        }

        if (revision.IssuedAtUtc.HasValue
            || revision.SnapshotJson is not null
            || revision.SnapshotHash is not null)
        {
            AddIssue(
                issues,
                "RevisionAlreadyFrozen",
                "Draft BuyerProposalRevision already contains issue metadata or snapshot data.",
                revision.Id);
        }

        if (revision.ValidToUtc.HasValue
            && revision.ValidToUtc.Value <= evaluatedAtUtc)
        {
            AddIssue(
                issues,
                "RevisionAlreadyExpired",
                $"BuyerProposalRevision ValidToUtc '{revision.ValidToUtc.Value:O}' has already passed.",
                revision.Id);
        }

        var rfqResult =
            await _referenceValidator.ValidateBuyerRfqRevisionAsync(
                proposal.BuyerRfqId,
                revision.BuyerRfqRevisionId,
                cancellationToken);

        foreach (var error in rfqResult.Errors)
        {
            AddIssue(
                issues,
                "InvalidBuyerRfqRevision",
                error.Message,
                revision.BuyerRfqRevisionId);
        }

        var disclosureResult =
            await ValidateDisclosureForIssueAsync(
                proposal,
                revision,
                cancellationToken);

        foreach (var error in disclosureResult.Errors)
        {
            AddIssue(
                issues,
                "InvalidDisclosurePackage",
                error.Message,
                revision.DisclosurePackageRevisionId);
        }

        if (revision.Options.Count == 0)
        {
            AddIssue(
                issues,
                "NoOptions",
                "BuyerProposalRevision must contain at least one option.",
                revision.Id);
        }

        foreach (var option in revision.Options
                     .OrderBy(x => x.OptionNumber))
        {
            if (option.Status != BuyerProposalOptionStatus.Draft)
            {
                AddIssue(
                    issues,
                    "OptionNotDraft",
                    $"Option {option.OptionNumber} has status '{option.Status}'.",
                    option.Id);
            }

            if (option.CurrencyId != revision.CurrencyId)
            {
                AddIssue(
                    issues,
                    "OptionCurrencyMismatch",
                    $"Option {option.OptionNumber} uses another currency than the proposal revision.",
                    option.Id);
            }

            var candidateResult =
                await _referenceValidator.ValidateCandidateAsync(
                    revision.BuyerRfqRevisionId,
                    option.SupplySolutionCandidateId,
                    evaluatedAtUtc,
                    cancellationToken);

            foreach (var error in candidateResult.Errors)
            {
                AddIssue(
                    issues,
                    "InvalidSupplySolutionCandidate",
                    $"Option {option.OptionNumber}: {error.Message}",
                    option.SupplySolutionCandidateId);
            }

            ValidateOptionAmounts(
                option,
                issues);

            if (option.Lines.Count == 0)
            {
                AddIssue(
                    issues,
                    "OptionHasNoLines",
                    $"Option {option.OptionNumber} must contain at least one proposal line.",
                    option.Id);

                continue;
            }

            foreach (var line in option.Lines
                         .OrderBy(x => x.LineNumber))
            {
                ValidateLineAmounts(
                    line,
                    issues);

                var lineResult =
                    await _referenceValidator.ValidateLineAsync(
                        revision.BuyerRfqRevisionId,
                        option.SupplySolutionCandidateId,
                        line.BuyerRfqLineId,
                        line.SupplySolutionLineId,
                        line.MaterialId,
                        line.ProductId,
                        line.QuantityUnitId,
                        cancellationToken);

                foreach (var error in lineResult.Errors)
                {
                    AddIssue(
                        issues,
                        "InvalidProposalLine",
                        $"Option {option.OptionNumber}, line {line.LineNumber}: {error.Message}",
                        line.Id);
                }
            }
        }

        _logger.LogDebug(
            "Buyer proposal revision {BuyerProposalRevisionId} issue readiness evaluated for proposal {BuyerProposalId}. IsReady={IsReady}, IssueCount={IssueCount}, IssueCodes={IssueCodes}",
            revision.Id,
            proposal.Id,
            issues.Count == 0,
            issues.Count,
            string.Join(",", issues.Select(x => x.Code)));

        return Result.Ok(
            new BuyerProposalIssueReadinessResponse
            {
                BuyerProposalId = proposal.Id,
                BuyerProposalRevisionId = revision.Id,
                IsReady = issues.Count == 0,
                Issues = issues
            });
    }

    /// <summary>
    /// Kontrollerar att lagrade line-belopp fortfarande motsvarar quantity, unit price och tax.
    /// </summary>
    private static void ValidateLineAmounts(
        BuyerProposalLine line,
        ICollection<BuyerProposalReadinessIssueResponse> issues)
    {
        var expected =
            BuyerProposalAmountCalculator.Calculate(
                line.Quantity,
                line.UnitPrice,
                line.TaxPercent);

        if (line.NetAmount != expected.NetAmount)
        {
            AddIssue(
                issues,
                "LineNetAmountMismatch",
                $"Proposal line {line.LineNumber} has an invalid NetAmount.",
                line.Id);
        }

        if (line.TaxAmount != expected.TaxAmount)
        {
            AddIssue(
                issues,
                "LineTaxAmountMismatch",
                $"Proposal line {line.LineNumber} has an invalid TaxAmount.",
                line.Id);
        }

        if (line.GrossAmount != expected.GrossAmount)
        {
            AddIssue(
                issues,
                "LineGrossAmountMismatch",
                $"Proposal line {line.LineNumber} has an invalid GrossAmount.",
                line.Id);
        }
    }

    /// <summary>
    /// Kontrollerar att optionens lagrade totals motsvarar dess lines.
    /// </summary>
    private static void ValidateOptionAmounts(
        BuyerProposalOption option,
        ICollection<BuyerProposalReadinessIssueResponse> issues)
    {
        var expectedNet =
            option.Lines.Sum(x => x.NetAmount);

        var expectedGross =
            option.Lines.Sum(x => x.GrossAmount);

        decimal? expectedTax =
            option.Lines.Any(x => x.TaxAmount.HasValue)
                ? option.Lines.Sum(x => x.TaxAmount ?? 0m)
                : null;

        if (option.TotalNetAmount != expectedNet)
        {
            AddIssue(
                issues,
                "OptionNetAmountMismatch",
                $"Option {option.OptionNumber} has an invalid TotalNetAmount.",
                option.Id);
        }

        if (option.TotalTaxAmount != expectedTax)
        {
            AddIssue(
                issues,
                "OptionTaxAmountMismatch",
                $"Option {option.OptionNumber} has an invalid TotalTaxAmount.",
                option.Id);
        }

        if (option.TotalGrossAmount != expectedGross)
        {
            AddIssue(
                issues,
                "OptionGrossAmountMismatch",
                $"Option {option.OptionNumber} has an invalid TotalGrossAmount.",
                option.Id);
        }
    }

    /// <summary>
    /// Fryser buyer-facing proposal-innehåll, supersede:ar tidigare issue och gör Draft-options Offered.
    /// </summary>
    public async Task<Result<BuyerProposalResponse>> IssueRevisionAsync(
        Guid buyerProposalRevisionId,
        IssueBuyerProposalRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation =
            await _validationService.ValidateAsync(
                request,
                cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                validation.Errors);
        }

        var revision = await _db.Set<BuyerProposalRevision>()
            .Include(x => x.BuyerProposal)
            .Include(x => x.Options)
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalRevisionId,
                cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerProposalResponse>(
                new NotFoundError(
                    nameof(BuyerProposalRevision),
                    buyerProposalRevisionId));
        }

        /*
         * Retry efter lyckad issue returnerar befintligt immutable resultat.
         */
        if (revision.Status == BuyerProposalRevisionStatus.Issued
            && !string.IsNullOrWhiteSpace(revision.SnapshotJson)
            && !string.IsNullOrWhiteSpace(revision.SnapshotHash))
        {
            var computedHash =
                _snapshotService.ComputeHash(
                    revision.SnapshotJson);

            if (!string.Equals(
                    computedHash,
                    revision.SnapshotHash,
                    StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Stored Buyer proposal revision {BuyerProposalRevisionId} snapshot verification failed during idempotent issue retry",
                    revision.Id);

                return Result.Fail<BuyerProposalResponse>(
                    new ConflictError(
                        "Stored BuyerProposalRevision snapshot is invalid."));
            }

            return Result.Ok(
                await ProjectRequiredProposalAsync(
                    revision.BuyerProposalId,
                    cancellationToken));
        }

        var now =
            DateTimeOffset.UtcNow;

        var readiness =
            await GetIssueReadinessAsync(
                revision.Id,
                now,
                cancellationToken);

        if (readiness.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                readiness.Errors);
        }

        if (!readiness.Value.IsReady)
        {
            _logger.LogInformation(
                "Buyer proposal revision {BuyerProposalRevisionId} issue blocked by readiness validation. IssueCount={IssueCount}, IssueCodes={IssueCodes}",
                revision.Id,
                readiness.Value.Issues.Count,
                string.Join(",", readiness.Value.Issues.Select(x => x.Code)));

            return Result.Fail<BuyerProposalResponse>(
                readiness.Value.Issues.Select(
                    x => new ConflictError(
                        $"{x.Code}: {x.Message}")));
        }

        var snapshot =
            await _snapshotService.CreateAsync(
                revision.Id,
                cancellationToken);

        var proposal =
            revision.BuyerProposal;

        _db.Entry(proposal)
            .Property(x => x.RowVersion)
            .OriginalValue =
                request.BuyerProposalRowVersion;

        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue =
                request.BuyerProposalRevisionRowVersion;

        await using var transaction =
            await _db.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var previousIssued =
                await _db.Set<BuyerProposalRevision>()
                    .Include(x => x.Options)
                    .SingleOrDefaultAsync(
                        x => x.BuyerProposalId == proposal.Id
                            && x.Id != revision.Id
                            && x.Status == BuyerProposalRevisionStatus.Issued,
                        cancellationToken);

            if (previousIssued is not null)
            {
                previousIssued.Status =
                    BuyerProposalRevisionStatus.Superseded;

                previousIssued.ActionDateUtc =
                    now;

                foreach (var oldOption in previousIssued.Options)
                {
                    if (oldOption.Status ==
                        BuyerProposalOptionStatus.Offered)
                    {
                        oldOption.Status =
                            BuyerProposalOptionStatus.Expired;

                        oldOption.ActionDateUtc =
                            now;
                    }
                }

                /*
                 * Sparas innan nya revisionen blir Issued så partial unique-indexet
                 * aldrig behöver se två Issued revisions samtidigt.
                 */
                await _db.SaveChangesAsync(
                    cancellationToken);
            }

            revision.Status =
                BuyerProposalRevisionStatus.Issued;

            revision.IssuedAtUtc =
                now;

            revision.IssuedByUserId =
                request.IssuedByUserId;

            revision.SnapshotJson =
                snapshot.Json;

            revision.SnapshotHash =
                snapshot.Hash;

            revision.ActionDateUtc =
                now;

            foreach (var option in revision.Options)
            {
                option.Status =
                    BuyerProposalOptionStatus.Offered;

                option.ActionDateUtc =
                    now;
            }

            proposal.Status =
                BuyerProposalStatus.Issued;

            /*
             * Root-värdet beskriver när proposalet först utfärdades.
             * Exakt issue-tid per version ligger på revisionen.
             */
            proposal.IssuedAtUtc ??=
                now;

            proposal.ClosedAtUtc =
                null;

            proposal.ActionDateUtc =
                now;

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
                "Concurrency conflict while issuing Buyer proposal {BuyerProposalId} revision {BuyerProposalRevisionId}",
                proposal.Id,
                revision.Id);

            if (exception.Entries.Any(
                    x => x.Entity is BuyerProposal))
            {
                return Result.Fail<BuyerProposalResponse>(
                    new ConcurrencyError(
                        nameof(BuyerProposal),
                        proposal.Id));
            }

            return Result.Fail<BuyerProposalResponse>(
                new ConcurrencyError(
                    nameof(BuyerProposalRevision),
                    revision.Id));
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _logger.LogWarning(
                exception,
                "Concurrent Issued-revision conflict while issuing Buyer proposal {BuyerProposalId} revision {BuyerProposalRevisionId}",
                proposal.Id,
                revision.Id);

            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    "Another BuyerProposalRevision became Issued concurrently."));
        }

        _logger.LogInformation(
            "Buyer proposal {BuyerProposalId} revision {BuyerProposalRevisionId} issued by user {IssuedByUserId}. RevisionNumber={RevisionNumber}, OptionCount={OptionCount}",
            proposal.Id,
            revision.Id,
            request.IssuedByUserId,
            revision.RevisionNumber,
            revision.Options.Count);

        return Result.Ok(
            await ProjectRequiredProposalAsync(
                proposal.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kontrollerar att revisionen fortfarande är en ren Draft.
    /// </summary>
    private static Result ValidateRevisionIsWritable(
        BuyerProposalRevision revision)
    {
        if (revision.Status != BuyerProposalRevisionStatus.Draft)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposalRevision with status '{revision.Status}' is immutable."));
        }

        if (revision.IssuedAtUtc.HasValue
            || revision.SnapshotJson is not null
            || revision.SnapshotHash is not null)
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerProposalRevision contains issued snapshot data and is immutable."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Räknar option-totals från redan laddade lines.
    /// </summary>
    private static void RecalculateOptionTotals(
        BuyerProposalOption option)
    {
        option.TotalNetAmount =
            option.Lines.Sum(x => x.NetAmount);

        option.TotalGrossAmount =
            option.Lines.Sum(x => x.GrossAmount);

        option.TotalTaxAmount =
            option.Lines.Any(x => x.TaxAmount.HasValue)
                ? option.Lines.Sum(x => x.TaxAmount ?? 0m)
                : null;
    }

    /// <summary>
    /// Markerar revisionen för aggregate-level concurrency.
    /// </summary>
    private static void TouchRevision(
        BuyerProposalRevision revision)
    {
        revision.ActionDateUtc =
            DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Lägger till ett readiness-problem.
    /// </summary>
    private static void AddIssue(
        ICollection<BuyerProposalReadinessIssueResponse> issues,
        string code,
        string message,
        Guid? entityId = null)
    {
        issues.Add(
            new BuyerProposalReadinessIssueResponse
            {
                Code = code,
                Message = message,
                EntityId = entityId
            });
    }

    /// <summary>
    /// Projekterar ett proposal eller returnerar null.
    /// </summary>
    private Task<BuyerProposalResponse?> ProjectProposalAsync(
        Guid proposalId,
        CancellationToken cancellationToken)
    {
        return _db.Set<BuyerProposal>()
            .AsNoTracking()
            .Where(x => x.Id == proposalId)
            .Select(BuyerProposalProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar en revision eller returnerar null.
    /// </summary>
    private Task<BuyerProposalRevisionResponse?> ProjectRevisionAsync(
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        return _db.Set<BuyerProposalRevision>()
            .AsNoTracking()
            .Where(x => x.Id == revisionId)
            .Select(BuyerProposalProjections.RevisionDetails)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar ett redan sparat proposal.
    /// </summary>
    private async Task<BuyerProposalResponse> ProjectRequiredProposalAsync(
        Guid proposalId,
        CancellationToken cancellationToken)
    {
        return await ProjectProposalAsync(
                   proposalId,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   $"BuyerProposal '{proposalId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Projekterar en redan sparad revision.
    /// </summary>
    private async Task<BuyerProposalRevisionResponse> ProjectRequiredRevisionAsync(
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        return await ProjectRevisionAsync(
                   revisionId,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   $"BuyerProposalRevision '{revisionId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Lägger på stabil sortering innan paginering utförs.
    /// </summary>
    private static IQueryable<BuyerProposal> ApplySorting(
        IQueryable<BuyerProposal> query,
        BuyerProposalSortField sortBy,
        SortDirection sortDirection)
    {
        return (sortBy, sortDirection) switch
        {
            (BuyerProposalSortField.ProposalNumber, SortDirection.Descending) =>
                query.OrderByDescending(x => x.ProposalNumber)
                    .ThenByDescending(x => x.Id),

            (BuyerProposalSortField.Name, SortDirection.Ascending) =>
                query.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id),

            (BuyerProposalSortField.Name, SortDirection.Descending) =>
                query.OrderByDescending(x => x.Name)
                    .ThenByDescending(x => x.Id),

            (BuyerProposalSortField.BuyerName, SortDirection.Ascending) =>
                query.OrderBy(x => x.BuyerBusinessRelationship.Counterparty.Name)
                    .ThenBy(x => x.Id),

            (BuyerProposalSortField.BuyerName, SortDirection.Descending) =>
                query.OrderByDescending(x => x.BuyerBusinessRelationship.Counterparty.Name)
                    .ThenByDescending(x => x.Id),

            (BuyerProposalSortField.IssuedAtUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.IssuedAtUtc)
                    .ThenBy(x => x.Id),

            (BuyerProposalSortField.IssuedAtUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.IssuedAtUtc)
                    .ThenByDescending(x => x.Id),

            (BuyerProposalSortField.ClosedAtUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.ClosedAtUtc)
                    .ThenBy(x => x.Id),

            (BuyerProposalSortField.ClosedAtUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.ClosedAtUtc)
                    .ThenByDescending(x => x.Id),

            (BuyerProposalSortField.CreateDateUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.CreateDateUtc)
                    .ThenBy(x => x.Id),

            (BuyerProposalSortField.CreateDateUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.CreateDateUtc)
                    .ThenByDescending(x => x.Id),

            (BuyerProposalSortField.LatestRevisionNumber, SortDirection.Ascending) =>
                query.OrderBy(x => x.LatestRevisionNumber)
                    .ThenBy(x => x.Id),

            (BuyerProposalSortField.LatestRevisionNumber, SortDirection.Descending) =>
                query.OrderByDescending(x => x.LatestRevisionNumber)
                    .ThenByDescending(x => x.Id),

            _ =>
                query.OrderBy(x => x.ProposalNumber)
                    .ThenBy(x => x.Id)
        };
    }

    /// <summary>
    /// Kontrollerar PostgreSQL unique violation.
    /// </summary>
    private static bool IsUniqueViolation(
        DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };
    }

    /// <summary>
    /// Trimmar text och ersätter whitespace med null.
    /// </summary>
    private static string? TrimToNull(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    /// <summary>
    /// Kontrollerar att eventuell disclosure-revision är godkänd för exakt proposalets buyer.
    /// </summary>
    private async Task<Result> ValidateDisclosureForIssueAsync(
        BuyerProposal proposal,
        BuyerProposalRevision revision,
        CancellationToken cancellationToken)
    {
        if (!revision.DisclosurePackageRevisionId.HasValue)
        {
            return Result.Ok();
        }

        var disclosure = await _db.Set<DisclosurePackageRevision>()
            .AsNoTracking()
            .Where(x => x.Id == revision.DisclosurePackageRevisionId.Value)
            .Select(x => new
            {
                RevisionStatus = x.Status,
                x.ContentHash,
                x.ApprovedAtUtc,
                PackageStatus = x.DisclosurePackage.Status,
                x.DisclosurePackage.AudienceBusinessRelationshipId,
                x.DisclosurePackage.Direction
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (disclosure is null)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(DisclosurePackageRevision),
                    revision.DisclosurePackageRevisionId.Value));
        }

        if (disclosure.AudienceBusinessRelationshipId != proposal.BuyerBusinessRelationshipId)
        {
            return Result.Fail(
                new ConflictError(
                    "DisclosurePackage is intended for another buyer relationship."));
        }

        if (disclosure.Direction != DisclosureDirection.ToBuyer)
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerProposal can only issue a DisclosurePackage with direction ToBuyer."));
        }

        if (disclosure.RevisionStatus is not (
            DisclosurePackageStatus.Approved
            or DisclosurePackageStatus.Sent))
        {
            return Result.Fail(
                new ConflictError(
                    $"DisclosurePackageRevision must be Approved before proposal issue. Current status is '{disclosure.RevisionStatus}'."));
        }

        if (disclosure.PackageStatus is not (
            DisclosurePackageStatus.Approved
            or DisclosurePackageStatus.Sent))
        {
            return Result.Fail(
                new ConflictError(
                    $"DisclosurePackage must be Approved before proposal issue. Current status is '{disclosure.PackageStatus}'."));
        }

        if (!disclosure.ApprovedAtUtc.HasValue)
        {
            return Result.Fail(
                new ConflictError(
                    "Approved DisclosurePackageRevision has no ApprovedAtUtc."));
        }

        if (string.IsNullOrWhiteSpace(disclosure.ContentHash))
        {
            return Result.Fail(
                new ConflictError(
                    "DisclosurePackageRevision has no ContentHash."));
        }

        return Result.Ok();
    }
}