
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Trading.Snapshots;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar buyer acceptance som append-only kommersiell historik ovanpå en immutable Issued proposal-revision.
/// </summary>
public sealed class BuyerProposalAcceptanceService : IBuyerProposalAcceptanceService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly IBuyerProposalRevisionSnapshotService _snapshotService;

    /// <summary>
    /// Skapar acceptance-servicen med databas-, validation- och snapshot-beroenden.
    /// </summary>
    public BuyerProposalAcceptanceService(
        Arb_IntDbContext db,
        IValidationService validationService,
        IBuyerProposalRevisionSnapshotService snapshotService)
    {
        _db = db;
        _validationService = validationService;
        _snapshotService = snapshotService;
    }

    /// <summary>
    /// Hämtar en acceptance med dess accepterade quantities.
    /// </summary>
    public async Task<Result<BuyerProposalAcceptanceResponse>> GetByIdAsync(
        Guid buyerProposalAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        var response = await _db.Set<BuyerProposalAcceptance>()
            .AsNoTracking()
            .Where(x => x.Id == buyerProposalAcceptanceId)
            .Select(BuyerProposalAcceptanceProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);

        return response is null
            ? Result.Fail<BuyerProposalAcceptanceResponse>(
                new NotFoundError(
                    nameof(BuyerProposalAcceptance),
                    buyerProposalAcceptanceId))
            : Result.Ok(response);
    }

    /// <summary>
    /// Hämtar acceptance-historiken för en proposal-revision med senaste buyer acceptance först.
    /// </summary>
    public async Task<Result<IReadOnlyList<BuyerProposalAcceptanceResponse>>> GetByRevisionAsync(
        Guid buyerProposalRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revisionExists = await _db.Set<BuyerProposalRevision>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == buyerProposalRevisionId,
                cancellationToken);

        if (!revisionExists)
        {
            return Result.Fail<IReadOnlyList<BuyerProposalAcceptanceResponse>>(
                new NotFoundError(
                    nameof(BuyerProposalRevision),
                    buyerProposalRevisionId));
        }

        var responses = await _db.Set<BuyerProposalAcceptance>()
            .AsNoTracking()
            .Where(x => x.BuyerProposalRevisionId == buyerProposalRevisionId)
            .OrderByDescending(x => x.AcceptedAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(BuyerProposalAcceptanceProjections.Details)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<BuyerProposalAcceptanceResponse>>(
            responses);
    }

    /// <summary>
    /// Registrerar buyer acceptance och härleder partial-state, units och QuantityChanged från det utfärdade proposalet.
    /// </summary>
    public async Task<Result<BuyerProposalAcceptanceResponse>> RecordAsync(
        Guid buyerProposalOptionId,
        RecordBuyerProposalAcceptanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                validation.Errors);
        }

        var option = await LoadOptionForAcceptanceAsync(
            buyerProposalOptionId,
            cancellationToken);

        if (option is null)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                new NotFoundError(
                    nameof(BuyerProposalOption),
                    buyerProposalOptionId));
        }

        var revision = option.BuyerProposalRevision;
        var proposal = revision.BuyerProposal;
        var now = DateTimeOffset.UtcNow;

        var baselineResult = ValidateRecordBaseline(
            proposal,
            revision,
            option,
            request.AcceptedAtUtc,
            now);

        if (baselineResult.IsFailed)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                baselineResult.Errors);
        }

        var snapshotResult = ValidateRevisionSnapshot(
            revision);

        if (snapshotResult.IsFailed)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                snapshotResult.Errors);
        }

        /*
         * Bara en acceptance får ligga och vänta på approval i samma revision.
         */
        if (revision.Acceptances.Any(
                x => x.Status == BuyerProposalAcceptanceStatus.PendingInternalApproval))
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                new ConflictError(
                    "BuyerProposalRevision already has an acceptance pending internal approval."));
        }

        /*
         * Efter en tidigare approved partial acceptance måste resterande
         * acceptances fortsätta på samma option. Proposal-options är alternativ.
         */
        var previouslyApprovedOptionIds = revision.Acceptances
            .Where(x => x.Status == BuyerProposalAcceptanceStatus.Approved)
            .Select(x => x.BuyerProposalOptionId)
            .Distinct()
            .ToArray();

        if (previouslyApprovedOptionIds.Length > 1)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                new ConflictError(
                    "BuyerProposalRevision contains Approved acceptances for multiple options."));
        }

        if (previouslyApprovedOptionIds.Length == 1
            && previouslyApprovedOptionIds[0] != option.Id)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                new ConflictError(
                    "A partial acceptance already exists for another BuyerProposalOption."));
        }

        var requestedByLineId = request.Lines
            .ToDictionary(
                x => x.BuyerProposalLineId);

        foreach (var requestedLine in request.Lines)
        {
            if (option.Lines.All(
                    x => x.Id != requestedLine.BuyerProposalLineId))
            {
                return Result.Fail<BuyerProposalAcceptanceResponse>(
                    new ConflictError(
                        $"BuyerProposalLine '{requestedLine.BuyerProposalLineId}' does not belong to the selected BuyerProposalOption."));
            }
        }

        var previouslyApprovedQuantities = GetApprovedQuantitiesByLine(
            revision,
            option.Id);

        foreach (var requestedLine in request.Lines)
        {
            var proposalLine = option.Lines.Single(
                x => x.Id == requestedLine.BuyerProposalLineId);

            previouslyApprovedQuantities.TryGetValue(
                proposalLine.Id,
                out var alreadyApprovedQuantity);

            var remainingQuantity =
                proposalLine.Quantity - alreadyApprovedQuantity;

            if (remainingQuantity <= 0m)
            {
                return Result.Fail<BuyerProposalAcceptanceResponse>(
                    new ConflictError(
                        $"BuyerProposalLine '{proposalLine.Id}' has already been fully accepted."));
            }

            if (requestedLine.AcceptedQuantity > remainingQuantity)
            {
                return Result.Fail<BuyerProposalAcceptanceResponse>(
                    new ConflictError(
                        $"AcceptedQuantity for BuyerProposalLine '{proposalLine.Id}' exceeds the remaining offered quantity."));
            }
        }

        /*
         * IsPartialAcceptance beskriver just denna buyer acceptance relativt
         * hela optionen, inte cumulative state efter tidigare acceptances.
         */
        var isPartialAcceptance =
            option.Lines.Count != request.Lines.Count
            || option.Lines.Any(
                proposalLine =>
                    !requestedByLineId.TryGetValue(
                        proposalLine.Id,
                        out var acceptedLine)
                    || acceptedLine.AcceptedQuantity != proposalLine.Quantity);

        var acceptance = new BuyerProposalAcceptance
        {
            Id = Guid.CreateVersion7(),
            BuyerProposalRevisionId = revision.Id,
            BuyerProposalOptionId = option.Id,
            Status = BuyerProposalAcceptanceStatus.PendingInternalApproval,
            BuyerAcceptanceReference = TrimToNull(request.BuyerAcceptanceReference),
            AcceptedAtUtc = request.AcceptedAtUtc,
            RecordedByUserId = request.RecordedByUserId,
            IsPartialAcceptance = isPartialAcceptance,
            InternallyApprovedAtUtc = null,
            InternallyApprovedByUserId = null,
            InternalApprovalNotes = null
        };

        foreach (var requestedLine in request.Lines)
        {
            var proposalLine = option.Lines.Single(
                x => x.Id == requestedLine.BuyerProposalLineId);

            acceptance.Lines.Add(
                new BuyerProposalAcceptanceLine
                {
                    Id = Guid.CreateVersion7(),
                    BuyerProposalAcceptanceId = acceptance.Id,
                    BuyerProposalOptionId = option.Id,
                    BuyerProposalLineId = proposalLine.Id,
                    AcceptedQuantity = requestedLine.AcceptedQuantity,

                    /*
                     * Unit hämtas från det utfärdade proposalet.
                     * Klienten får inte förändra quantity-semantiken vid acceptance.
                     */
                    QuantityUnitId = proposalLine.QuantityUnitId,

                    /*
                     * Flaggan beskriver om just denna acceptance skiljer sig
                     * från hela quantityn på proposal-raden.
                     */
                    QuantityChanged = requestedLine.AcceptedQuantity != proposalLine.Quantity,
                    BuyerNotes = TrimToNull(requestedLine.BuyerNotes),
                    InternalNotes = TrimToNull(requestedLine.InternalNotes)
                });
        }

        _db.Entry(proposal)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerProposalRowVersion;

        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerProposalRevisionRowVersion;

        _db.Entry(option)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerProposalOptionRowVersion;

        /*
         * Alla tre nivåerna touchas så acceptance registreras mot exakt det
         * proposal/revision/option-state som användaren faktiskt såg.
         */
        proposal.ActionDateUtc = now;
        revision.ActionDateUtc = now;
        option.ActionDateUtc = now;

        _db.Set<BuyerProposalAcceptance>()
            .Add(acceptance);

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                CreateConcurrencyError(
                    exception,
                    proposal,
                    revision,
                    option));
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                new ConflictError(
                    "Another BuyerProposalAcceptance is already pending internal approval."));
        }

        return Result.Ok(
            await ProjectRequiredAcceptanceAsync(
                acceptance.Id,
                cancellationToken));
    }

    /// <summary>
    /// Godkänner en pending acceptance och härleder cumulative acceptance-state för proposal, revision och option.
    /// </summary>
    public async Task<Result<BuyerProposalResponse>> ApproveAsync(
        Guid buyerProposalAcceptanceId,
        ApproveBuyerProposalAcceptanceRequest request,
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

        var acceptance = await LoadAcceptanceForApprovalAsync(
            buyerProposalAcceptanceId,
            cancellationToken);

        if (acceptance is null)
        {
            return Result.Fail<BuyerProposalResponse>(
                new NotFoundError(
                    nameof(BuyerProposalAcceptance),
                    buyerProposalAcceptanceId));
        }

        if (acceptance.Status != BuyerProposalAcceptanceStatus.PendingInternalApproval)
        {
            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    $"Only PendingInternalApproval acceptance can be approved. Current status is '{acceptance.Status}'."));
        }

        var revision = acceptance.BuyerProposalRevision;
        var proposal = revision.BuyerProposal;
        var option = acceptance.BuyerProposalOption;

        var baselineResult = ValidateApprovalBaseline(
            proposal,
            revision,
            option,
            acceptance);

        if (baselineResult.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                baselineResult.Errors);
        }

        var snapshotResult = ValidateRevisionSnapshot(
            revision);

        if (snapshotResult.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                snapshotResult.Errors);
        }

        /*
         * Ett nyare internt Draft-proposal får inte ligga kvar när en äldre
         * externally-issued revision görs kontraktuellt accepterad.
         *
         * Draften hanteras explicit i stället för att automatiskt raderas här.
         */
        var newerDraftExists = await _db.Set<BuyerProposalRevision>()
            .AsNoTracking()
            .AnyAsync(
                x => x.BuyerProposalId == proposal.Id
                    && x.RevisionNumber > revision.RevisionNumber
                    && x.Status == BuyerProposalRevisionStatus.Draft,
                cancellationToken);

        if (newerDraftExists)
        {
            return Result.Fail<BuyerProposalResponse>(
                new ConflictError(
                    "A newer Draft BuyerProposalRevision exists and must be resolved before this acceptance can be approved."));
        }

        var cumulativeResult = CalculateCumulativeAcceptance(
            revision,
            option,
            acceptance);

        if (cumulativeResult.IsFailed)
        {
            return Result.Fail<BuyerProposalResponse>(
                cumulativeResult.Errors);
        }

        _db.Entry(acceptance)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerProposalAcceptanceRowVersion;

        _db.Entry(proposal)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerProposalRowVersion;

        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerProposalRevisionRowVersion;

        _db.Entry(option)
            .Property(x => x.RowVersion)
            .OriginalValue = request.BuyerProposalOptionRowVersion;

        var now = DateTimeOffset.UtcNow;

        acceptance.Status = BuyerProposalAcceptanceStatus.Approved;
        acceptance.InternallyApprovedAtUtc = now;
        acceptance.InternallyApprovedByUserId = request.InternallyApprovedByUserId;
        acceptance.InternalApprovalNotes = TrimToNull(request.InternalApprovalNotes);
        acceptance.ActionDateUtc = now;

        if (cumulativeResult.Value.IsFullyAccepted)
        {
            /*
             * Hela det valda optionet är nu accepterat cumulativt över
             * en eller flera Approved acceptances.
             */
            option.Status = BuyerProposalOptionStatus.Accepted;
            option.ActionDateUtc = now;

            revision.Status = BuyerProposalRevisionStatus.Accepted;
            revision.ActionDateUtc = now;

            proposal.Status = BuyerProposalStatus.Accepted;
            proposal.ClosedAtUtc = now;
            proposal.ActionDateUtc = now;
        }
        else
        {
            /*
             * Det finns fortfarande quantity kvar på det valda optionet.
             * Optionen och revisionen fortsätter därför vara externally open.
             */
            option.Status = BuyerProposalOptionStatus.Offered;
            option.ActionDateUtc = now;

            revision.Status = BuyerProposalRevisionStatus.Issued;
            revision.ActionDateUtc = now;

            proposal.Status = BuyerProposalStatus.PartiallyAccepted;
            proposal.ClosedAtUtc = null;
            proposal.ActionDateUtc = now;
        }

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<BuyerProposalResponse>(
                CreateApprovalConcurrencyError(
                    exception,
                    acceptance,
                    proposal,
                    revision,
                    option));
        }

        return Result.Ok(
            await ProjectRequiredProposalAsync(
                proposal.Id,
                cancellationToken));
    }

    /// <summary>
    /// Avslår en pending acceptance utan att förändra tidigare Approved quantities eller proposal-status.
    /// </summary>
    public async Task<Result<BuyerProposalAcceptanceResponse>> RejectAsync(
        Guid buyerProposalAcceptanceId,
        RejectBuyerProposalAcceptanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                validation.Errors);
        }

        var acceptance = await _db.Set<BuyerProposalAcceptance>()
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalAcceptanceId,
                cancellationToken);

        if (acceptance is null)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                new NotFoundError(
                    nameof(BuyerProposalAcceptance),
                    buyerProposalAcceptanceId));
        }

        if (acceptance.Status != BuyerProposalAcceptanceStatus.PendingInternalApproval)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                new ConflictError(
                    $"Only PendingInternalApproval acceptance can be rejected. Current status is '{acceptance.Status}'."));
        }

        _db.Entry(acceptance)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        acceptance.Status = BuyerProposalAcceptanceStatus.Rejected;
        acceptance.InternallyApprovedAtUtc = null;
        acceptance.InternallyApprovedByUserId = null;
        acceptance.InternalApprovalNotes = TrimToNull(request.InternalApprovalNotes);
        acceptance.ActionDateUtc = DateTimeOffset.UtcNow;

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<BuyerProposalAcceptanceResponse>(
                new ConcurrencyError(
                    nameof(BuyerProposalAcceptance),
                    acceptance.Id));
        }

        return Result.Ok(
            await ProjectRequiredAcceptanceAsync(
                acceptance.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kontrollerar att buyer acceptance avser ett fortfarande accepterbart utfärdat erbjudande.
    /// </summary>
    private static Result ValidateRecordBaseline(
        BuyerProposal proposal,
        BuyerProposalRevision revision,
        BuyerProposalOption option,
        DateTimeOffset acceptedAtUtc,
        DateTimeOffset recordedAtUtc)
    {
        if (proposal.Status is not (
            BuyerProposalStatus.Issued
            or BuyerProposalStatus.PartiallyAccepted))
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposal with status '{proposal.Status}' cannot receive a new buyer acceptance."));
        }

        if (revision.Status != BuyerProposalRevisionStatus.Issued)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposalRevision must be Issued. Current status is '{revision.Status}'."));
        }

        if (option.Status != BuyerProposalOptionStatus.Offered)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposalOption must be Offered. Current status is '{option.Status}'."));
        }

        if (!revision.IssuedAtUtc.HasValue)
        {
            return Result.Fail(
                new ConflictError(
                    "Issued BuyerProposalRevision has no IssuedAtUtc."));
        }

        if (acceptedAtUtc > recordedAtUtc)
        {
            return Result.Fail(
                new ConflictError(
                    "AcceptedAtUtc cannot be in the future."));
        }

        if (acceptedAtUtc < revision.IssuedAtUtc.Value)
        {
            return Result.Fail(
                new ConflictError(
                    "Buyer acceptance cannot occur before the proposal revision was issued."));
        }

        if (revision.ValidFromUtc.HasValue
            && acceptedAtUtc < revision.ValidFromUtc.Value)
        {
            return Result.Fail(
                new ConflictError(
                    $"Buyer acceptance occurred before ValidFromUtc '{revision.ValidFromUtc.Value:O}'."));
        }

        if (revision.ValidToUtc.HasValue
            && acceptedAtUtc > revision.ValidToUtc.Value)
        {
            return Result.Fail(
                new ConflictError(
                    $"Buyer acceptance occurred after ValidToUtc '{revision.ValidToUtc.Value:O}'."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Verifierar att acceptance fortfarande refererar exakt det immutable proposal som utfärdades.
    /// </summary>
    private Result ValidateRevisionSnapshot(
        BuyerProposalRevision revision)
    {
        if (string.IsNullOrWhiteSpace(revision.SnapshotJson)
            || string.IsNullOrWhiteSpace(revision.SnapshotHash))
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerProposalRevision does not contain an immutable issued snapshot."));
        }

        var computedHash = _snapshotService.ComputeHash(
            revision.SnapshotJson);

        if (!string.Equals(
                computedHash,
                revision.SnapshotHash,
                StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerProposalRevision snapshot hash is invalid."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Summerar tidigare internt godkända quantities per proposal-line för en option.
    /// </summary>
    private static Dictionary<Guid, decimal> GetApprovedQuantitiesByLine(
        BuyerProposalRevision revision,
        Guid buyerProposalOptionId)
    {
        return revision.Acceptances
            .Where(x =>
                x.BuyerProposalOptionId == buyerProposalOptionId
                && x.Status == BuyerProposalAcceptanceStatus.Approved)
            .SelectMany(x => x.Lines)
            .GroupBy(x => x.BuyerProposalLineId)
            .ToDictionary(
                group => group.Key,
                group => group.Sum(x => x.AcceptedQuantity));
    }

    /// <summary>
    /// Kontrollerar att en pending acceptance fortfarande kan godkännas mot samma utfärdade option.
    /// </summary>
    private static Result ValidateApprovalBaseline(
        BuyerProposal proposal,
        BuyerProposalRevision revision,
        BuyerProposalOption option,
        BuyerProposalAcceptance acceptance)
    {
        if (proposal.Status is not (
            BuyerProposalStatus.Issued
            or BuyerProposalStatus.PartiallyAccepted))
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposal with status '{proposal.Status}' cannot approve another acceptance."));
        }

        if (revision.Status != BuyerProposalRevisionStatus.Issued)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposalRevision must still be Issued. Current status is '{revision.Status}'."));
        }

        if (option.Status != BuyerProposalOptionStatus.Offered)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposalOption must still be Offered. Current status is '{option.Status}'."));
        }

        if (acceptance.BuyerProposalRevisionId != revision.Id
            || acceptance.BuyerProposalOptionId != option.Id)
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerProposalAcceptance provenance does not match the loaded proposal revision and option."));
        }

        if (!revision.IssuedAtUtc.HasValue)
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerProposalRevision has no IssuedAtUtc."));
        }

        /*
         * Approval får ske efter ValidToUtc om buyer faktiskt accepterade
         * inom det giltiga fönstret.
         */
        if (acceptance.AcceptedAtUtc < revision.IssuedAtUtc.Value)
        {
            return Result.Fail(
                new ConflictError(
                    "Buyer acceptance occurred before the proposal revision was issued."));
        }

        if (revision.ValidFromUtc.HasValue
            && acceptance.AcceptedAtUtc < revision.ValidFromUtc.Value)
        {
            return Result.Fail(
                new ConflictError(
                    "Buyer acceptance occurred before the proposal became valid."));
        }

        if (revision.ValidToUtc.HasValue
            && acceptance.AcceptedAtUtc > revision.ValidToUtc.Value)
        {
            return Result.Fail(
                new ConflictError(
                    "Buyer acceptance occurred after the proposal expired."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Beskriver cumulative acceptance-state för ett proposal-option efter aktuell approval.
    /// </summary>
    private sealed class CumulativeAcceptanceResult
    {
        /// <summary>
        /// Anger om samtliga proposal-lines nu är accepterade till full quantity.
        /// </summary>
        public bool IsFullyAccepted { get; init; }

        /// <summary>
        /// Innehåller cumulative accepted quantity per proposal-line.
        /// </summary>
        public required IReadOnlyDictionary<Guid, decimal> AcceptedQuantityByLineId { get; init; }
    }

    /// <summary>
    /// Beräknar cumulative Approved quantity inklusive den pending acceptance som nu ska godkännas.
    /// </summary>
    private static Result<CumulativeAcceptanceResult> CalculateCumulativeAcceptance(
        BuyerProposalRevision revision,
        BuyerProposalOption option,
        BuyerProposalAcceptance currentAcceptance)
    {
        var acceptedQuantityByLineId = revision.Acceptances
            .Where(x =>
                x.BuyerProposalOptionId == option.Id
                && x.Status == BuyerProposalAcceptanceStatus.Approved
                && x.Id != currentAcceptance.Id)
            .SelectMany(x => x.Lines)
            .GroupBy(x => x.BuyerProposalLineId)
            .ToDictionary(
                group => group.Key,
                group => group.Sum(x => x.AcceptedQuantity));

        foreach (var acceptanceLine in currentAcceptance.Lines)
        {
            if (!acceptedQuantityByLineId.TryAdd(
                    acceptanceLine.BuyerProposalLineId,
                    acceptanceLine.AcceptedQuantity))
            {
                acceptedQuantityByLineId[acceptanceLine.BuyerProposalLineId] +=
                    acceptanceLine.AcceptedQuantity;
            }
        }

        foreach (var proposalLine in option.Lines)
        {
            acceptedQuantityByLineId.TryGetValue(
                proposalLine.Id,
                out var acceptedQuantity);

            if (acceptedQuantity > proposalLine.Quantity)
            {
                return Result.Fail<CumulativeAcceptanceResult>(
                    new ConflictError(
                        $"Cumulative accepted quantity exceeds offered quantity for BuyerProposalLine '{proposalLine.Id}'."));
            }
        }

        /*
         * Full acceptance kräver exakt full quantity på samtliga lines.
         * Saknad line motsvarar zero accepted quantity.
         */
        var isFullyAccepted = option.Lines.All(
            proposalLine =>
            {
                acceptedQuantityByLineId.TryGetValue(
                    proposalLine.Id,
                    out var acceptedQuantity);

                return acceptedQuantity == proposalLine.Quantity;
            });

        return Result.Ok(
            new CumulativeAcceptanceResult
            {
                IsFullyAccepted = isFullyAccepted,
                AcceptedQuantityByLineId = acceptedQuantityByLineId
            });
    }

    /// <summary>
    /// Läser option, proposal-lines, revision, root och acceptance-historik för registrering av buyer acceptance.
    /// </summary>
    private Task<BuyerProposalOption?> LoadOptionForAcceptanceAsync(
        Guid buyerProposalOptionId,
        CancellationToken cancellationToken)
    {
        return _db.Set<BuyerProposalOption>()
            .Include(x => x.Lines)
            .Include(x => x.BuyerProposalRevision)
                .ThenInclude(x => x.BuyerProposal)
            .Include(x => x.BuyerProposalRevision)
                .ThenInclude(x => x.Acceptances)
                    .ThenInclude(x => x.Lines)
            .AsSplitQuery()
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalOptionId,
                cancellationToken);
    }

    /// <summary>
    /// Läser acceptance tillsammans med proposal, revision, option, proposal-lines och tidigare acceptances.
    /// </summary>
    private Task<BuyerProposalAcceptance?> LoadAcceptanceForApprovalAsync(
        Guid buyerProposalAcceptanceId,
        CancellationToken cancellationToken)
    {
        return _db.Set<BuyerProposalAcceptance>()
            .Include(x => x.Lines)
            .Include(x => x.BuyerProposalOption)
                .ThenInclude(x => x.Lines)
            .Include(x => x.BuyerProposalRevision)
                .ThenInclude(x => x.BuyerProposal)
            .Include(x => x.BuyerProposalRevision)
                .ThenInclude(x => x.Acceptances)
                    .ThenInclude(x => x.Lines)
            .AsSplitQuery()
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalAcceptanceId,
                cancellationToken);
    }

    /// <summary>
    /// Översätter concurrency-fel vid acceptance-registrering till rätt aggregate-nivå.
    /// </summary>
    private static ConcurrencyError CreateConcurrencyError(
        DbUpdateConcurrencyException exception,
        BuyerProposal proposal,
        BuyerProposalRevision revision,
        BuyerProposalOption option)
    {
        foreach (var entry in exception.Entries)
        {
            switch (entry.Entity)
            {
                case BuyerProposal buyerProposal:
                    return new ConcurrencyError(
                        nameof(BuyerProposal),
                        buyerProposal.Id);

                case BuyerProposalRevision proposalRevision:
                    return new ConcurrencyError(
                        nameof(BuyerProposalRevision),
                        proposalRevision.Id);

                case BuyerProposalOption proposalOption:
                    return new ConcurrencyError(
                        nameof(BuyerProposalOption),
                        proposalOption.Id);
            }
        }

        return new ConcurrencyError(
            nameof(BuyerProposal),
            proposal.Id);
    }

    /// <summary>
    /// Översätter concurrency-fel vid approval till acceptance eller rätt proposal-nivå.
    /// </summary>
    private static ConcurrencyError CreateApprovalConcurrencyError(
        DbUpdateConcurrencyException exception,
        BuyerProposalAcceptance acceptance,
        BuyerProposal proposal,
        BuyerProposalRevision revision,
        BuyerProposalOption option)
    {
        foreach (var entry in exception.Entries)
        {
            switch (entry.Entity)
            {
                case BuyerProposalAcceptance buyerAcceptance:
                    return new ConcurrencyError(
                        nameof(BuyerProposalAcceptance),
                        buyerAcceptance.Id);

                case BuyerProposal buyerProposal:
                    return new ConcurrencyError(
                        nameof(BuyerProposal),
                        buyerProposal.Id);

                case BuyerProposalRevision proposalRevision:
                    return new ConcurrencyError(
                        nameof(BuyerProposalRevision),
                        proposalRevision.Id);

                case BuyerProposalOption proposalOption:
                    return new ConcurrencyError(
                        nameof(BuyerProposalOption),
                        proposalOption.Id);
            }
        }

        return new ConcurrencyError(
            nameof(BuyerProposalAcceptance),
            acceptance.Id);
    }

    /// <summary>
    /// Projekterar en redan sparad acceptance.
    /// </summary>
    private async Task<BuyerProposalAcceptanceResponse> ProjectRequiredAcceptanceAsync(
        Guid buyerProposalAcceptanceId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerProposalAcceptance>()
            .AsNoTracking()
            .Where(x => x.Id == buyerProposalAcceptanceId)
            .Select(BuyerProposalAcceptanceProjections.Details)
            .SingleAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar proposalet efter en godkänd acceptance.
    /// </summary>
    private async Task<BuyerProposalResponse> ProjectRequiredProposalAsync(
        Guid buyerProposalId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerProposal>()
            .AsNoTracking()
            .Where(x => x.Id == buyerProposalId)
            .Select(BuyerProposalProjections.Details)
            .SingleAsync(cancellationToken);
    }

    /// <summary>
    /// Kontrollerar om PostgreSQL rapporterade en unique violation.
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
}