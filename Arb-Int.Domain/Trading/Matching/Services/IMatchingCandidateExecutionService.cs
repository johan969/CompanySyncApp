using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar worker-genererade candidate-resultat medan en matching run är Running.
/// </summary>
public interface IMatchingCandidateExecutionService
{
    /// <summary>
    /// Skapar en generated candidate i en running matchningskörning.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> CreateCandidateAsync(Guid matchingRunId, CreateSupplySolutionCandidateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar maskinberäknad candidate-metadata.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> UpdateCandidateAsync(Guid supplySolutionCandidateId, UpdateSupplySolutionCandidateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en generated candidate medan körningen fortfarande pågår.
    /// </summary>
    Task<Result> DeleteCandidateAsync(Guid supplySolutionCandidateId, uint rowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lägger till resultat för en MatchingRunLine.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> AddLineAsync(Guid supplySolutionCandidateId, CreateSupplySolutionLineRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar coverage på en solution line.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> UpdateLineAsync(Guid supplySolutionLineId, UpdateSupplySolutionLineRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en solution line under pågående generation.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> DeleteLineAsync(Guid supplySolutionLineId, uint rowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lägger till en supplier-allokering på en solution line.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> AddAllocationAsync(Guid supplySolutionLineId, CreateSupplyAllocationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar en supplier-allokering.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> UpdateAllocationAsync(Guid supplyAllocationId, UpdateSupplyAllocationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en supplier-allokering.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> DeleteAllocationAsync(Guid supplyAllocationId, uint rowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lägger till en capacity evaluation på line- eller allocation-nivå.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> AddCapacityEvaluationAsync(Guid supplySolutionLineId, CreateCapacityEvaluationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar en capacity evaluation.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> UpdateCapacityEvaluationAsync(Guid capacityEvaluationId, UpdateCapacityEvaluationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en capacity evaluation under pågående run.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> DeleteCapacityEvaluationAsync(Guid capacityEvaluationId, uint rowVersion, CancellationToken cancellationToken = default);
}