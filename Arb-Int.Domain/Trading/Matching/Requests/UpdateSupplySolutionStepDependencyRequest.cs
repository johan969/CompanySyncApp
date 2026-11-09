namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara metadata för en step-dependency.
/// </summary>
public sealed class UpdateSupplySolutionStepDependencyRequest
{
    /// <summary>
    /// Innehåller eventuell förklaring till beroendet.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa dependency.
    /// </summary>
    public uint RowVersion { get; init; }
}