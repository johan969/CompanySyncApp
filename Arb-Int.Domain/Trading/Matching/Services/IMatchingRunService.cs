using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar skapande, läsning och användarstyrda avbrott av matching runs.
/// </summary>
public interface IMatchingRunService
{
    /// <summary>
    /// Hämtar en matchningskörning.
    /// </summary>
    Task<Result<MatchingRunResponse>> GetByIdAsync(Guid matchingRunId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar matchningskörningar för en RFQ-revision.
    /// </summary>
    Task<Result<IReadOnlyList<MatchingRunResponse>>> GetByRevisionAsync(Guid buyerRfqRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar immutable run-input och köar en ny matching run.
    /// </summary>
    Task<Result<MatchingRunResponse>> QueueAsync(Guid buyerRfqRevisionId, QueueMatchingRunRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Avbryter en queued eller running körning.
    /// </summary>
    Task<Result<MatchingRunResponse>> CancelAsync(Guid matchingRunId, CancelMatchingRunRequest request, CancellationToken cancellationToken = default);
}