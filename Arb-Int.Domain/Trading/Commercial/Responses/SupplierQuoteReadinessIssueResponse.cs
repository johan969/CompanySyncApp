namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beskriver ett blockerande problem i supplier-quote-lifecycle.
/// </summary>
public sealed class SupplierQuoteReadinessIssueResponse
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