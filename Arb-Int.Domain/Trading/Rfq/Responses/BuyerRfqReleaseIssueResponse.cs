namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver ett blockerande problem som måste lösas innan en RFQ-revision kan släppas.
/// </summary>
public sealed class BuyerRfqReleaseIssueResponse
{
    /// <summary>
    /// Maskinläsbar kod för problemet.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Läsbar beskrivning av problemet.
    /// </summary>
    public required string Message { get; init; }
}