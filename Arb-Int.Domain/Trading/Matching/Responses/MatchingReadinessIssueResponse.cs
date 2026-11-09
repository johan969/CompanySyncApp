namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver ett blockerande problem vid finalisering eller completion.
/// </summary>
public sealed class MatchingReadinessIssueResponse
{
    /// <summary>
    /// Stabil maskinläsbar felkod.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Beskriver varför operationen inte är redo.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Identifierar eventuell entitet som problemet gäller.
    /// </summary>
    public Guid? EntityId { get; init; }
}