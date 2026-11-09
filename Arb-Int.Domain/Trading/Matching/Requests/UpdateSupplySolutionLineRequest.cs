namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara coverage-värden för en candidate-line.
/// </summary>
public sealed class UpdateSupplySolutionLineRequest
{
    /// <summary>
    /// Anger hur mycket av required quantity som kandidaten kan täcka.
    /// </summary>
    public decimal CoveredQuantity { get; init; }

    /// <summary>
    /// Intern förklaring till line-resultatet.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa line.
    /// </summary>
    public uint RowVersion { get; init; }
}