using System.Text.Json;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Trading.Snapshots;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Skapar reproducerbara matching runs från released RFQ-snapshots och publicerade regelversioner.
/// </summary>
public sealed class MatchingRunService : IMatchingRunService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ICanonicalJsonService _canonicalJsonService;
    private readonly ILogger<MatchingRunService> _logger;

    /// <summary>
    /// Skapar run-servicen med databas-, validerings- och JSON-beroenden.
    /// </summary>
    public MatchingRunService(Arb_IntDbContext db, IValidationService validationService, ICanonicalJsonService canonicalJsonService, ILogger<MatchingRunService> logger)
    {
        _db = db;
        _validationService = validationService;
        _canonicalJsonService = canonicalJsonService;
        _logger = logger;
    }

    /// <summary>
    /// Hämtar en matchningskörning med dess frysta input-lines.
    /// </summary>
    public async Task<Result<MatchingRunResponse>> GetByIdAsync(Guid matchingRunId, CancellationToken cancellationToken = default)
    {
        var response = await ProjectAsync(matchingRunId, cancellationToken);

        return response is null
            ? Result.Fail<MatchingRunResponse>(new NotFoundError(nameof(MatchingRun), matchingRunId))
            : Result.Ok(response);
    }

    /// <summary>
    /// Hämtar historiska runs för en RFQ-revision med senaste run först.
    /// </summary>
    public async Task<Result<IReadOnlyList<MatchingRunResponse>>> GetByRevisionAsync(Guid buyerRfqRevisionId, CancellationToken cancellationToken = default)
    {
        var revisionExists = await _db.Set<BuyerRfqRevision>().AsNoTracking().AnyAsync(x => x.Id == buyerRfqRevisionId, cancellationToken);

        if (!revisionExists)
        {
            return Result.Fail<IReadOnlyList<MatchingRunResponse>>(new NotFoundError(nameof(BuyerRfqRevision), buyerRfqRevisionId));
        }

        var runs = await _db.Set<MatchingRun>()
            .AsNoTracking()
            .Where(x => x.BuyerRfqRevisionId == buyerRfqRevisionId)
            .OrderByDescending(x => x.QueuedAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(MatchingProjections.RunDetails)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<MatchingRunResponse>>(runs);
    }

    /// <summary>
    /// Fryser RFQ- och rule-input och skapar en queued matching run.
    /// </summary>
    public async Task<Result<MatchingRunResponse>> QueueAsync(Guid buyerRfqRevisionId, QueueMatchingRunRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<MatchingRunResponse>(validation.Errors);
        }

        var queuedAtUtc = DateTimeOffset.UtcNow;

        var revision = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqRevisionId)
            .Select(x => new
            {
                x.Id,
                x.Status,
                x.SnapshotJson,
                x.SnapshotHash
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (revision is null)
        {
            return Result.Fail<MatchingRunResponse>(new NotFoundError(nameof(BuyerRfqRevision), buyerRfqRevisionId));
        }

        if (revision.Status != BuyerRfqRevisionStatus.ReleasedForMatching)
        {
            return Result.Fail<MatchingRunResponse>(new ConflictError("Matching can only be queued for a ReleasedForMatching BuyerRfqRevision."));
        }

        if (string.IsNullOrWhiteSpace(revision.SnapshotJson) || string.IsNullOrWhiteSpace(revision.SnapshotHash))
        {
            return Result.Fail<MatchingRunResponse>(new ConflictError("BuyerRfqRevision does not contain a released snapshot."));
        }

        var computedRfqHash = _canonicalJsonService.ComputeHash(revision.SnapshotJson);

        if (!string.Equals(computedRfqHash, revision.SnapshotHash, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Matching run could not be queued because Buyer RFQ revision {BuyerRfqRevisionId} snapshot verification failed",
                revision.Id);

            return Result.Fail<MatchingRunResponse>(new ConflictError("BuyerRfqRevision SnapshotHash does not match SnapshotJson."));
        }

        var versionResult = await ResolveRuleSetVersionAsync(request, queuedAtUtc, cancellationToken);

        if (versionResult.IsFailed)
        {
            return Result.Fail<MatchingRunResponse>(versionResult.Errors);
        }

        var version = versionResult.Value;

        var computedConfigurationHash = _canonicalJsonService.ComputeHash(version.ConfigurationJson);

        if (!string.Equals(computedConfigurationHash, version.ConfigurationHash, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Matching run could not be queued because matching rule set version {MatchingRuleSetVersionId} configuration verification failed",
                version.Id);

            return Result.Fail<MatchingRunResponse>(new ConflictError("MatchingRuleSetVersion ConfigurationHash does not match ConfigurationJson."));
        }

        using var rfqDocument = JsonDocument.Parse(revision.SnapshotJson);
        using var configurationDocument = JsonDocument.Parse(version.ConfigurationJson);

        var inputSnapshot = new MatchingRunInputSnapshot
        {
            BuyerRfqRevisionId = revision.Id,
            BuyerRfqSnapshotHash = revision.SnapshotHash,
            BuyerRfqSnapshot = rfqDocument.RootElement.Clone(),
            MatchingRuleSetId = version.MatchingRuleSetId,
            MatchingRuleSetCode = version.MatchingRuleSet.Code,
            MatchingRuleSetVersionId = version.Id,
            MatchingRuleSetVersionNumber = version.VersionNumber,
            MatchingConfigurationHash = version.ConfigurationHash,
            MatchingConfiguration = configurationDocument.RootElement.Clone()
        };

        var serializedInput = JsonSerializer.Serialize(inputSnapshot);
        var canonicalInput = _canonicalJsonService.CanonicalizeAndHash(serializedInput);

        var lineResult = await CreateRunLinesAsync(
            revision.Id,
            rfqDocument.RootElement,
            cancellationToken);

        if (lineResult.IsFailed)
        {
            _logger.LogWarning(
                "Matching run could not be queued because Buyer RFQ revision {BuyerRfqRevisionId} snapshot lines were inconsistent with persisted lines. ErrorCount={ErrorCount}",
                revision.Id,
                lineResult.Errors.Count);

            return Result.Fail<MatchingRunResponse>(lineResult.Errors);
        }

        var run = new MatchingRun
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqRevisionId = revision.Id,
            MatchingRuleSetVersionId = version.Id,
            Status = MatchingRunStatus.Queued,
            QueuedAtUtc = queuedAtUtc,
            StartedAtUtc = null,
            CompletedAtUtc = null,
            StartedByUserId = request.StartedByUserId,
            TriggerName = TrimToNull(request.TriggerName),
            CorrelationId = TrimToNull(request.CorrelationId),
            InputSnapshotJson = canonicalInput.Json,
            InputSnapshotHash = canonicalInput.Hash,
            ErrorMessage = null
        };

        foreach (var line in lineResult.Value)
        {
            line.MatchingRunId = run.Id;
            run.Lines.Add(line);
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _db.Set<MatchingRun>().Add(run);
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Matching run {MatchingRunId} queued for Buyer RFQ revision {BuyerRfqRevisionId} with rule set {MatchingRuleSetId} version {MatchingRuleSetVersionId}. LineCount={LineCount}, TriggerName={TriggerName}, CorrelationId={CorrelationId}",
                run.Id,
                revision.Id,
                version.MatchingRuleSetId,
                version.Id,
                run.Lines.Count,
                run.TriggerName,
                run.CorrelationId);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return Result.Ok(await ProjectRequiredAsync(run.Id, cancellationToken));
    }

    /// <summary>
    /// Avbryter en queued eller running run utan att radera historiska resultat.
    /// </summary>
    public async Task<Result<MatchingRunResponse>> CancelAsync(Guid matchingRunId, CancelMatchingRunRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<MatchingRunResponse>(validation.Errors);
        }

        var run = await _db.Set<MatchingRun>().SingleOrDefaultAsync(x => x.Id == matchingRunId, cancellationToken);

        if (run is null)
        {
            return Result.Fail<MatchingRunResponse>(new NotFoundError(nameof(MatchingRun), matchingRunId));
        }

        if (run.Status is MatchingRunStatus.Completed or MatchingRunStatus.Failed or MatchingRunStatus.Cancelled)
        {
            return Result.Fail<MatchingRunResponse>(new ConflictError($"MatchingRun with status '{run.Status}' cannot be cancelled."));
        }

        _db.Entry(run).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        var previousStatus = run.Status;

        run.Status = MatchingRunStatus.Cancelled;
        run.CompletedAtUtc = DateTimeOffset.UtcNow;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(
                exception,
                "Concurrency conflict while cancelling matching run {MatchingRunId}",
                run.Id);

            return Result.Fail<MatchingRunResponse>(new ConcurrencyError(nameof(MatchingRun), run.Id));
        }

        _logger.LogInformation(
            "Matching run {MatchingRunId} cancelled from status {PreviousStatus}",
            run.Id,
            previousStatus);

        return Result.Ok(await ProjectRequiredAsync(run.Id, cancellationToken));
    }

    /// <summary>
    /// Väljer explicit eller senast publicerad effektiv version av ett aktivt regelverk.
    /// </summary>
    private async Task<Result<MatchingRuleSetVersion>> ResolveRuleSetVersionAsync(QueueMatchingRunRequest request, DateTimeOffset effectiveAtUtc, CancellationToken cancellationToken)
    {
        var ruleSet = await _db.Set<MatchingRuleSet>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.MatchingRuleSetId, cancellationToken);

        if (ruleSet is null)
        {
            return Result.Fail<MatchingRuleSetVersion>(new NotFoundError(nameof(MatchingRuleSet), request.MatchingRuleSetId));
        }

        if (ruleSet.Status != MatchingRuleSetStatus.Active)
        {
            return Result.Fail<MatchingRuleSetVersion>(new ConflictError($"MatchingRuleSet with status '{ruleSet.Status}' cannot be used for new matching runs."));
        }

        IQueryable<MatchingRuleSetVersion> query = _db.Set<MatchingRuleSetVersion>()
            .AsNoTracking()
            .Include(x => x.MatchingRuleSet)
            .Where(x =>
                x.MatchingRuleSetId == ruleSet.Id
                && x.PublishedAtUtc.HasValue
                && (!x.EffectiveFromUtc.HasValue || x.EffectiveFromUtc.Value <= effectiveAtUtc)
                && (!x.EffectiveToUtc.HasValue || x.EffectiveToUtc.Value >= effectiveAtUtc));

        if (request.MatchingRuleSetVersionId.HasValue)
        {
            var explicitVersion = await query.SingleOrDefaultAsync(x => x.Id == request.MatchingRuleSetVersionId.Value, cancellationToken);

            return explicitVersion is null
                ? Result.Fail<MatchingRuleSetVersion>(new ConflictError("The selected MatchingRuleSetVersion is not published and effective for this run."))
                : Result.Ok(explicitVersion);
        }

        var version = await query
            .OrderByDescending(x => x.VersionNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return version is null
            ? Result.Fail<MatchingRuleSetVersion>(new ConflictError("MatchingRuleSet has no published and effective version."))
            : Result.Ok(version);
    }

    /// <summary>
    /// Bygger run-lines från exakt de line-objekt som finns i RFQ-snapshotet.
    /// </summary>
    private async Task<Result<IReadOnlyList<MatchingRunLine>>> CreateRunLinesAsync(Guid buyerRfqRevisionId, JsonElement rfqSnapshot, CancellationToken cancellationToken)
    {
        if (!TryGetProperty(rfqSnapshot, "lines", out var linesElement) || linesElement.ValueKind != JsonValueKind.Array)
        {
            return Result.Fail<IReadOnlyList<MatchingRunLine>>(new ConflictError("BuyerRfq snapshot does not contain a valid lines array."));
        }

        var persistedLines = await _db.Set<BuyerRfqLine>()
            .AsNoTracking()
            .Where(x => x.BuyerRfqRevisionId == buyerRfqRevisionId)
            .Select(x => new
            {
                x.Id,
                x.LineNumber
            })
            .ToListAsync(cancellationToken);

        var persistedByNumber = persistedLines.ToDictionary(x => x.LineNumber);
        var result = new List<MatchingRunLine>();

        foreach (var lineElement in linesElement.EnumerateArray())
        {
            if (!TryGetProperty(lineElement, "lineNumber", out var lineNumberElement)
                || !TryGetProperty(lineElement, "requiredQuantity", out var requiredQuantityElement)
                || !TryGetProperty(lineElement, "quantityUnitId", out var quantityUnitElement))
            {
                return Result.Fail<IReadOnlyList<MatchingRunLine>>(new ConflictError("BuyerRfq line snapshot is missing required matching fields."));
            }

            var lineNumber = lineNumberElement.GetInt32();

            if (!persistedByNumber.TryGetValue(lineNumber, out var persistedLine))
            {
                return Result.Fail<IReadOnlyList<MatchingRunLine>>(new ConflictError($"BuyerRfq snapshot line '{lineNumber}' does not exist in the released revision."));
            }

            result.Add(
                new MatchingRunLine
                {
                    Id = Guid.CreateVersion7(),
                    BuyerRfqRevisionId = buyerRfqRevisionId,
                    BuyerRfqLineId = persistedLine.Id,
                    RequiredQuantity = requiredQuantityElement.GetDecimal(),
                    QuantityUnitId = quantityUnitElement.GetInt32(),
                    InputSnapshotJson = _canonicalJsonService.Canonicalize(lineElement.GetRawText())
                });
        }

        if (result.Count != persistedLines.Count)
        {
            return Result.Fail<IReadOnlyList<MatchingRunLine>>(new ConflictError("BuyerRfq snapshot line count does not match the released revision."));
        }

        return Result.Ok<IReadOnlyList<MatchingRunLine>>(result);
    }

    /// <summary>
    /// Hämtar en JSON-property utan att vara beroende av property casing.
    /// </summary>
    private static bool TryGetProperty(JsonElement element, string propertyName, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Projekterar en matching run från databasen.
    /// </summary>
    private Task<MatchingRunResponse?> ProjectAsync(Guid matchingRunId, CancellationToken cancellationToken)
    {
        return _db.Set<MatchingRun>()
            .AsNoTracking()
            .Where(x => x.Id == matchingRunId)
            .Select(MatchingProjections.RunDetails)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar en sparad run och kastar om den oväntat saknas.
    /// </summary>
    private async Task<MatchingRunResponse> ProjectRequiredAsync(Guid matchingRunId, CancellationToken cancellationToken)
    {
        return await ProjectAsync(matchingRunId, cancellationToken)
            ?? throw new InvalidOperationException($"MatchingRun '{matchingRunId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Trimmar text och ersätter whitespace med null.
    /// </summary>
    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}