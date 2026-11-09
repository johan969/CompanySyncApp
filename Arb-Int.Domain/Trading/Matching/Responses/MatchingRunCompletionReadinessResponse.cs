namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver om en matching run kan markeras som Completed.
/// </summary>
public sealed class MatchingRunCompletionReadinessResponse
{
    /// <summary>
    /// Identifierar matchningskörningen.
    /// </summary>
    public Guid MatchingRunId { get; init; }

    /// <summary>
    /// Anger om körningen kan slutföras.
    /// </summary>
    public bool IsReady { get; init; }

    /// <summary>
    /// Anger hur många candidates körningen producerade.
    /// </summary>
    public int CandidateCount { get; init; }

    /// <summary>
    /// Anger hur många candidates som har ett giltigt immutable snapshot.
    /// </summary>
    public int FinalizedCandidateCount { get; init; }

    /// <summary>
    /// Innehåller blockerande problem.
    /// </summary>
    public required IReadOnlyList<MatchingReadinessIssueResponse> Issues { get; init; }
}