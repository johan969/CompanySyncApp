using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar worker-genererade solution steps och deras dependency-graf.
/// </summary>
public interface IMatchingStepExecutionService
{
    /// <summary>
    /// Skapar ett Proposed solution step i en generated candidate.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> AddStepAsync(Guid supplySolutionCandidateId, CreateSupplySolutionStepRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett Proposed solution step.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> UpdateStepAsync(Guid supplySolutionStepId, UpdateSupplySolutionStepRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort ett Proposed step som inte längre refereras av graf eller evaluations.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> DeleteStepAsync(Guid supplySolutionStepId, uint rowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar en riktad dependency mellan två steps utan att skapa en cykel.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> AddDependencyAsync(Guid supplySolutionCandidateId, CreateSupplySolutionStepDependencyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar metadata på en befintlig dependency.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> UpdateDependencyAsync(Guid supplySolutionStepDependencyId, UpdateSupplySolutionStepDependencyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en dependency från grafen.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> DeleteDependencyAsync(Guid supplySolutionStepDependencyId, uint rowVersion, CancellationToken cancellationToken = default);
}