using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Trading.Snapshots;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar business-lifecycle ovanpå immutable supplier-quote-revisions.
/// </summary>
public sealed class SupplierQuoteLifecycleService : ISupplierQuoteLifecycleService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ISupplierQuoteRevisionSnapshotService _snapshotService;

    /// <summary>
    /// Skapar lifecycle-servicen med databas-, validation- och snapshot-beroenden.
    /// </summary>
    public SupplierQuoteLifecycleService(
        Arb_IntDbContext db,
        IValidationService validationService,
        ISupplierQuoteRevisionSnapshotService snapshotService)
    {
        _db = db;
        _validationService = validationService;
        _snapshotService = snapshotService;
    }

    /// <summary>
    /// Kontrollerar att exakt den angivna revisionen är aktuell, immutable och fortfarande kommersiellt giltig.
    /// </summary>
    public async Task<Result<SupplierQuoteReviewReadinessResponse>> GetReviewReadinessAsync(
        Guid supplierQuoteId,
        Guid supplierQuoteRevisionId,
        DateTimeOffset evaluatedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var quote = await _db.Set<SupplierQuote>()
            .AsNoTracking()
            .Include(x => x.Revisions)
            .SingleOrDefaultAsync(
                x => x.Id == supplierQuoteId,
                cancellationToken);

        if (quote is null)
        {
            return Result.Fail<SupplierQuoteReviewReadinessResponse>(
                new NotFoundError(nameof(SupplierQuote), supplierQuoteId));
        }

        var revision = quote.Revisions
            .SingleOrDefault(x => x.Id == supplierQuoteRevisionId);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteReviewReadinessResponse>(
                new NotFoundError(nameof(SupplierQuoteRevision), supplierQuoteRevisionId));
        }

        var issues = new List<SupplierQuoteReadinessIssueResponse>();

        if (quote.Status != SupplierQuoteStatus.Received)
        {
            AddIssue(
                issues,
                "QuoteNotReceived",
                $"SupplierQuote has status '{quote.Status}'.",
                quote.Id);
        }

        if (revision.Status != SupplierQuoteRevisionStatus.Received)
        {
            AddIssue(
                issues,
                "RevisionNotReceived",
                $"SupplierQuoteRevision has status '{revision.Status}'.",
                revision.Id);
        }

        if (revision.RevisionNumber != quote.LatestRevisionNumber)
        {
            AddIssue(
                issues,
                "RevisionNotLatest",
                "Only the latest SupplierQuoteRevision can enter review.",
                revision.Id);
        }

        if (string.IsNullOrWhiteSpace(revision.SnapshotJson)
            || string.IsNullOrWhiteSpace(revision.SnapshotHash))
        {
            AddIssue(
                issues,
                "RevisionSnapshotMissing",
                "SupplierQuoteRevision must contain an immutable snapshot before review.",
                revision.Id);
        }
        else
        {
            var computedHash = _snapshotService.ComputeHash(
                revision.SnapshotJson);

            if (!string.Equals(
                    computedHash,
                    revision.SnapshotHash,
                    StringComparison.OrdinalIgnoreCase))
            {
                AddIssue(
                    issues,
                    "RevisionSnapshotInvalid",
                    "SupplierQuoteRevision snapshot hash does not match SnapshotJson.",
                    revision.Id);
            }
        }

        if (!revision.ReceivedAtUtc.HasValue)
        {
            AddIssue(
                issues,
                "RevisionNotReceivedAt",
                "SupplierQuoteRevision has no ReceivedAtUtc.",
                revision.Id);
        }

        if (IsExpired(
                revision,
                evaluatedAtUtc))
        {
            AddIssue(
                issues,
                "RevisionExpired",
                $"SupplierQuoteRevision expired at '{revision.ValidToUtc!.Value:O}'.",
                revision.Id);
        }

        return Result.Ok(
            new SupplierQuoteReviewReadinessResponse
            {
                SupplierQuoteId = quote.Id,
                SupplierQuoteRevisionId = revision.Id,
                IsReady = issues.Count == 0,
                Issues = issues
            });
    }

    /// <summary>
    /// Flyttar en Received quote till UnderReview medan revisionen ligger kvar som immutable Received.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> BeginReviewAsync(
        Guid supplierQuoteId,
        BeginSupplierQuoteReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                validation.Errors);
        }

        var quote = await LoadQuoteForWriteAsync(
            supplierQuoteId,
            request.SupplierQuoteRevisionId,
            cancellationToken);

        if (quote is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(
                    nameof(SupplierQuote),
                    supplierQuoteId));
        }

        var revision = quote.Revisions
            .SingleOrDefault(
                x => x.Id == request.SupplierQuoteRevisionId);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(
                    nameof(SupplierQuoteRevision),
                    request.SupplierQuoteRevisionId));
        }

        var now = DateTimeOffset.UtcNow;

        var readiness = await GetReviewReadinessAsync(
            quote.Id,
            revision.Id,
            now,
            cancellationToken);

        if (readiness.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                readiness.Errors);
        }

        if (!readiness.Value.IsReady)
        {
            return Result.Fail<SupplierQuoteResponse>(
                readiness.Value.Issues.Select(
                    x => new ConflictError(
                        $"{x.Code}: {x.Message}")));
        }

        _db.Entry(quote)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRowVersion;

        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRevisionRowVersion;

        /*
         * Revisionen ändras medvetet inte till någon review-status.
         * SupplierQuoteRevisionStatus saknar UnderReview.
         */
        quote.Status = SupplierQuoteStatus.UnderReview;
        quote.ActionDateUtc = now;

        /*
         * Revisionen touchas för att concurrency-token från review-baselinen
         * också ska delta i operationen utan att ändra dess status.
         */
        revision.ActionDateUtc = now;

        var saveResult = await SaveWithConcurrencyAsync(
            quote,
            revision,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                quote.Id,
                cancellationToken));
    }

    /// <summary>
    /// Accepterar den reviewed revisionen internt utan att ändra dess snapshot.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> AcceptInternallyAsync(
        Guid supplierQuoteId,
        AcceptSupplierQuoteInternallyRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                validation.Errors);
        }

        var quote = await LoadQuoteForWriteAsync(
            supplierQuoteId,
            request.SupplierQuoteRevisionId,
            cancellationToken);

        if (quote is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(
                    nameof(SupplierQuote),
                    supplierQuoteId));
        }

        var revision = quote.Revisions
            .SingleOrDefault(
                x => x.Id == request.SupplierQuoteRevisionId);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(
                    nameof(SupplierQuoteRevision),
                    request.SupplierQuoteRevisionId));
        }

        var baselineResult = ValidateReviewDecisionBaseline(
            quote,
            revision,
            DateTimeOffset.UtcNow);

        if (baselineResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                baselineResult.Errors);
        }

        _db.Entry(quote)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRowVersion;

        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRevisionRowVersion;

        var now = DateTimeOffset.UtcNow;

        quote.Status = SupplierQuoteStatus.AcceptedInternally;
        quote.ActionDateUtc = now;

        revision.Status = SupplierQuoteRevisionStatus.AcceptedInternally;
        revision.ActionDateUtc = now;

        var saveResult = await SaveWithConcurrencyAsync(
            quote,
            revision,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                quote.Id,
                cancellationToken));
    }

    /// <summary>
    /// Avslår den aktuella reviewed revisionen och behåller dess immutable snapshot som historik.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> RejectAsync(
        Guid supplierQuoteId,
        RejectSupplierQuoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                validation.Errors);
        }

        var quote = await LoadQuoteForWriteAsync(
            supplierQuoteId,
            request.SupplierQuoteRevisionId,
            cancellationToken);

        if (quote is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(
                    nameof(SupplierQuote),
                    supplierQuoteId));
        }

        var revision = quote.Revisions
            .SingleOrDefault(
                x => x.Id == request.SupplierQuoteRevisionId);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(
                    nameof(SupplierQuoteRevision),
                    request.SupplierQuoteRevisionId));
        }

        var baselineResult = ValidateReviewDecisionBaseline(
            quote,
            revision,
            DateTimeOffset.UtcNow);

        if (baselineResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                baselineResult.Errors);
        }

        _db.Entry(quote)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRowVersion;

        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRevisionRowVersion;

        var now = DateTimeOffset.UtcNow;

        quote.Status = SupplierQuoteStatus.Rejected;
        quote.ActionDateUtc = now;

        revision.Status = SupplierQuoteRevisionStatus.Rejected;
        revision.ActionDateUtc = now;

        var saveResult = await SaveWithConcurrencyAsync(
            quote,
            revision,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                quote.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kontrollerar att en review fortfarande avser rätt immutable och giltiga revision.
    /// </summary>
    private Result ValidateReviewDecisionBaseline(
        SupplierQuote quote,
        SupplierQuoteRevision revision,
        DateTimeOffset evaluatedAtUtc)
    {
        if (quote.Status != SupplierQuoteStatus.UnderReview)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplierQuote must be UnderReview. Current status is '{quote.Status}'."));
        }

        if (revision.Status != SupplierQuoteRevisionStatus.Received)
        {
            return Result.Fail(
                new ConflictError(
                    $"Reviewed SupplierQuoteRevision must still be Received. Current status is '{revision.Status}'."));
        }

        if (revision.RevisionNumber != quote.LatestRevisionNumber)
        {
            return Result.Fail(
                new ConflictError(
                    "Reviewed SupplierQuoteRevision is no longer the latest revision."));
        }

        if (string.IsNullOrWhiteSpace(revision.SnapshotJson)
            || string.IsNullOrWhiteSpace(revision.SnapshotHash))
        {
            return Result.Fail(
                new ConflictError(
                    "Reviewed SupplierQuoteRevision does not contain an immutable snapshot."));
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
                    "SupplierQuoteRevision snapshot hash is invalid."));
        }

        if (IsExpired(
                revision,
                evaluatedAtUtc))
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplierQuoteRevision expired at '{revision.ValidToUtc!.Value:O}'."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Markerar hela quoten som Withdrawn utan att skriva om den mottagna eller accepterade revisionens historik.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> WithdrawAsync(
        Guid supplierQuoteId,
        WithdrawSupplierQuoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                validation.Errors);
        }

        var quote = await _db.Set<SupplierQuote>()
            .Include(x => x.Revisions)
            .SingleOrDefaultAsync(
                x => x.Id == supplierQuoteId,
                cancellationToken);

        if (quote is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(
                    nameof(SupplierQuote),
                    supplierQuoteId));
        }

        if (quote.Status == SupplierQuoteStatus.Withdrawn)
        {
            return Result.Ok(
                await ProjectRequiredAsync(
                    quote.Id,
                    cancellationToken));
        }

        if (!CanWithdraw(
                quote.Status))
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError(
                    $"SupplierQuote with status '{quote.Status}' cannot be withdrawn."));
        }

        var currentRevision = FindCurrentCommercialRevision(
            quote);

        if (currentRevision is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError(
                    "SupplierQuote does not have a current commercial revision."));
        }

        if (IsExpired(
                currentRevision,
                DateTimeOffset.UtcNow))
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConflictError(
                    "SupplierQuote has already reached ValidToUtc and should be expired instead of withdrawn."));
        }

        _db.Entry(quote)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        quote.Status = SupplierQuoteStatus.Withdrawn;
        quote.ActionDateUtc = DateTimeOffset.UtcNow;

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new ConcurrencyError(
                    nameof(SupplierQuote),
                    quote.Id));
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                quote.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kontrollerar om quote-rooten ligger i ett läge där supplier-withdrawal fortfarande är relevant.
    /// </summary>
    private static bool CanWithdraw(
        SupplierQuoteStatus status)
    {
        return status is SupplierQuoteStatus.Received
            or SupplierQuoteStatus.UnderReview
            or SupplierQuoteStatus.AcceptedInternally;
    }

    /// <summary>
    /// Markerar current commercial revision och quote-root som Expired när ValidToUtc har passerat.
    /// </summary>
    public async Task<Result<SupplierQuoteResponse>> ExpireAsync(
        Guid supplierQuoteId,
        ExpireSupplierQuoteRequest request,
        DateTimeOffset evaluatedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                validation.Errors);
        }

        var quote = await LoadQuoteForWriteAsync(
            supplierQuoteId,
            request.SupplierQuoteRevisionId,
            cancellationToken);

        if (quote is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(
                    nameof(SupplierQuote),
                    supplierQuoteId));
        }

        var revision = quote.Revisions
            .SingleOrDefault(
                x => x.Id == request.SupplierQuoteRevisionId);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteResponse>(
                new NotFoundError(
                    nameof(SupplierQuoteRevision),
                    request.SupplierQuoteRevisionId));
        }

        if (quote.Status == SupplierQuoteStatus.Expired
            && revision.Status == SupplierQuoteRevisionStatus.Expired)
        {
            return Result.Ok(
                await ProjectRequiredAsync(
                    quote.Id,
                    cancellationToken));
        }

        var expireResult = ValidateCanExpire(
            quote,
            revision,
            evaluatedAtUtc);

        if (expireResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                expireResult.Errors);
        }

        _db.Entry(quote)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRowVersion;

        _db.Entry(revision)
            .Property(x => x.RowVersion)
            .OriginalValue = request.SupplierQuoteRevisionRowVersion;

        quote.Status = SupplierQuoteStatus.Expired;
        quote.ActionDateUtc = evaluatedAtUtc;

        revision.Status = SupplierQuoteRevisionStatus.Expired;
        revision.ActionDateUtc = evaluatedAtUtc;

        var saveResult = await SaveWithConcurrencyAsync(
            quote,
            revision,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteResponse>(
                saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                quote.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kontrollerar att aktuell revision faktiskt är due för expiration.
    /// </summary>
    private static Result ValidateCanExpire(
        SupplierQuote quote,
        SupplierQuoteRevision revision,
        DateTimeOffset evaluatedAtUtc)
    {
        if (quote.Status is not (
            SupplierQuoteStatus.Received
            or SupplierQuoteStatus.UnderReview
            or SupplierQuoteStatus.AcceptedInternally))
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplierQuote with status '{quote.Status}' cannot expire."));
        }

        if (revision.Status is not (
            SupplierQuoteRevisionStatus.Received
            or SupplierQuoteRevisionStatus.AcceptedInternally))
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplierQuoteRevision with status '{revision.Status}' cannot expire."));
        }

        if (revision.RevisionNumber != quote.LatestRevisionNumber)
        {
            return Result.Fail(
                new ConflictError(
                    "Only the latest commercial SupplierQuoteRevision can expire the quote-root."));
        }

        if (!revision.ValidToUtc.HasValue)
        {
            return Result.Fail(
                new ConflictError(
                    "SupplierQuoteRevision does not have a ValidToUtc and therefore cannot expire automatically."));
        }

        if (revision.ValidToUtc.Value > evaluatedAtUtc)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplierQuoteRevision remains valid until '{revision.ValidToUtc.Value:O}'."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Expire:ar due current supplier-quote-revisions och deras quote-roots i en transaktion.
    /// </summary>
    public async Task<Result<SupplierQuoteExpirationBatchResponse>> ExpireDueAsync(
        DateTimeOffset evaluatedAtUtc,
        int batchSize = 500,
        CancellationToken cancellationToken = default)
    {
        if (batchSize <= 0)
        {
            return Result.Fail<SupplierQuoteExpirationBatchResponse>(
                new ConflictError(
                    "BatchSize must be greater than zero."));
        }

        var due = await _db.Set<SupplierQuoteRevision>()
            .AsNoTracking()
            .Where(x =>
                x.ValidToUtc.HasValue
                && x.ValidToUtc.Value <= evaluatedAtUtc
                && x.SnapshotJson != null
                && x.SnapshotHash != null
                && x.RevisionNumber == x.SupplierQuote.LatestRevisionNumber
                && (
                    x.Status == SupplierQuoteRevisionStatus.Received
                    || x.Status == SupplierQuoteRevisionStatus.AcceptedInternally
                )
                && (
                    x.SupplierQuote.Status == SupplierQuoteStatus.Received
                    || x.SupplierQuote.Status == SupplierQuoteStatus.UnderReview
                    || x.SupplierQuote.Status == SupplierQuoteStatus.AcceptedInternally
                ))
            .OrderBy(x => x.ValidToUtc)
            .ThenBy(x => x.Id)
            .Select(x => new ExpiringSupplierQuoteReference
            {
                SupplierQuoteId = x.SupplierQuoteId,
                SupplierQuoteRevisionId = x.Id
            })
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (due.Count == 0)
        {
            return Result.Ok(
                new SupplierQuoteExpirationBatchResponse
                {
                    EvaluatedAtUtc = evaluatedAtUtc,
                    CandidateCount = 0,
                    ExpiredRevisionCount = 0,
                    ExpiredQuoteCount = 0
                });
        }

        var revisionIds = due
            .Select(x => x.SupplierQuoteRevisionId)
            .ToArray();

        var quoteIds = due
            .Select(x => x.SupplierQuoteId)
            .Distinct()
            .ToArray();

        await using var transaction = await _db.Database.BeginTransactionAsync(
            cancellationToken);

        try
        {
            /*
             * Status och ValidTo upprepas i UPDATE-villkoret.
             * En revision som hunnit Superseded/Rejected mellan SELECT och UPDATE
             * kommer därför inte att skrivas över.
             */
            var expiredRevisionCount = await _db.Set<SupplierQuoteRevision>()
                .Where(x =>
                    revisionIds.Contains(x.Id)
                    && x.ValidToUtc.HasValue
                    && x.ValidToUtc.Value <= evaluatedAtUtc
                    && (
                        x.Status == SupplierQuoteRevisionStatus.Received
                        || x.Status == SupplierQuoteRevisionStatus.AcceptedInternally
                    ))
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            x => x.Status,
                            SupplierQuoteRevisionStatus.Expired)
                        .SetProperty(
                            x => x.ActionDateUtc,
                            evaluatedAtUtc),
                    cancellationToken);

            /*
             * Rooten expire:as bara om dess latest revision faktiskt är en av de
             * nyss Expired revisionerna. En ny revision som hunnit bli current
             * gör alltså att gamla SELECT-resultatet inte kan expire:a rooten.
             */
            var expiredQuoteCount = await _db.Set<SupplierQuote>()
                .Where(x =>
                    quoteIds.Contains(x.Id)
                    && (
                        x.Status == SupplierQuoteStatus.Received
                        || x.Status == SupplierQuoteStatus.UnderReview
                        || x.Status == SupplierQuoteStatus.AcceptedInternally
                    )
                    && x.Revisions.Any(
                        revision =>
                            revision.RevisionNumber == x.LatestRevisionNumber
                            && revisionIds.Contains(revision.Id)
                            && revision.Status == SupplierQuoteRevisionStatus.Expired))
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            x => x.Status,
                            SupplierQuoteStatus.Expired)
                        .SetProperty(
                            x => x.ActionDateUtc,
                            evaluatedAtUtc),
                    cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return Result.Ok(
                new SupplierQuoteExpirationBatchResponse
                {
                    EvaluatedAtUtc = evaluatedAtUtc,
                    CandidateCount = due.Count,
                    ExpiredRevisionCount = expiredRevisionCount,
                    ExpiredQuoteCount = expiredQuoteCount
                });
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    /// <summary>
    /// Kontrollerar om revisionens sista giltighetstid har passerat.
    /// </summary>
    private static bool IsExpired(
        SupplierQuoteRevision revision,
        DateTimeOffset evaluatedAtUtc)
    {
        return revision.ValidToUtc.HasValue
            && revision.ValidToUtc.Value <= evaluatedAtUtc;
    }

    /// <summary>
    /// Hittar den revision som fortfarande representerar aktuell kommersiell supplier-data.
    /// </summary>
    private static SupplierQuoteRevision? FindCurrentCommercialRevision(
        SupplierQuote quote)
    {
        return quote.Revisions
            .SingleOrDefault(
                x => x.RevisionNumber == quote.LatestRevisionNumber
                    && (
                        x.Status == SupplierQuoteRevisionStatus.Received
                        || x.Status == SupplierQuoteRevisionStatus.AcceptedInternally
                    ));
    }

    /// <summary>
    /// Läser quote och exakt den revision som operationen avser som tracked entities.
    /// </summary>
    private Task<SupplierQuote?> LoadQuoteForWriteAsync(
        Guid supplierQuoteId,
        Guid supplierQuoteRevisionId,
        CancellationToken cancellationToken)
    {
        return _db.Set<SupplierQuote>()
            .Include(x => x.Revisions.Where(
                revision =>
                    revision.Id == supplierQuoteRevisionId))
            .SingleOrDefaultAsync(
                x => x.Id == supplierQuoteId,
                cancellationToken);
    }

    /// <summary>
    /// Sparar root och revision och översätter optimistic-concurrency-fel.
    /// </summary>
    private async Task<Result> SaveWithConcurrencyAsync(
        SupplierQuote quote,
        SupplierQuoteRevision revision,
        CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            if (exception.Entries.Any(
                    x => x.Entity is SupplierQuote))
            {
                return Result.Fail(
                    new ConcurrencyError(
                        nameof(SupplierQuote),
                        quote.Id));
            }

            return Result.Fail(
                new ConcurrencyError(
                    nameof(SupplierQuoteRevision),
                    revision.Id));
        }
    }

    /// <summary>
    /// Lägger till ett supplier-quote-readiness-problem.
    /// </summary>
    private static void AddIssue(
        ICollection<SupplierQuoteReadinessIssueResponse> issues,
        string code,
        string message,
        Guid? entityId = null)
    {
        issues.Add(
            new SupplierQuoteReadinessIssueResponse
            {
                Code = code,
                Message = message,
                EntityId = entityId
            });
    }

    /// <summary>
    /// Projekterar quote-rooten tillsammans med revisionshistoriken.
    /// </summary>
    private async Task<SupplierQuoteResponse> ProjectRequiredAsync(
        Guid supplierQuoteId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<SupplierQuote>()
            .AsNoTracking()
            .Where(x => x.Id == supplierQuoteId)
            .Select(SupplierQuoteProjections.Details)
            .SingleAsync(cancellationToken);
    }



    /// <summary>
    /// Innehåller de två identifierare som behövs vid batch-expiration.
    /// </summary>
    private sealed class ExpiringSupplierQuoteReference
    {
        /// <summary>
        /// Identifierar quote-roten.
        /// </summary>
        public Guid SupplierQuoteId { get; init; }

        /// <summary>
        /// Identifierar latest current revision.
        /// </summary>
        public Guid SupplierQuoteRevisionId { get; init; }
    }
}