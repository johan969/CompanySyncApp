namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver om en candidate kan frysas som immutable maskinellt resultat.
/// </summary>
public sealed class SupplySolutionCandidateFinalizationReadinessResponse
{
    /// <summary>
    /// Identifierar kandidaten.
    /// </summary>
    public Guid SupplySolutionCandidateId { get; init; }

    /// <summary>
    /// Anger om kandidaten kan finaliseras.
    /// </summary>
    public bool IsReady { get; init; }

    /// <summary>
    /// Innehåller blockerande problem.
    /// </summary>
    public required IReadOnlyList<MatchingReadinessIssueResponse> Issues { get; init; }
}