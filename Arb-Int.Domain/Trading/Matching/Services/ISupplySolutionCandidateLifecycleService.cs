using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar tidsstyrd lifecycle för finaliserade supply-solution-candidates.
/// </summary>
public interface ISupplySolutionCandidateLifecycleService
{
    /// <summary>
    /// Markerar en enskild candidate som Expired efter concurrency- och lifecycle-kontroll.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> ExpireAsync(Guid supplySolutionCandidateId, ExpireSupplySolutionCandidateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Markerar en begränsad batch av förfallna candidates som Expired.
    /// </summary>
    Task<Result<SupplySolutionCandidateExpirationBatchResponse>> ExpireDueAsync(DateTimeOffset evaluatedAtUtc, int batchSize = 500, CancellationToken cancellationToken = default);
}