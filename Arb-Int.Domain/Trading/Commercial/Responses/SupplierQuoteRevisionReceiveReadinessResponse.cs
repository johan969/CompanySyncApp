namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver om en supplier-quote-revision kan tas emot och frysas.
/// </summary>
public sealed class SupplierQuoteRevisionReceiveReadinessResponse
{
    /// <summary>
    /// Identifierar revisionen.
    /// </summary>
    public Guid SupplierQuoteRevisionId { get; init; }

    /// <summary>
    /// Anger om revisionen är redo.
    /// </summary>
    public bool IsReady { get; init; }

    /// <summary>
    /// Innehåller blockerande problem.
    /// </summary>
    public required IReadOnlyList<MatchingReadinessIssueResponse> Issues { get; init; }
}