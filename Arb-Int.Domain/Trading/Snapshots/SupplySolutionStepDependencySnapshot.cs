namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller en riktad edge i kandidatens step-graf.
/// </summary>
public sealed class SupplySolutionStepDependencySnapshot
{
    /// <summary>
    /// Anger predecessor-stegets sekvensnummer.
    /// </summary>
    public int PredecessorSequenceNumber { get; init; }

    /// <summary>
    /// Anger successor-stegets sekvensnummer.
    /// </summary>
    public int SuccessorSequenceNumber { get; init; }

    /// <summary>
    /// Innehåller eventuell förklaring till beroendet.
    /// </summary>
    public string? Notes { get; init; }
}