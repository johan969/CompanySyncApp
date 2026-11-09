namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att skapa en candidate-line från en MatchingRunLine.
/// </summary>
public sealed class CreateSupplySolutionLineRequest
{
    /// <summary>
    /// Identifierar den frysta RFQ-rad som lösningen gäller.
    /// </summary>
    public Guid MatchingRunLineId { get; init; }

    /// <summary>
    /// Anger hur mycket av radens required quantity som kandidaten kan täcka i run-radens enhet.
    /// </summary>
    public decimal CoveredQuantity { get; init; }

    /// <summary>
    /// Intern förklaring till line-resultatet.
    /// </summary>
    public string? Notes { get; init; }
}