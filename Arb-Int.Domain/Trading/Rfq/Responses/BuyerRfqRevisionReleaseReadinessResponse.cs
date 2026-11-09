namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller resultatet av release-valideringen för en RFQ-revision.
/// </summary>
public sealed class BuyerRfqRevisionReleaseReadinessResponse
{
    /// <summary>
    /// Identifierar revisionen som validerades.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Anger om revisionen får släppas.
    /// </summary>
    public bool IsReady { get; init; }

    /// <summary>
    /// Innehåller blockerande problem som måste lösas innan release.
    /// </summary>
    public required IReadOnlyList<BuyerRfqReleaseIssueResponse> Issues { get; init; }
}