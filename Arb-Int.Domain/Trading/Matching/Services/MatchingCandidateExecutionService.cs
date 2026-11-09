using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Persisterar candidate-resultat från matchningsmotorn och skyddar run-aggregatet mot samtidiga ändringar.
/// </summary>
public sealed class MatchingCandidateExecutionService : IMatchingCandidateExecutionService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;

    /// <summary>
    /// Skapar execution-servicen med databas- och valideringsberoenden.
    /// </summary>
    public MatchingCandidateExecutionService(Arb_IntDbContext db, IValidationService validationService)
    {
        _db = db;
        _validationService = validationService;
    }

    /// <summary>
    /// Skapar en candidate med initialt Incomplete outcome och Generated status.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> CreateCandidateAsync(Guid matchingRunId, CreateSupplySolutionCandidateRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var run = await _db.Set<MatchingRun>().SingleOrDefaultAsync(x => x.Id == matchingRunId, cancellationToken);

        if (run is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(MatchingRun), matchingRunId));
        }

        var writableResult = ValidateRunIsWritable(run);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        if (await _db.Set<SupplySolutionCandidate>().AsNoTracking().AnyAsync(x => x.MatchingRunId == run.Id && x.CandidateNumber == request.CandidateNumber, cancellationToken))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError($"Candidate number '{request.CandidateNumber}' already exists in the MatchingRun."));
        }

        var currencyResult = await ValidateOptionalCurrencyAsync(request.EstimatedCostCurrencyId, cancellationToken);

        if (currencyResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(currencyResult.Errors);
        }

        var candidate = new SupplySolutionCandidate
        {
            Id = Guid.CreateVersion7(),
            MatchingRunId = run.Id,
            CandidateNumber = request.CandidateNumber,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Kind = request.Kind,
            Outcome = SupplySolutionOutcome.Incomplete,
            Status = SupplySolutionStatus.Generated,
            VerificationState = request.VerificationState,
            TotalScore = 0m,
            CoveragePercent = request.CoveragePercent,
            EstimatedPurchaseCost = request.EstimatedPurchaseCost,
            EstimatedLogisticsCost = request.EstimatedLogisticsCost,
            EstimatedProcessingCost = request.EstimatedProcessingCost,
            EstimatedTotalCost = request.EstimatedTotalCost,
            EstimatedCostCurrencyId = request.EstimatedCostCurrencyId,
            EstimatedLeadTimeDays = request.EstimatedLeadTimeDays,
            ContainsHardFail = false,
            SnapshotJson = null,
            SnapshotHash = null,
            ExpiresAtUtc = request.ExpiresAtUtc
        };

        TouchRun(run);

        _db.Set<SupplySolutionCandidate>().Add(candidate);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(CreateConcurrencyError(exception, nameof(MatchingRun), run.Id));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError($"Candidate number '{request.CandidateNumber}' already exists in the MatchingRun."));
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar candidate-metadata medan kandidaten fortfarande genereras.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> UpdateCandidateAsync(Guid supplySolutionCandidateId, UpdateSupplySolutionCandidateRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var candidate = await LoadCandidateForWriteAsync(supplySolutionCandidateId, cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var currencyResult = await ValidateOptionalCurrencyAsync(request.EstimatedCostCurrencyId, cancellationToken);

        if (currencyResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(currencyResult.Errors);
        }

        candidate.Name = request.Name.Trim();
        candidate.Description = request.Description.Trim();
        candidate.Kind = request.Kind;
        candidate.VerificationState = request.VerificationState;
        candidate.CoveragePercent = request.CoveragePercent;
        candidate.EstimatedPurchaseCost = request.EstimatedPurchaseCost;
        candidate.EstimatedLogisticsCost = request.EstimatedLogisticsCost;
        candidate.EstimatedProcessingCost = request.EstimatedProcessingCost;
        candidate.EstimatedTotalCost = request.EstimatedTotalCost;
        candidate.EstimatedCostCurrencyId = request.EstimatedCostCurrencyId;
        candidate.EstimatedLeadTimeDays = request.EstimatedLeadTimeDays;
        candidate.ExpiresAtUtc = request.ExpiresAtUtc;

        _db.Entry(candidate).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        TouchRun(candidate.MatchingRun);

        var saveResult = await SaveWithConcurrencyAsync(nameof(SupplySolutionCandidate), candidate.Id, cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Tar bort en generated candidate inklusive dess cascade-ägda resultat.
    /// </summary>
    public async Task<Result> DeleteCandidateAsync(Guid supplySolutionCandidateId, uint rowVersion, CancellationToken cancellationToken = default)
    {
        var candidate = await LoadCandidateForWriteAsync(supplySolutionCandidateId, cancellationToken);

        if (candidate is null)
        {
            return Result.Fail(new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return writableResult;
        }

        _db.Entry(candidate).Property(x => x.RowVersion).OriginalValue = rowVersion;

        TouchRun(candidate.MatchingRun);

        var dependencies = await _db.Set<SupplySolutionStepDependency>()
            .Where(x => x.SupplySolutionCandidateId == candidate.Id)
            .ToListAsync(cancellationToken);

        _db.Set<SupplySolutionStepDependency>().RemoveRange(dependencies);

        _db.Set<SupplySolutionCandidate>().Remove(candidate);

        return await SaveWithConcurrencyAsync(nameof(SupplySolutionCandidate), candidate.Id, cancellationToken);
    }

    /// <summary>
    /// Skapar en solution line från MatchingRunLine och härleder alla quantity-resultat.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> AddLineAsync(Guid supplySolutionCandidateId, CreateSupplySolutionLineRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var candidate = await LoadCandidateForWriteAsync(supplySolutionCandidateId, cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId));
        }

        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var runLine = await _db.Set<MatchingRunLine>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.MatchingRunLineId && x.MatchingRunId == candidate.MatchingRunId, cancellationToken);

        if (runLine is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError("MatchingRunLine does not belong to the candidate's MatchingRun."));
        }

        if (request.CoveredQuantity > runLine.RequiredQuantity)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError("CoveredQuantity cannot exceed MatchingRunLine.RequiredQuantity."));
        }

        if (await _db.Set<SupplySolutionLine>().AsNoTracking().AnyAsync(x => x.SupplySolutionCandidateId == candidate.Id && x.MatchingRunLineId == runLine.Id, cancellationToken))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError("The MatchingRunLine already has a SupplySolutionLine in this candidate."));
        }

        var line = new SupplySolutionLine
        {
            Id = Guid.CreateVersion7(),
            SupplySolutionCandidateId = candidate.Id,
            MatchingRunId = candidate.MatchingRunId,
            MatchingRunLineId = runLine.Id,
            RequiredQuantity = runLine.RequiredQuantity,
            QuantityUnitId = runLine.QuantityUnitId,
            Notes = TrimToNull(request.Notes)
        };

        ApplyCoverage(line, request.CoveredQuantity);

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<SupplySolutionLine>().Add(line);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(CreateConcurrencyError(exception, nameof(SupplySolutionCandidate), candidate.Id));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError("The MatchingRunLine already has a SupplySolutionLine in this candidate."));
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar coverage och härleder shortage, procent och fully-covered-flagga.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> UpdateLineAsync(Guid supplySolutionLineId, UpdateSupplySolutionLineRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var line = await LoadLineForWriteAsync(supplySolutionLineId, cancellationToken);

        if (line is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(SupplySolutionLine), supplySolutionLineId));
        }

        var writableResult = ValidateCandidateIsWritable(line.SupplySolutionCandidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        if (request.CoveredQuantity > line.RequiredQuantity)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError("CoveredQuantity cannot exceed RequiredQuantity."));
        }

        ApplyCoverage(line, request.CoveredQuantity);
        line.Notes = TrimToNull(request.Notes);

        _db.Entry(line).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        TouchCandidate(line.SupplySolutionCandidate);
        TouchRun(line.SupplySolutionCandidate.MatchingRun);

        var saveResult = await SaveWithConcurrencyAsync(nameof(SupplySolutionLine), line.Id, cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(line.SupplySolutionCandidateId, cancellationToken));
    }

    /// <summary>
    /// Tar bort en solution line och dess cascade-ägda allocations och capacity evaluations.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> DeleteLineAsync(Guid supplySolutionLineId, uint rowVersion, CancellationToken cancellationToken = default)
    {
        var line = await LoadLineForWriteAsync(supplySolutionLineId, cancellationToken);

        if (line is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(SupplySolutionLine), supplySolutionLineId));
        }

        var writableResult = ValidateCandidateIsWritable(line.SupplySolutionCandidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var candidateId = line.SupplySolutionCandidateId;

        _db.Entry(line).Property(x => x.RowVersion).OriginalValue = rowVersion;

        TouchCandidate(line.SupplySolutionCandidate);
        TouchRun(line.SupplySolutionCandidate.MatchingRun);

        _db.Set<SupplySolutionLine>().Remove(line);

        var saveResult = await SaveWithConcurrencyAsync(nameof(SupplySolutionLine), line.Id, cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(candidateId, cancellationToken));
    }

    /// <summary>
    /// Skapar en supplier-allokering efter att supplier-, offering- och availability-kedjan verifierats.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> AddAllocationAsync(Guid supplySolutionLineId, CreateSupplyAllocationRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var line = await LoadLineForWriteAsync(supplySolutionLineId, cancellationToken);

        if (line is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(SupplySolutionLine), supplySolutionLineId));
        }

        var writableResult = ValidateCandidateIsWritable(line.SupplySolutionCandidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var referencesResult = await ValidateAllocationReferencesAsync(
            request.SupplierBusinessRelationshipId,
            request.SupplierMaterialOfferingId,
            request.SupplierOfferingAvailabilityId,
            request.InventoryLotId,
            request.SupplierQuoteLineId,
            request.QuantityUnitId,
            request.UnitPurchasePrice,
            request.PurchasePriceCurrencyId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(referencesResult.Errors);
        }

        if (await AllocationSequenceExistsAsync(line.Id, request.AllocationSequence, null, cancellationToken))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError($"AllocationSequence '{request.AllocationSequence}' already exists on the SupplySolutionLine."));
        }

        var allocation = new SupplyAllocation
        {
            Id = Guid.CreateVersion7(),
            SupplySolutionLineId = line.Id,
            SupplierBusinessRelationshipId = request.SupplierBusinessRelationshipId,
            SupplierMaterialOfferingId = request.SupplierMaterialOfferingId,
            SupplierOfferingAvailabilityId = request.SupplierOfferingAvailabilityId,
            InventoryLotId = request.InventoryLotId,
            SupplierQuoteLineId = request.SupplierQuoteLineId,
            Status = request.Status,
            AllocationSequence = request.AllocationSequence,
            AllocatedQuantity = request.AllocatedQuantity,
            QuantityUnitId = request.QuantityUnitId,
            UnitPurchasePrice = request.UnitPurchasePrice,
            PurchasePriceCurrencyId = request.PurchasePriceCurrencyId,
            EstimatedAdditionalCost = request.EstimatedAdditionalCost,
            AvailabilityVerificationState = request.AvailabilityVerificationState,
            QualityVerificationState = request.QualityVerificationState,
            Notes = TrimToNull(request.Notes)
        };

        TouchCandidate(line.SupplySolutionCandidate);
        TouchRun(line.SupplySolutionCandidate.MatchingRun);

        _db.Set<SupplyAllocation>().Add(allocation);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(CreateConcurrencyError(exception, nameof(SupplySolutionCandidate), line.SupplySolutionCandidateId));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError($"AllocationSequence '{request.AllocationSequence}' already exists on the SupplySolutionLine."));
        }

        return Result.Ok(await ProjectRequiredAsync(line.SupplySolutionCandidateId, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar en allocation utan att byta supplier eller offering.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> UpdateAllocationAsync(Guid supplyAllocationId, UpdateSupplyAllocationRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var allocation = await LoadAllocationForWriteAsync(supplyAllocationId, cancellationToken);

        if (allocation is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(SupplyAllocation), supplyAllocationId));
        }

        var candidate = allocation.SupplySolutionLine.SupplySolutionCandidate;
        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var referencesResult = await ValidateAllocationReferencesAsync(
            allocation.SupplierBusinessRelationshipId,
            allocation.SupplierMaterialOfferingId,
            request.SupplierOfferingAvailabilityId,
            request.InventoryLotId,
            request.SupplierQuoteLineId,
            request.QuantityUnitId,
            request.UnitPurchasePrice,
            request.PurchasePriceCurrencyId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(referencesResult.Errors);
        }

        if (await AllocationSequenceExistsAsync(allocation.SupplySolutionLineId, request.AllocationSequence, allocation.Id, cancellationToken))
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError($"AllocationSequence '{request.AllocationSequence}' already exists on the SupplySolutionLine."));
        }

        allocation.SupplierOfferingAvailabilityId = request.SupplierOfferingAvailabilityId;
        allocation.InventoryLotId = request.InventoryLotId;
        allocation.SupplierQuoteLineId = request.SupplierQuoteLineId;
        allocation.Status = request.Status;
        allocation.AllocationSequence = request.AllocationSequence;
        allocation.AllocatedQuantity = request.AllocatedQuantity;
        allocation.QuantityUnitId = request.QuantityUnitId;
        allocation.UnitPurchasePrice = request.UnitPurchasePrice;
        allocation.PurchasePriceCurrencyId = request.PurchasePriceCurrencyId;
        allocation.EstimatedAdditionalCost = request.EstimatedAdditionalCost;
        allocation.AvailabilityVerificationState = request.AvailabilityVerificationState;
        allocation.QualityVerificationState = request.QualityVerificationState;
        allocation.Notes = TrimToNull(request.Notes);

        _db.Entry(allocation).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        var saveResult = await SaveWithConcurrencyAsync(nameof(SupplyAllocation), allocation.Id, cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Tar bort en supplier-allokering från en generated candidate.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> DeleteAllocationAsync(Guid supplyAllocationId, uint rowVersion, CancellationToken cancellationToken = default)
    {
        var allocation = await LoadAllocationForWriteAsync(supplyAllocationId, cancellationToken);

        if (allocation is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(SupplyAllocation), supplyAllocationId));
        }

        var candidate = allocation.SupplySolutionLine.SupplySolutionCandidate;
        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        _db.Entry(allocation).Property(x => x.RowVersion).OriginalValue = rowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<SupplyAllocation>().Remove(allocation);

        var saveResult = await SaveWithConcurrencyAsync(nameof(SupplyAllocation), allocation.Id, cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Skapar en capacity evaluation och säkerställer att eventuell allocation ligger på samma solution line.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> AddCapacityEvaluationAsync(Guid supplySolutionLineId, CreateCapacityEvaluationRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var line = await LoadLineForWriteAsync(supplySolutionLineId, cancellationToken);

        if (line is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(SupplySolutionLine), supplySolutionLineId));
        }

        var writableResult = ValidateCandidateIsWritable(line.SupplySolutionCandidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        if (request.SupplyAllocationId.HasValue)
        {
            var allocationExists = await _db.Set<SupplyAllocation>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.SupplyAllocationId.Value && x.SupplySolutionLineId == line.Id, cancellationToken);

            if (!allocationExists)
            {
                return Result.Fail<SupplySolutionCandidateResponse>(new ConflictError("SupplyAllocation does not belong to the selected SupplySolutionLine."));
            }
        }

        var unitResult = await ValidateUnitAsync(request.QuantityUnitId, cancellationToken);

        if (unitResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(unitResult.Errors);
        }

        var evaluation = new CapacityEvaluation
        {
            Id = Guid.CreateVersion7(),
            SupplySolutionLineId = line.Id,
            SupplyAllocationId = request.SupplyAllocationId,
            Outcome = request.Outcome,
            RequestedQuantity = request.RequestedQuantity,
            AvailableQuantity = request.AvailableQuantity,
            AllocatedQuantity = request.AllocatedQuantity,
            QuantityUnitId = request.QuantityUnitId,
            AvailabilityFromUtc = request.AvailabilityFromUtc,
            AvailabilityToUtc = request.AvailabilityToUtc,
            Explanation = TrimToNull(request.Explanation)
        };

        TouchCandidate(line.SupplySolutionCandidate);
        TouchRun(line.SupplySolutionCandidate.MatchingRun);

        _db.Set<CapacityEvaluation>().Add(evaluation);

        var saveResult = await SaveWithConcurrencyAsync(nameof(CapacityEvaluation), evaluation.Id, cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(line.SupplySolutionCandidateId, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar ett kapacitetsresultat utan att flytta det mellan line eller allocation.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> UpdateCapacityEvaluationAsync(Guid capacityEvaluationId, UpdateCapacityEvaluationRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(validation.Errors);
        }

        var evaluation = await LoadCapacityEvaluationForWriteAsync(capacityEvaluationId, cancellationToken);

        if (evaluation is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(CapacityEvaluation), capacityEvaluationId));
        }

        var candidate = evaluation.SupplySolutionLine.SupplySolutionCandidate;
        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        var unitResult = await ValidateUnitAsync(request.QuantityUnitId, cancellationToken);

        if (unitResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(unitResult.Errors);
        }

        evaluation.Outcome = request.Outcome;
        evaluation.RequestedQuantity = request.RequestedQuantity;
        evaluation.AvailableQuantity = request.AvailableQuantity;
        evaluation.AllocatedQuantity = request.AllocatedQuantity;
        evaluation.QuantityUnitId = request.QuantityUnitId;
        evaluation.AvailabilityFromUtc = request.AvailabilityFromUtc;
        evaluation.AvailabilityToUtc = request.AvailabilityToUtc;
        evaluation.Explanation = TrimToNull(request.Explanation);

        _db.Entry(evaluation).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        var saveResult = await SaveWithConcurrencyAsync(nameof(CapacityEvaluation), evaluation.Id, cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Tar bort en capacity evaluation under pågående matching.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> DeleteCapacityEvaluationAsync(Guid capacityEvaluationId, uint rowVersion, CancellationToken cancellationToken = default)
    {
        var evaluation = await LoadCapacityEvaluationForWriteAsync(capacityEvaluationId, cancellationToken);

        if (evaluation is null)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(CapacityEvaluation), capacityEvaluationId));
        }

        var candidate = evaluation.SupplySolutionLine.SupplySolutionCandidate;
        var writableResult = ValidateCandidateIsWritable(candidate);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(writableResult.Errors);
        }

        _db.Entry(evaluation).Property(x => x.RowVersion).OriginalValue = rowVersion;

        TouchCandidate(candidate);
        TouchRun(candidate.MatchingRun);

        _db.Set<CapacityEvaluation>().Remove(evaluation);

        var saveResult = await SaveWithConcurrencyAsync(nameof(CapacityEvaluation), evaluation.Id, cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<SupplySolutionCandidateResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(candidate.Id, cancellationToken));
    }

    /// <summary>
    /// Kontrollerar att en run fortfarande accepterar maskinella resultat.
    /// </summary>
    private static Result ValidateRunIsWritable(MatchingRun run)
    {
        return run.Status == MatchingRunStatus.Running
            ? Result.Ok()
            : Result.Fail(new ConflictError($"MatchingRun with status '{run.Status}' cannot be modified."));
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
    /// Härleder alla quantity-resultat från required och covered quantity.
    /// </summary>
    private static void ApplyCoverage(SupplySolutionLine line, decimal coveredQuantity)
    {
        var shortageQuantity = line.RequiredQuantity - coveredQuantity;

        line.CoveredQuantity = coveredQuantity;
        line.ShortageQuantity = shortageQuantity;
        line.IsFullyCovered = shortageQuantity == 0m;
        line.CoveragePercent = decimal.Round((coveredQuantity / line.RequiredQuantity) * 100m, 4, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Kontrollerar alla externa referenser som används av en supplier-allokering.
    /// </summary>
    private async Task<Result> ValidateAllocationReferencesAsync(
        Guid supplierBusinessRelationshipId,
        Guid supplierMaterialOfferingId,
        Guid? supplierOfferingAvailabilityId,
        Guid? inventoryLotId,
        Guid? supplierQuoteLineId,
        int quantityUnitId,
        decimal? unitPurchasePrice,
        int? purchasePriceCurrencyId,
        CancellationToken cancellationToken)
    {
        var supplierExists = await _db.Set<BusinessRelationship>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == supplierBusinessRelationshipId,
                cancellationToken);

        if (!supplierExists)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(BusinessRelationship),
                    supplierBusinessRelationshipId));
        }

        var offeringExists = await _db.Set<SupplierMaterialOffering>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == supplierMaterialOfferingId
                    && x.BusinessRelationshipId == supplierBusinessRelationshipId,
                cancellationToken);

        if (!offeringExists)
        {
            return Result.Fail(
                new ConflictError(
                    "SupplierMaterialOffering does not belong to the selected supplier relationship."));
        }

        if (supplierOfferingAvailabilityId.HasValue)
        {
            var availabilityExists = await _db.Set<SupplierOfferingAvailability>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == supplierOfferingAvailabilityId.Value
                        && x.SupplierMaterialOfferingId == supplierMaterialOfferingId,
                    cancellationToken);

            if (!availabilityExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "SupplierOfferingAvailability does not belong to the selected offering."));
            }
        }

        if (inventoryLotId.HasValue)
        {
            var lotExists = await _db.Set<InventoryLot>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == inventoryLotId.Value
                        && x.SupplierBusinessRelationshipId == supplierBusinessRelationshipId,
                    cancellationToken);

            if (!lotExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "InventoryLot does not belong to the selected supplier relationship."));
            }
        }

        var unitResult = await ValidateUnitAsync(
            quantityUnitId,
            cancellationToken);

        if (unitResult.IsFailed)
        {
            return unitResult;
        }

        var currencyResult = await ValidateOptionalCurrencyAsync(
            purchasePriceCurrencyId,
            cancellationToken);

        if (currencyResult.IsFailed)
        {
            return currencyResult;
        }

        var quoteLineResult = await ValidateSupplierQuoteLineForAllocationAsync(
            supplierQuoteLineId,
            supplierBusinessRelationshipId,
            supplierMaterialOfferingId,
            inventoryLotId,
            quantityUnitId,
            unitPurchasePrice,
            purchasePriceCurrencyId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        if (quoteLineResult.IsFailed)
        {
            return Result.Fail(
                quoteLineResult.Errors);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar om allocation-sequence redan används på samma solution line.
    /// </summary>
    private Task<bool> AllocationSequenceExistsAsync(Guid supplySolutionLineId, int allocationSequence, Guid? excludeAllocationId, CancellationToken cancellationToken)
    {
        IQueryable<SupplyAllocation> query = _db.Set<SupplyAllocation>()
            .AsNoTracking()
            .Where(x => x.SupplySolutionLineId == supplySolutionLineId && x.AllocationSequence == allocationSequence);

        if (excludeAllocationId.HasValue)
        {
            query = query.Where(x => x.Id != excludeAllocationId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Kontrollerar att en UnitOfMeasure finns.
    /// </summary>
    private async Task<Result> ValidateUnitAsync(int unitOfMeasureId, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<UnitOfMeasure>().AsNoTracking().AnyAsync(x => x.Id == unitOfMeasureId, cancellationToken);

        return exists
            ? Result.Ok()
            : Result.Fail(new NotFoundError(nameof(UnitOfMeasure), unitOfMeasureId));
    }

    /// <summary>
    /// Kontrollerar en valfri Currency-referens.
    /// </summary>
    private async Task<Result> ValidateOptionalCurrencyAsync(int? currencyId, CancellationToken cancellationToken)
    {
        if (!currencyId.HasValue)
        {
            return Result.Ok();
        }

        var exists = await _db.Set<Currency>().AsNoTracking().AnyAsync(x => x.Id == currencyId.Value, cancellationToken);

        return exists
            ? Result.Ok()
            : Result.Fail(new NotFoundError(nameof(Currency), currencyId.Value));
    }

    /// <summary>
    /// Läser candidate tillsammans med run för write-operationer.
    /// </summary>
    private Task<SupplySolutionCandidate?> LoadCandidateForWriteAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        return _db.Set<SupplySolutionCandidate>()
            .Include(x => x.MatchingRun)
            .SingleOrDefaultAsync(x => x.Id == candidateId, cancellationToken);
    }

    /// <summary>
    /// Läser solution line tillsammans med candidate och run.
    /// </summary>
    private Task<SupplySolutionLine?> LoadLineForWriteAsync(Guid lineId, CancellationToken cancellationToken)
    {
        return _db.Set<SupplySolutionLine>()
            .Include(x => x.SupplySolutionCandidate)
                .ThenInclude(x => x.MatchingRun)
            .SingleOrDefaultAsync(x => x.Id == lineId, cancellationToken);
    }

    /// <summary>
    /// Läser allocation tillsammans med hela write-kontexten.
    /// </summary>
    private Task<SupplyAllocation?> LoadAllocationForWriteAsync(Guid allocationId, CancellationToken cancellationToken)
    {
        return _db.Set<SupplyAllocation>()
            .Include(x => x.SupplySolutionLine)
                .ThenInclude(x => x.SupplySolutionCandidate)
                    .ThenInclude(x => x.MatchingRun)
            .SingleOrDefaultAsync(x => x.Id == allocationId, cancellationToken);
    }

    /// <summary>
    /// Läser capacity evaluation tillsammans med candidate och run.
    /// </summary>
    private Task<CapacityEvaluation?> LoadCapacityEvaluationForWriteAsync(Guid evaluationId, CancellationToken cancellationToken)
    {
        return _db.Set<CapacityEvaluation>()
            .Include(x => x.SupplySolutionLine)
                .ThenInclude(x => x.SupplySolutionCandidate)
                    .ThenInclude(x => x.MatchingRun)
            .SingleOrDefaultAsync(x => x.Id == evaluationId, cancellationToken);
    }

    /// <summary>
    /// Markerar att candidate-aggregatets innehåll har ändrats.
    /// </summary>
    private static void TouchCandidate(SupplySolutionCandidate candidate)
    {
        candidate.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Markerar att MatchingRun-output har förändrats så att Complete och child-writes konkurrerar på samma RowVersion.
    /// </summary>
    private static void TouchRun(MatchingRun run)
    {
        run.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Sparar och översätter optimistic-concurrency-fel.
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
            return Result.Fail(CreateConcurrencyError(exception, fallbackEntityName, fallbackEntityId));
        }
    }

    /// <summary>
    /// Identifierar rätt entitet i ett EF concurrency-fel.
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

                case SupplySolutionLine line:
                    return new ConcurrencyError(nameof(SupplySolutionLine), line.Id);

                case SupplyAllocation allocation:
                    return new ConcurrencyError(nameof(SupplyAllocation), allocation.Id);

                case CapacityEvaluation evaluation:
                    return new ConcurrencyError(nameof(CapacityEvaluation), evaluation.Id);
            }
        }

        return new ConcurrencyError(fallbackEntityName, fallbackEntityId);
    }

    /// <summary>
    /// Kontrollerar om PostgreSQL rapporterade en unique violation.
    /// </summary>
    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }

    /// <summary>
    /// Projekterar candidate med hela första resultatslicen.
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
            ?? throw new InvalidOperationException($"SupplySolutionCandidate '{candidateId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Trimmar text och ersätter whitespace med null.
    /// </summary>
    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    /// <summary>
    /// Validerar att en quote line är ett aktuellt accepterat prisunderlag för exakt samma supplier-allokering.
    /// </summary>
    private async Task<Result<AcceptedSupplierQuoteLineReference?>> ValidateSupplierQuoteLineForAllocationAsync(
        Guid? supplierQuoteLineId,
        Guid supplierBusinessRelationshipId,
        Guid supplierMaterialOfferingId,
        Guid? inventoryLotId,
        int quantityUnitId,
        decimal? requestedUnitPurchasePrice,
        int? requestedPurchasePriceCurrencyId,
        DateTimeOffset evaluatedAtUtc,
        CancellationToken cancellationToken)
    {
        if (!supplierQuoteLineId.HasValue)
        {
            return Result.Ok<AcceptedSupplierQuoteLineReference?>(
                null);
        }

        var quoteLine = await _db.Set<SupplierQuoteLine>()
            .AsNoTracking()
            .Where(x => x.Id == supplierQuoteLineId.Value)
            .Select(x => new AcceptedSupplierQuoteLineReference
            {
                Id = x.Id,
                SupplierBusinessRelationshipId = x.SupplierBusinessRelationshipId,
                SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                InventoryLotId = x.InventoryLotId,
                QuantityUnitId = x.QuantityUnitId,
                UnitPrice = x.UnitPrice,
                CurrencyId = x.SupplierQuoteRevision.CurrencyId,
                ValidToUtc = x.SupplierQuoteRevision.ValidToUtc
            })
            .SingleOrDefaultAsync(
                cancellationToken);

        if (quoteLine is null)
        {
            return Result.Fail<AcceptedSupplierQuoteLineReference?>(
                new NotFoundError(
                    nameof(SupplierQuoteLine),
                    supplierQuoteLineId.Value));
        }

        var lifecycle = await _db.Set<SupplierQuoteLine>()
            .AsNoTracking()
            .Where(x => x.Id == quoteLine.Id)
            .Select(x => new
            {
                QuoteStatus = x.SupplierQuoteRevision.SupplierQuote.Status,
                RevisionStatus = x.SupplierQuoteRevision.Status
            })
            .SingleAsync(
                cancellationToken);

        if (lifecycle.QuoteStatus != SupplierQuoteStatus.AcceptedInternally
            || lifecycle.RevisionStatus != SupplierQuoteRevisionStatus.AcceptedInternally)
        {
            return Result.Fail<AcceptedSupplierQuoteLineReference?>(
                new ConflictError(
                    "SupplierQuoteLine can only be attached to a SupplyAllocation when its quote and revision are AcceptedInternally."));
        }

        if (quoteLine.ValidToUtc.HasValue
            && quoteLine.ValidToUtc.Value <= evaluatedAtUtc)
        {
            return Result.Fail<AcceptedSupplierQuoteLineReference?>(
                new ConflictError(
                    "SupplierQuoteLine belongs to an expired SupplierQuoteRevision."));
        }

        if (quoteLine.SupplierBusinessRelationshipId != supplierBusinessRelationshipId)
        {
            return Result.Fail<AcceptedSupplierQuoteLineReference?>(
                new ConflictError(
                    "SupplierQuoteLine belongs to another SupplierBusinessRelationship."));
        }

        /*
         * SupplyAllocation är alltid en material-allokering och har ett obligatoriskt
         * SupplierMaterialOfferingId. En quote line som används som purchase-price
         * provenance måste därför prissätta exakt samma offering.
         */
        if (!quoteLine.SupplierMaterialOfferingId.HasValue
            || quoteLine.SupplierMaterialOfferingId.Value != supplierMaterialOfferingId)
        {
            return Result.Fail<AcceptedSupplierQuoteLineReference?>(
                new ConflictError(
                    "SupplierQuoteLine does not price the SupplierMaterialOffering used by the SupplyAllocation."));
        }

        if (quoteLine.InventoryLotId.HasValue
            && quoteLine.InventoryLotId != inventoryLotId)
        {
            return Result.Fail<AcceptedSupplierQuoteLineReference?>(
                new ConflictError(
                    "SupplierQuoteLine references another InventoryLot than the SupplyAllocation."));
        }

        /*
         * Ingen unit conversion-semantik har definierats i denna slice.
         * Därför krävs samma unit innan UnitPrice kan användas direkt.
         */
        if (quoteLine.QuantityUnitId != quantityUnitId)
        {
            return Result.Fail<AcceptedSupplierQuoteLineReference?>(
                new ConflictError(
                    "SupplierQuoteLine and SupplyAllocation use different QuantityUnitId values."));
        }

        /*
         * Om requesten anger price/currency samtidigt som quote line används
         * måste värdena stämma exakt med det accepterade supplier-underlaget.
         */
        if (requestedUnitPurchasePrice != quoteLine.UnitPrice)
        {
            return Result.Fail<AcceptedSupplierQuoteLineReference?>(
                new ConflictError(
                    "UnitPurchasePrice must match SupplierQuoteLine.UnitPrice when SupplierQuoteLineId is supplied."));
        }

        if (requestedPurchasePriceCurrencyId != quoteLine.CurrencyId)
        {
            return Result.Fail<AcceptedSupplierQuoteLineReference?>(
                new ConflictError(
                    "PurchasePriceCurrencyId must match SupplierQuoteRevision.CurrencyId when SupplierQuoteLineId is supplied."));
        }

        return Result.Ok<AcceptedSupplierQuoteLineReference?>(
            quoteLine);
    }

    /// <summary>
    /// Innehåller kommersiella värden från en accepterad supplier-quote-line.
    /// </summary>
    private sealed class AcceptedSupplierQuoteLineReference
    {
        /// <summary>
        /// Identifierar offertraden.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar supplier-relationen.
        /// </summary>
        public Guid SupplierBusinessRelationshipId { get; init; }

        /// <summary>
        /// Identifierar material offering som prissätts.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Identifierar eventuell konkret inventory lot.
        /// </summary>
        public Guid? InventoryLotId { get; init; }

        /// <summary>
        /// Identifierar offertradens quantity unit.
        /// </summary>
        public int QuantityUnitId { get; init; }

        /// <summary>
        /// Anger supplierns accepterade unit price.
        /// </summary>
        public decimal UnitPrice { get; init; }

        /// <summary>
        /// Identifierar revisionens prisvaluta.
        /// </summary>
        public int CurrencyId { get; init; }

        /// <summary>
        /// Anger revisionens sista giltighetstid.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }
    }
}