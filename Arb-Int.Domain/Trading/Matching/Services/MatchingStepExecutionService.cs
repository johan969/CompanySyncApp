using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Sourcing;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Persisterar solution-step-grafen som genereras av matchningsmotorn.
/// </summary>
public sealed class MatchingStepExecutionService : IMatchingStepExecutionService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ICanonicalJsonService _canonicalJsonService;

    /// <summary>
    /// Skapar servicen med databas-, validerings- och JSON-beroenden.
    /// </summary>
    public MatchingStepExecutionService(Arb_IntDbContext db, IValidationService validationService, ICanonicalJsonService canonicalJsonService)
    {
        _db = db;
        _validationService = validationService;
        _canonicalJsonService = canonicalJsonService;
    }

    /// <summary>
    /// Skapar ett Proposed solution step efter att candidate och referenser verifierats.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> AddStepAsync(
        Guid supplySolutionCandidateId,
        CreateSupplySolutionStepRequest request,
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

        var referencesResult = await ValidateStepReferencesAsync(
            candidate.Id,
            request.SupplySolutionLineId,
            request.ProviderBusinessRelationshipId,
            request.SupplierProcessCapabilityId,
            request.ProcessTypeId,
            request.InputMaterialId,
            request.OutputMaterialId,
            request.InputQuantityUnitId,
            request.OutputQuantityUnitId,
            request.FromLocationId,
            request.ToLocationId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(referencesResult.Errors);
        }

        if (await SequenceNumberExistsAsync(candidate.Id, request.SequenceNumber, null, cancellationToken))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError($"SequenceNumber '{request.SequenceNumber}' already exists in the SupplySolutionCandidate."));
        }

        var step = new SupplySolutionStep
        {
            Id = Guid.CreateVersion7(),
            SupplySolutionCandidateId = candidate.Id,
            SupplySolutionLineId = request.SupplySolutionLineId,
            SequenceNumber = request.SequenceNumber,
            Type = request.Type,
            Status = SupplySolutionStepStatus.Proposed,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            ProviderBusinessRelationshipId = request.ProviderBusinessRelationshipId,
            SupplierProcessCapabilityId = request.SupplierProcessCapabilityId,
            ProcessTypeId = request.ProcessTypeId,
            InputMaterialId = request.InputMaterialId,
            OutputMaterialId = request.OutputMaterialId,
            ExpectedInputQuantity = request.ExpectedInputQuantity,
            InputQuantityUnitId = request.InputQuantityUnitId,
            ExpectedOutputQuantity = request.ExpectedOutputQuantity,
            OutputQuantityUnitId = request.OutputQuantityUnitId,
            ExpectedYieldPercent = request.ExpectedYieldPercent,
            EstimatedLeadTimeDays = request.EstimatedLeadTimeDays,
            FromLocationId = request.FromLocationId,
            ToLocationId = request.ToLocationId,
            PlannedStartUtc = request.PlannedStartUtc,
            PlannedEndUtc = request.PlannedEndUtc,
            ParametersJson = CanonicalizeOptionalJson(request.ParametersJson)
        };

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<SupplySolutionStep>().Add(step);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                CreateConcurrencyError(exception, nameof(SupplySolutionCandidate), candidate.Id));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError($"SequenceNumber '{request.SequenceNumber}' already exists in the SupplySolutionCandidate."));
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar ett Proposed step utan att flytta det mellan solution lines.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> UpdateStepAsync(
        Guid supplySolutionStepId,
        UpdateSupplySolutionStepRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var step = await LoadStepForWriteAsync(supplySolutionStepId, cancellationToken);

        if (step is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionStep), supplySolutionStepId));
        }

        var candidate = step.SupplySolutionCandidate;

        var writableResult = ValidateStepIsWritable(step);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var referencesResult = await ValidateStepReferencesAsync(
            candidate.Id,
            step.SupplySolutionLineId,
            request.ProviderBusinessRelationshipId,
            request.SupplierProcessCapabilityId,
            request.ProcessTypeId,
            request.InputMaterialId,
            request.OutputMaterialId,
            request.InputQuantityUnitId,
            request.OutputQuantityUnitId,
            request.FromLocationId,
            request.ToLocationId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(referencesResult.Errors);
        }

        if (await SequenceNumberExistsAsync(candidate.Id, request.SequenceNumber, step.Id, cancellationToken))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError($"SequenceNumber '{request.SequenceNumber}' already exists in the SupplySolutionCandidate."));
        }

        step.SequenceNumber = request.SequenceNumber;
        step.Type = request.Type;
        step.Name = request.Name.Trim();
        step.Description = request.Description.Trim();
        step.ProviderBusinessRelationshipId = request.ProviderBusinessRelationshipId;
        step.SupplierProcessCapabilityId = request.SupplierProcessCapabilityId;
        step.ProcessTypeId = request.ProcessTypeId;
        step.InputMaterialId = request.InputMaterialId;
        step.OutputMaterialId = request.OutputMaterialId;
        step.ExpectedInputQuantity = request.ExpectedInputQuantity;
        step.InputQuantityUnitId = request.InputQuantityUnitId;
        step.ExpectedOutputQuantity = request.ExpectedOutputQuantity;
        step.OutputQuantityUnitId = request.OutputQuantityUnitId;
        step.ExpectedYieldPercent = request.ExpectedYieldPercent;
        step.EstimatedLeadTimeDays = request.EstimatedLeadTimeDays;
        step.FromLocationId = request.FromLocationId;
        step.ToLocationId = request.ToLocationId;
        step.PlannedStartUtc = request.PlannedStartUtc;
        step.PlannedEndUtc = request.PlannedEndUtc;
        step.ParametersJson = CanonicalizeOptionalJson(request.ParametersJson);

        _db.Entry(step).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                CreateConcurrencyError(exception, nameof(SupplySolutionStep), step.Id));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError($"SequenceNumber '{request.SequenceNumber}' already exists in the SupplySolutionCandidate."));
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Tar bort ett Proposed step om inga dependencies eller evaluations fortfarande refererar till det.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> DeleteStepAsync(
        Guid supplySolutionStepId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var step = await LoadStepForWriteAsync(supplySolutionStepId, cancellationToken);

        if (step is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionStep), supplySolutionStepId));
        }

        var candidate = step.SupplySolutionCandidate;

        var writableResult = ValidateStepIsWritable(step);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var dependencyExists = await _db.Set<SupplySolutionStepDependency>()
            .AsNoTracking()
            .AnyAsync(
                x => x.PredecessorStepId == step.Id
                    || x.SuccessorStepId == step.Id,
                cancellationToken);

        if (dependencyExists)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError("SupplySolutionStep cannot be deleted while dependencies reference it."));
        }

        var evaluationExists = await _db.Set<RequirementEvaluation>()
            .AsNoTracking()
            .AnyAsync(
                x => x.SupplySolutionStepId == step.Id,
                cancellationToken);

        if (evaluationExists)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError("SupplySolutionStep cannot be deleted while RequirementEvaluations reference it."));
        }

        _db.Entry(step).Property(x => x.RowVersion).OriginalValue = rowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<SupplySolutionStep>().Remove(step);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(SupplySolutionStep),
            step.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Skapar en dependency efter att candidate-scope, duplicate edge och cykler verifierats.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> AddDependencyAsync(
        Guid supplySolutionCandidateId,
        CreateSupplySolutionStepDependencyRequest request,
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

        var stepIds = await _db.Set<SupplySolutionStep>()
            .AsNoTracking()
            .Where(x =>
                x.SupplySolutionCandidateId == candidate.Id
                && (x.Id == request.PredecessorStepId || x.Id == request.SuccessorStepId))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (stepIds.Count != 2)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError("Both dependency steps must belong to the selected SupplySolutionCandidate."));
        }

        var duplicateExists = await _db.Set<SupplySolutionStepDependency>()
            .AsNoTracking()
            .AnyAsync(
                x => x.PredecessorStepId == request.PredecessorStepId
                    && x.SuccessorStepId == request.SuccessorStepId,
                cancellationToken);

        if (duplicateExists)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError("The dependency already exists."));
        }

        var createsCycle = await WouldCreateCycleAsync(
            candidate.Id,
            request.PredecessorStepId,
            request.SuccessorStepId,
            cancellationToken);

        if (createsCycle)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError("The dependency would create a cycle in the SupplySolutionStep graph."));
        }

        var dependency = new SupplySolutionStepDependency
        {
            Id = Guid.CreateVersion7(),
            SupplySolutionCandidateId = candidate.Id,
            PredecessorStepId = request.PredecessorStepId,
            SuccessorStepId = request.SuccessorStepId,
            Notes = TrimToNull(request.Notes)
        };

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<SupplySolutionStepDependency>().Add(dependency);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                CreateConcurrencyError(exception, nameof(SupplySolutionCandidate), candidate.Id));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new ConflictError("The dependency already exists."));
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar endast dependency-metadata eftersom edge-endpoints är immutable.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> UpdateDependencyAsync(
        Guid supplySolutionStepDependencyId,
        UpdateSupplySolutionStepDependencyRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var dependency = await _db.Set<SupplySolutionStepDependency>()
            .SingleOrDefaultAsync(
                x => x.Id == supplySolutionStepDependencyId,
                cancellationToken);

        if (dependency is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionStepDependency), supplySolutionStepDependencyId));
        }

        var candidate = await LoadCandidateForWriteAsync(
            dependency.SupplySolutionCandidateId,
            cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), dependency.SupplySolutionCandidateId));
        }

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        dependency.Notes = TrimToNull(request.Notes);

        _db.Entry(dependency).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(SupplySolutionStepDependency),
            dependency.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Tar bort en dependency från kandidatens step-graf.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> DeleteDependencyAsync(
        Guid supplySolutionStepDependencyId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var dependency = await _db.Set<SupplySolutionStepDependency>()
            .SingleOrDefaultAsync(
                x => x.Id == supplySolutionStepDependencyId,
                cancellationToken);

        if (dependency is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionStepDependency), supplySolutionStepDependencyId));
        }

        var candidate = await LoadCandidateForWriteAsync(
            dependency.SupplySolutionCandidateId,
            cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(
                new NotFoundError(nameof(SupplySolutionCandidate), dependency.SupplySolutionCandidateId));
        }

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        _db.Entry(dependency).Property(x => x.RowVersion).OriginalValue = rowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<SupplySolutionStepDependency>().Remove(dependency);

        var saveResult = await SaveWithConcurrencyAsync(
            nameof(SupplySolutionStepDependency),
            dependency.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Kontrollerar om en ny predecessor-to-successor-edge skulle skapa en cykel.
    /// <para>
    /// En cykel uppstår om successor redan kan nå predecessor via befintliga edges.<br />
    /// Exempel: A → B → C gör att C → A måste nekas.
    /// </para>
    /// </summary>
    private async Task<bool> WouldCreateCycleAsync(
        Guid supplySolutionCandidateId,
        Guid predecessorStepId,
        Guid successorStepId,
        CancellationToken cancellationToken)
    {
        var edges = await _db.Set<SupplySolutionStepDependency>()
            .AsNoTracking()
            .Where(x => x.SupplySolutionCandidateId == supplySolutionCandidateId)
            .Select(x => new
            {
                x.PredecessorStepId,
                x.SuccessorStepId
            })
            .ToListAsync(cancellationToken);

        var adjacency = edges
            .GroupBy(x => x.PredecessorStepId)
            .ToDictionary(
                x => x.Key,
                x => x.Select(y => y.SuccessorStepId).ToArray());

        return HasPath(
            successorStepId,
            predecessorStepId,
            adjacency);
    }

    /// <summary>
    /// Söker efter en riktad väg mellan två steps med depth-first search.
    /// </summary>
    private static bool HasPath(
        Guid startStepId,
        Guid targetStepId,
        IReadOnlyDictionary<Guid, Guid[]> adjacency)
    {
        var visited = new HashSet<Guid>();
        var stack = new Stack<Guid>();

        stack.Push(startStepId);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (current == targetStepId)
            {
                return true;
            }

            if (!visited.Add(current))
            {
                continue;
            }

            if (!adjacency.TryGetValue(current, out var successors))
            {
                continue;
            }

            foreach (var successor in successors)
            {
                stack.Push(successor);
            }
        }

        return false;
    }

    /// <summary>
    /// Kontrollerar alla optional references som används av ett solution step.
    /// </summary>
    private async Task<Result> ValidateStepReferencesAsync(
        Guid supplySolutionCandidateId,
        Guid? supplySolutionLineId,
        Guid? providerBusinessRelationshipId,
        Guid? supplierProcessCapabilityId,
        int? processTypeId,
        Guid? inputMaterialId,
        Guid? outputMaterialId,
        int? inputQuantityUnitId,
        int? outputQuantityUnitId,
        Guid? fromLocationId,
        Guid? toLocationId,
        CancellationToken cancellationToken)
    {
        if (supplySolutionLineId.HasValue)
        {
            var lineExists = await _db.Set<SupplySolutionLine>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == supplySolutionLineId.Value
                        && x.SupplySolutionCandidateId == supplySolutionCandidateId,
                    cancellationToken);

            if (!lineExists)
            {
                return Result.Fail(
                    new ConflictError("SupplySolutionLine does not belong to the selected SupplySolutionCandidate."));
            }
        }

        if (providerBusinessRelationshipId.HasValue)
        {
            var providerExists = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == providerBusinessRelationshipId.Value,
                    cancellationToken);

            if (!providerExists)
            {
                return Result.Fail(
                    new NotFoundError(nameof(BusinessRelationship), providerBusinessRelationshipId.Value));
            }
        }

        if (supplierProcessCapabilityId.HasValue)
        {
            if (!providerBusinessRelationshipId.HasValue)
            {
                return Result.Fail(
                    new ConflictError("ProviderBusinessRelationshipId is required when SupplierProcessCapabilityId is supplied."));
            }

            var capabilityExists = await _db.Set<SupplierProcessCapability>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == supplierProcessCapabilityId.Value
                        && x.BusinessRelationshipId == providerBusinessRelationshipId.Value,
                    cancellationToken);

            if (!capabilityExists)
            {
                return Result.Fail(
                    new ConflictError("SupplierProcessCapability does not belong to the selected provider relationship."));
            }
        }

        if (processTypeId.HasValue)
        {
            var processTypeExists = await _db.Set<ProcessType>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == processTypeId.Value, cancellationToken);

            if (!processTypeExists)
            {
                return Result.Fail(new NotFoundError(nameof(ProcessType), processTypeId.Value));
            }
        }

        if (inputMaterialId.HasValue)
        {
            var inputMaterialExists = await _db.Set<Material>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == inputMaterialId.Value, cancellationToken);

            if (!inputMaterialExists)
            {
                return Result.Fail(new NotFoundError(nameof(Material), inputMaterialId.Value));
            }
        }

        if (outputMaterialId.HasValue)
        {
            var outputMaterialExists = await _db.Set<Material>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == outputMaterialId.Value, cancellationToken);

            if (!outputMaterialExists)
            {
                return Result.Fail(new NotFoundError(nameof(Material), outputMaterialId.Value));
            }
        }

        if (inputQuantityUnitId.HasValue)
        {
            var inputUnitExists = await _db.Set<UnitOfMeasure>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == inputQuantityUnitId.Value, cancellationToken);

            if (!inputUnitExists)
            {
                return Result.Fail(new NotFoundError(nameof(UnitOfMeasure), inputQuantityUnitId.Value));
            }
        }

        if (outputQuantityUnitId.HasValue)
        {
            var outputUnitExists = await _db.Set<UnitOfMeasure>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == outputQuantityUnitId.Value, cancellationToken);

            if (!outputUnitExists)
            {
                return Result.Fail(new NotFoundError(nameof(UnitOfMeasure), outputQuantityUnitId.Value));
            }
        }

        if (fromLocationId.HasValue)
        {
            var fromLocationExists = await _db.Set<GeographicLocation>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == fromLocationId.Value, cancellationToken);

            if (!fromLocationExists)
            {
                return Result.Fail(new NotFoundError(nameof(GeographicLocation), fromLocationId.Value));
            }
        }

        if (toLocationId.HasValue)
        {
            var toLocationExists = await _db.Set<GeographicLocation>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == toLocationId.Value, cancellationToken);

            if (!toLocationExists)
            {
                return Result.Fail(new NotFoundError(nameof(GeographicLocation), toLocationId.Value));
            }
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar om ett sekvensnummer redan används av ett annat step i kandidaten.
    /// </summary>
    private Task<bool> SequenceNumberExistsAsync(
        Guid supplySolutionCandidateId,
        int sequenceNumber,
        Guid? excludeStepId,
        CancellationToken cancellationToken)
    {
        IQueryable<SupplySolutionStep> query = _db.Set<SupplySolutionStep>()
            .AsNoTracking()
            .Where(x =>
                x.SupplySolutionCandidateId == supplySolutionCandidateId
                && x.SequenceNumber == sequenceNumber);

        if (excludeStepId.HasValue)
        {
            query = query.Where(x => x.Id != excludeStepId.Value);
        }

        return query.AnyAsync(cancellationToken);
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
    /// Kontrollerar både candidate-livscykeln och stegets egen status.
    /// </summary>
    private static Result ValidateStepIsWritable(SupplySolutionStep step)
    {
        var candidateResult = ValidateCandidateIsWritable(step.SupplySolutionCandidate);

        if (candidateResult.IsFailed)
        {
            return candidateResult;
        }

        if (step.Status != SupplySolutionStepStatus.Proposed)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplySolutionStep with status '{step.Status}' cannot be modified by the matching engine."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Läser candidate tillsammans med matching run.
    /// </summary>
    private Task<SupplySolutionCandidate?> LoadCandidateForWriteAsync(
        Guid candidateId,
        CancellationToken cancellationToken)
    {
        return _db.Set<SupplySolutionCandidate>()
            .Include(x => x.MatchingRun)
            .SingleOrDefaultAsync(
                x => x.Id == candidateId,
                cancellationToken);
    }

    /// <summary>
    /// Läser step tillsammans med candidate och matching run.
    /// </summary>
    private Task<SupplySolutionStep?> LoadStepForWriteAsync(
        Guid stepId,
        CancellationToken cancellationToken)
    {
        return _db.Set<SupplySolutionStep>()
            .Include(x => x.SupplySolutionCandidate)
                .ThenInclude(x => x.MatchingRun)
            .SingleOrDefaultAsync(
                x => x.Id == stepId,
                cancellationToken);
    }

    /// <summary>
    /// Markerar att kandidatens genererade innehåll har förändrats.
    /// </summary>
    private static void TouchCandidate(SupplySolutionCandidate candidate)
    {
        candidate.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Markerar run-roten så att completion konkurrerar med pågående grafändringar.
    /// </summary>
    private static void TouchRun(MatchingRun run)
    {
        run.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Canonicaliserar valfri JSON och ersätter whitespace med null.
    /// </summary>
    private string? CanonicalizeOptionalJson(string? json)
    {
        return string.IsNullOrWhiteSpace(json)
            ? null
            : _canonicalJsonService.Canonicalize(json);
    }

    /// <summary>
    /// Sparar och översätter optimistic-concurrency-fel.
    /// </summary>
    private async Task<Result> SaveWithConcurrencyAsync(
        string fallbackEntityName,
        Guid fallbackEntityId,
        CancellationToken cancellationToken)
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
    /// Identifierar rätt domänentitet bakom ett concurrency-fel.
    /// </summary>
    private static ConcurrencyError CreateConcurrencyError(
        DbUpdateConcurrencyException exception,
        string fallbackEntityName,
        Guid fallbackEntityId)
    {
        foreach (var entry in exception.Entries)
        {
            switch (entry.Entity)
            {
                case MatchingRun run:
                    return new ConcurrencyError(nameof(MatchingRun), run.Id);

                case SupplySolutionCandidate candidate:
                    return new ConcurrencyError(nameof(SupplySolutionCandidate), candidate.Id);

                case SupplySolutionStep step:
                    return new ConcurrencyError(nameof(SupplySolutionStep), step.Id);

                case SupplySolutionStepDependency dependency:
                    return new ConcurrencyError(nameof(SupplySolutionStepDependency), dependency.Id);
            }
        }

        return new ConcurrencyError(fallbackEntityName, fallbackEntityId);
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
    /// Projekterar kandidatens kompletta resultatgraf.
    /// </summary>
    private Task<SupplySolutionCandidateResponse?> ProjectAsync(
        Guid candidateId,
        CancellationToken cancellationToken)
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
    private async Task<SupplySolutionCandidateResponse> ProjectRequiredAsync(
        Guid candidateId,
        CancellationToken cancellationToken)
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