namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver resultatet från automatisk expiration av supplier quotes.
/// </summary>
public sealed class SupplierQuoteExpirationBatchResponse
{
    /// <summary>
    /// Anger tidpunkten som användes för expiration-kontrollen.
    /// </summary>
    public DateTimeOffset EvaluatedAtUtc { get; init; }

    /// <summary>
    /// Anger hur många current revisions som identifierades som förfallna.
    /// </summary>
    public int CandidateCount { get; init; }

    /// <summary>
    /// Anger hur många revisionsrader som ändrades till Expired.
    /// </summary>
    public int ExpiredRevisionCount { get; init; }

    /// <summary>
    /// Anger hur många quote-roots som ändrades till Expired.
    /// </summary>
    public int ExpiredQuoteCount { get; init; }
}