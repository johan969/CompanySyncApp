using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar worker-ägda statusövergångar för matching runs.
/// </summary>
public interface IMatchingRunExecutionService
{
    /// <summary>
    /// Claimar en queued körning och markerar den som Running.
    /// </summary>
    Task<Result<MatchingRunResponse>> StartAsync(Guid matchingRunId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Markerar en running körning som Failed och sparar felet.
    /// </summary>
    Task<Result<MatchingRunResponse>> FailAsync(Guid matchingRunId, string errorMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kontrollerar om en running matching run kan slutföras.
    /// </summary>
    Task<Result<MatchingRunCompletionReadinessResponse>> GetCompletionReadinessAsync(Guid matchingRunId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Markerar en running matching run som Completed när samtliga resultat är finaliserade.
    /// </summary>
    Task<Result<MatchingRunResponse>> CompleteAsync(Guid matchingRunId, CompleteMatchingRunRequest request, CancellationToken cancellationToken = default);
}