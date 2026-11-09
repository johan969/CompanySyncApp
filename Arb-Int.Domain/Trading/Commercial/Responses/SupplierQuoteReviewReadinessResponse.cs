namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver om en mottagen supplier-quote-revision kan gå in i intern review.
/// </summary>
public sealed class SupplierQuoteReviewReadinessResponse
{
    /// <summary>
    /// Identifierar quote-roten.
    /// </summary>
    public Guid SupplierQuoteId { get; init; }

    /// <summary>
    /// Identifierar revisionen som ska review:as.
    /// </summary>
    public Guid SupplierQuoteRevisionId { get; init; }

    /// <summary>
    /// Anger om supplier-quoten är redo för review.
    /// </summary>
    public bool IsReady { get; init; }

    /// <summary>
    /// Innehåller blockerande readiness-problem.
    /// </summary>
    public required IReadOnlyList<SupplierQuoteReadinessIssueResponse> Issues { get; init; }
}