using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Persisterar förklarbara requirement- och scoringresultat från matchningsmotorn.
/// </summary>
public sealed class MatchingEvaluationExecutionService : IMatchingEvaluationExecutionService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ICanonicalJsonService _canonicalJsonService;

    /// <summary>
    /// Skapar execution-servicen med databas-, validerings- och JSON-beroenden.
    /// </summary>
    public MatchingEvaluationExecutionService(Arb_IntDbContext db, IValidationService validationService, ICanonicalJsonService canonicalJsonService)
    {
        _db = db;
        _validationService = validationService;
        _canonicalJsonService = canonicalJsonService;
    }

    /// <summary>
    /// Skapar en requirement evaluation och verifierar hela provenance-kedjan mot candidate och RFQ-revision.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> AddRequirementEvaluationAsync(
        Guid supplySolutionCandidateId,
        CreateRequirementEvaluationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var candidate = await LoadCandidateForWriteAsync(supplySolutionCandidateId, cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var requirement = await _db.Set<BuyerRfqRequirement>()
            .AsNoTracking()
            .Where(x => x.Id == request.BuyerRfqRequirementId)
            .Select(x => new
            {
                x.Id,
                x.BuyerRfqRevisionId,
                x.BuyerRfqLineId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (requirement is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(BuyerRfqRequirement), request.BuyerRfqRequirementId));
        }

        if (requirement.BuyerRfqRevisionId != candidate.MatchingRun.BuyerRfqRevisionId)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError("BuyerRfqRequirement does not belong to the BuyerRfqRevision used by this MatchingRun."));
        }

        var targetResult = await ResolveEvaluationTargetAsync(
            candidate.Id,
            request.SupplySolutionLineId,
            request.SupplyAllocationId,
            request.SupplySolutionStepId,
            cancellationToken);

        if (targetResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(targetResult.Errors);
        }

        var target = targetResult.Value;

        if (requirement.BuyerRfqLineId.HasValue)
        {
            if (!target.BuyerRfqLineId.HasValue)
            {
                return Result.Fail<SupplySolutionCandidateResponse>(
                    new ConflictError("A line-specific BuyerRfqRequirement must be evaluated against a target that resolves to the same RFQ line."));
            }

            if (target.BuyerRfqLineId.Value != requirement.BuyerRfqLineId.Value)
            {
                return Result.Fail<SupplySolutionCandidateResponse>(
                    new ConflictError("The evaluation target does not belong to the RFQ line that owns the BuyerRfqRequirement."));
            }
        }

        var unitResult = await ValidateOptionalUnitAsync(
            request.UnitOfMeasureId,
            cancellationToken);

        if (unitResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(unitResult.Errors);
        }

        var evaluation = new RequirementEvaluation
        {
            Id = Guid.CreateVersion7(),
            SupplySolutionCandidateId = candidate.Id,
            BuyerRfqRequirementId = requirement.Id,
            SupplySolutionLineId = target.SupplySolutionLineId,
            SupplyAllocationId = request.SupplyAllocationId,
            SupplySolutionStepId = request.SupplySolutionStepId,
            Outcome = request.Outcome,
            IsHardFail = request.IsHardFail,
            VerificationState = request.VerificationState,
            ActualNumericValue = request.ActualNumericValue,
            ActualTextValue = TrimToNull(request.ActualTextValue),
            UnitOfMeasureId = request.UnitOfMeasureId,
            Explanation = TrimToNull(request.Explanation),
            EvidenceSnapshotJson = CanonicalizeOptionalJson(request.EvidenceSnapshotJson),
            RuleTraceJson = CanonicalizeOptionalJson(request.RuleTraceJson)
        };

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<RequirementEvaluation>().Add(evaluation);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(RequirementEvaluation),
            evaluation.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar det beräknade resultatet utan att ändra requirement eller evaluation-scope.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> UpdateRequirementEvaluationAsync(
        Guid requirementEvaluationId,
        UpdateRequirementEvaluationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var evaluation = await LoadEvaluationForWriteAsync(
            requirementEvaluationId,
            cancellationToken);

        if (evaluation is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(RequirementEvaluation), requirementEvaluationId));
        }

        var candidate = evaluation.SupplySolutionCandidate;

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var unitResult = await ValidateOptionalUnitAsync(
            request.UnitOfMeasureId,
            cancellationToken);

        if (unitResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(unitResult.Errors);
        }

        evaluation.Outcome = request.Outcome;
        evaluation.IsHardFail = request.IsHardFail;
        evaluation.VerificationState = request.VerificationState;
        evaluation.ActualNumericValue = request.ActualNumericValue;
        evaluation.ActualTextValue = TrimToNull(request.ActualTextValue);
        evaluation.UnitOfMeasureId = request.UnitOfMeasureId;
        evaluation.Explanation = TrimToNull(request.Explanation);
        evaluation.EvidenceSnapshotJson = CanonicalizeOptionalJson(request.EvidenceSnapshotJson);
        evaluation.RuleTraceJson = CanonicalizeOptionalJson(request.RuleTraceJson);

        _db.Entry(evaluation).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(RequirementEvaluation),
            evaluation.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Tar bort en evaluation om inga score contributions fortfarande refererar till den.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> DeleteRequirementEvaluationAsync(
        Guid requirementEvaluationId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var evaluation = await LoadEvaluationForWriteAsync(
            requirementEvaluationId,
            cancellationToken);

        if (evaluation is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(RequirementEvaluation), requirementEvaluationId));
        }

        var candidate = evaluation.SupplySolutionCandidate;

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var isReferenced = await _db.Set<ScoreContribution>()
            .AsNoTracking()
            .AnyAsync(
                x => x.RequirementEvaluationId == evaluation.Id,
                cancellationToken);

        if (isReferenced)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError("RequirementEvaluation cannot be deleted while ScoreContributions reference it."));
        }

        _db.Entry(evaluation).Property(x => x.RowVersion).OriginalValue = rowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<RequirementEvaluation>().Remove(evaluation);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(RequirementEvaluation),
            evaluation.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Skapar ett score contribution och verifierar eventuell evaluation mot samma candidate.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> AddScoreContributionAsync(
        Guid supplySolutionCandidateId,
        CreateScoreContributionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var candidate = await LoadCandidateForWriteAsync(
            supplySolutionCandidateId,
            cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        if (request.RequirementEvaluationId.HasValue)
        {
            var evaluationExists = await _db.Set<RequirementEvaluation>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == request.RequirementEvaluationId.Value
                        && x.SupplySolutionCandidateId == candidate.Id,
                    cancellationToken);

            if (!evaluationExists)
            {
                return Result.Fail<SupplySolutionCandidateResponse>(
                    new ConflictError("RequirementEvaluation does not belong to the selected SupplySolutionCandidate."));
            }
        }

        var contribution = new ScoreContribution
        {
            Id = Guid.CreateVersion7(),
            SupplySolutionCandidateId = candidate.Id,
            RequirementEvaluationId = request.RequirementEvaluationId,
            Category = TrimToNull(request.Category),
            Weight = request.Weight,
            NormalizedScore = request.NormalizedScore,
            Points = request.Points,
            Explanation = TrimToNull(request.Explanation)
        };

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<ScoreContribution>().Add(contribution);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(ScoreContribution),
            contribution.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar ett score contribution utan att ändra dess evaluation-provenance.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> UpdateScoreContributionAsync(
        Guid scoreContributionId,
        UpdateScoreContributionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var contribution = await LoadScoreContributionForWriteAsync(
            scoreContributionId,
            cancellationToken);

        if (contribution is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(ScoreContribution), scoreContributionId));
        }

        var candidate = contribution.SupplySolutionCandidate;

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        contribution.Category = TrimToNull(request.Category);
        contribution.Weight = request.Weight;
        contribution.NormalizedScore = request.NormalizedScore;
        contribution.Points = request.Points;
        contribution.Explanation = TrimToNull(request.Explanation);

        _db.Entry(contribution).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(ScoreContribution),
            contribution.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Tar bort ett score contribution under pågående matching.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> DeleteScoreContributionAsync(
        Guid scoreContributionId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var contribution = await LoadScoreContributionForWriteAsync(
            scoreContributionId,
            cancellationToken);

        if (contribution is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(ScoreContribution), scoreContributionId));
        }

        var candidate = contribution.SupplySolutionCandidate;

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        _db.Entry(contribution).Property(x => x.RowVersion).OriginalValue = rowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<ScoreContribution>().Remove(contribution);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(ScoreContribution),
            contribution.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Sparar regelmotorns sammanvägda resultat utan att ändra kandidatens workflow-status.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> SetCandidateEvaluationSummaryAsync(
        Guid supplySolutionCandidateId,
        SetCandidateEvaluationSummaryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var candidate = await LoadCandidateForWriteAsync(
            supplySolutionCandidateId,
            cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        candidate.Outcome = request.Outcome;
        candidate.VerificationState = request.VerificationState;
        candidate.TotalScore = request.TotalScore;
        candidate.ContainsHardFail = request.ContainsHardFail;

        _db.Entry(candidate).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        TouchRun(candidate.MatchingRun);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(SupplySolutionCandidate),
            candidate.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                candidate.Id,
                cancellationToken));
    }

    /// <summary>
    /// Verifierar att line, allocation och step hör till samma candidate och härleder eventuell RFQ-line.
    /// </summary>
    private async Task<Result<(Guid? SupplySolutionLineId, Guid? BuyerRfqLineId)>> ResolveEvaluationTargetAsync(
        Guid supplySolutionCandidateId,
        Guid? supplySolutionLineId,
        Guid? supplyAllocationId,
        Guid? supplySolutionStepId,
        CancellationToken cancellationToken)
    {
        Guid? resolvedSolutionLineId = null;
        Guid? resolvedBuyerRfqLineId = null;

        if (supplySolutionLineId.HasValue)
        {
            var line = await _db.Set<SupplySolutionLine>()
                .AsNoTracking()
                .Where(x =>
                    x.Id == supplySolutionLineId.Value
                    && x.SupplySolutionCandidateId == supplySolutionCandidateId)
                .Select(x => new
                {
                    x.Id,
                    BuyerRfqLineId = x.MatchingRunLine.BuyerRfqLineId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (line is null)
            {
                return Result.Fail<(Guid?, Guid?)>(
                    new ConflictError("SupplySolutionLine does not belong to the selected SupplySolutionCandidate."));
            }

            resolvedSolutionLineId = line.Id;
            resolvedBuyerRfqLineId = line.BuyerRfqLineId;
        }

        if (supplyAllocationId.HasValue)
        {
            var allocation = await _db.Set<SupplyAllocation>()
                .AsNoTracking()
                .Where(x =>
                    x.Id == supplyAllocationId.Value
                    && x.SupplySolutionLine.SupplySolutionCandidateId == supplySolutionCandidateId)
                .Select(x => new
                {
                    x.SupplySolutionLineId,
                    BuyerRfqLineId = x.SupplySolutionLine.MatchingRunLine.BuyerRfqLineId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (allocation is null)
            {
                return Result.Fail<(Guid?, Guid?)>(
                    new ConflictError("SupplyAllocation does not belong to the selected SupplySolutionCandidate."));
            }

            if (resolvedSolutionLineId.HasValue && resolvedSolutionLineId.Value != allocation.SupplySolutionLineId)
            {
                return Result.Fail<(Guid?, Guid?)>(
                    new ConflictError("SupplyAllocation and SupplySolutionLine reference different solution lines."));
            }

            resolvedSolutionLineId = allocation.SupplySolutionLineId;
            resolvedBuyerRfqLineId = allocation.BuyerRfqLineId;
        }

        if (supplySolutionStepId.HasValue)
        {
            var step = await _db.Set<SupplySolutionStep>()
                .AsNoTracking()
                .Where(x =>
                    x.Id == supplySolutionStepId.Value
                    && x.SupplySolutionCandidateId == supplySolutionCandidateId)
                .Select(x => new
                {
                    x.SupplySolutionLineId,
                    BuyerRfqLineId = x.SupplySolutionLineId.HasValue
                        ? x.SupplySolutionLine!.MatchingRunLine.BuyerRfqLineId
                        : (Guid?)null
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (step is null)
            {
                return Result.Fail<(Guid?, Guid?)>(
                    new ConflictError("SupplySolutionStep does not belong to the selected SupplySolutionCandidate."));
            }

            if (step.SupplySolutionLineId.HasValue)
            {
                if (resolvedSolutionLineId.HasValue && resolvedSolutionLineId.Value != step.SupplySolutionLineId.Value)
                {
                    return Result.Fail<(Guid?, Guid?)>(
                        new ConflictError("SupplySolutionStep resolves to a different SupplySolutionLine."));
                }

                resolvedSolutionLineId = step.SupplySolutionLineId;
                resolvedBuyerRfqLineId = step.BuyerRfqLineId;
            }
        }

        return Result.Ok(
            (resolvedSolutionLineId, resolvedBuyerRfqLineId));
    }

    /// <summary>
    /// Kontrollerar att kandidaten fortfarande kan ändras av matchningsmotorn.
    /// </summary>
    private static Result ValidateCandidateIsWritable(SupplySolutionCandidate candidate)
    {
        if (candidate.MatchingRun.Status != MatchingRunStatus.Running)
        {
            return Result.Fail(
                new ConflictError(
                    $"MatchingRun with status '{candidate.MatchingRun.Status}' cannot accept machine-generated changes."));
        }

        if (candidate.Status != SupplySolutionStatus.Generated)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplySolutionCandidate with status '{candidate.Status}' cannot be modified by the matching engine."));
        }

        if (candidate.SnapshotJson is not null
            || candidate.SnapshotHash is not null)
        {
            return Result.Fail(
                new ConflictError(
                    "Finalized SupplySolutionCandidate cannot be modified by the matching engine."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar en valfri UnitOfMeasure-referens.
    /// </summary>
    private async Task<Result> ValidateOptionalUnitAsync(int? unitOfMeasureId, CancellationToken cancellationToken)
    {
        if (!unitOfMeasureId.HasValue)
        {
            return Result.Ok();
        }

        var exists = await _db.Set<UnitOfMeasure>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == unitOfMeasureId.Value,
                cancellationToken);

        return exists
            ? Result.Ok()
            : Result.Fail(new NotFoundError(nameof(UnitOfMeasure), unitOfMeasureId.Value));
    }

    /// <summary>
    /// Canonicaliserar valfri JSON och ersätter tomma värden med null.
    /// </summary>
    private string? CanonicalizeOptionalJson(string? json)
    {
        return string.IsNullOrWhiteSpace(json)
            ? null
            : _canonicalJsonService.Canonicalize(json);
    }

    /// <summary>
    /// Läser candidate tillsammans med matching run för write-operationer.
    /// </summary>
    private Task<SupplySolutionCandidate?> LoadCandidateForWriteAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        return _db.Set<SupplySolutionCandidate>()
            .Include(x => x.MatchingRun)
            .SingleOrDefaultAsync(
                x => x.Id == candidateId,
                cancellationToken);
    }

    /// <summary>
    /// Läser requirement evaluation tillsammans med candidate och run.
    /// </summary>
    private Task<RequirementEvaluation?> LoadEvaluationForWriteAsync(Guid evaluationId, CancellationToken cancellationToken)
    {
        return _db.Set<RequirementEvaluation>()
            .Include(x => x.SupplySolutionCandidate)
                .ThenInclude(x => x.MatchingRun)
            .SingleOrDefaultAsync(
                x => x.Id == evaluationId,
                cancellationToken);
    }

    /// <summary>
    /// Läser score contribution tillsammans med candidate och run.
    /// </summary>
    private Task<ScoreContribution?> LoadScoreContributionForWriteAsync(Guid contributionId, CancellationToken cancellationToken)
    {
        return _db.Set<ScoreContribution>()
            .Include(x => x.SupplySolutionCandidate)
                .ThenInclude(x => x.MatchingRun)
            .SingleOrDefaultAsync(
                x => x.Id == contributionId,
                cancellationToken);
    }

    /// <summary>
    /// Markerar att kandidatens beräknade innehåll har förändrats.
    /// </summary>
    private static void TouchCandidate(SupplySolutionCandidate candidate)
    {
        candidate.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Markerar run-roten så att completion konkurrerar med pågående child-writes.
    /// </summary>
    private static void TouchRun(MatchingRun run)
    {
        run.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Sparar ändringar och översätter optimistic-concurrency-fel.
    /// </summary>
    private async Task<Result> SaveWithConcurrencyAsync(string fallbackEntityName, Guid fallbackEntityId, CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail(
                CreateConcurrencyError(
                    exception,
                    fallbackEntityName,
                    fallbackEntityId));
        }
    }

    /// <summary>
    /// Identifierar rätt entitet bakom ett EF concurrency-fel.
    /// </summary>
    private static ConcurrencyError CreateConcurrencyError(DbUpdateConcurrencyException exception, string fallbackEntityName, Guid fallbackEntityId)
    {
        foreach (var entry in exception.Entries)
        {
            switch (entry.Entity)
            {
                case MatchingRun run:
                    return new ConcurrencyError(nameof(MatchingRun), run.Id);

                case SupplySolutionCandidate candidate:
                    return new ConcurrencyError(nameof(SupplySolutionCandidate), candidate.Id);

                case RequirementEvaluation evaluation:
                    return new ConcurrencyError(nameof(RequirementEvaluation), evaluation.Id);

                case ScoreContribution contribution:
                    return new ConcurrencyError(nameof(ScoreContribution), contribution.Id);
            }
        }

        return new ConcurrencyError(
            fallbackEntityName,
            fallbackEntityId);
    }

    /// <summary>
    /// Projekterar candidate med dess aktuella resultatinnehåll.
    /// </summary>
    private Task<SupplySolutionCandidateResponse?> ProjectAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        return _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == candidateId)
            .Select(SupplySolutionCandidateProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar en sparad candidate och kastar om den oväntat saknas.
    /// </summary>
    private async Task<SupplySolutionCandidateResponse> ProjectRequiredAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        return await ProjectAsync(candidateId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"SupplySolutionCandidate '{candidateId}' was saved but could not be projected.");
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