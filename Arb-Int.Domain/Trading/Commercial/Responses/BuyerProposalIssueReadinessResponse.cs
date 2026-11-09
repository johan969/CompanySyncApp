namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver om en Draft proposal-revision kan utfärdas.
/// </summary>
public sealed class BuyerProposalIssueReadinessResponse
{
    /// <summary>
    /// Identifierar proposalet.
    /// </summary>
    public Guid BuyerProposalId { get; init; }

    /// <summary>
    /// Identifierar revisionen.
    /// </summary>
    public Guid BuyerProposalRevisionId { get; init; }

    /// <summary>
    /// Anger om revisionen kan utfärdas.
    /// </summary>
    public bool IsReady { get; init; }

    /// <summary>
    /// Innehåller blockerande readiness-problem.
    /// </summary>
    public required IReadOnlyList<BuyerProposalReadinessIssueResponse> Issues { get; init; }
}

/// <summary>
/// Beskriver ett blockerande problem vid proposal issue.
/// </summary>
public sealed class BuyerProposalReadinessIssueResponse
{
    /// <summary>
    /// Stabil maskinläsbar felkod.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Beskriver problemet.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Identifierar eventuell entitet som problemet gäller.
    /// </summary>
    public Guid? EntityId { get; init; }
}