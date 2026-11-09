namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver resultatet från en batch som markerar förfallna candidates som Expired.
/// </summary>
public sealed class SupplySolutionCandidateExpirationBatchResponse
{
    /// <summary>
    /// Anger tidpunkten som användes vid expiration-kontrollen.
    /// </summary>
    public DateTimeOffset EvaluatedAtUtc { get; init; }

    /// <summary>
    /// Anger hur många candidates som hittades i batchen.
    /// </summary>
    public int CandidateCount { get; init; }

    /// <summary>
    /// Anger hur många candidates som faktiskt ändrades till Expired.
    /// </summary>
    public int ExpiredCount { get; init; }
}