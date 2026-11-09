namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en riktad dependency mellan två solution steps.
/// </summary>
public sealed class SupplySolutionStepDependencyResponse
{
    /// <summary>
    /// Identifierar dependency-posten.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar kandidaten som grafen tillhör.
    /// </summary>
    public Guid SupplySolutionCandidateId { get; init; }

    /// <summary>
    /// Identifierar predecessor-steget.
    /// </summary>
    public Guid PredecessorStepId { get; init; }

    /// <summary>
    /// Anger predecessor-stegets sekvensnummer.
    /// </summary>
    public int PredecessorSequenceNumber { get; init; }

    /// <summary>
    /// Identifierar successor-steget.
    /// </summary>
    public Guid SuccessorStepId { get; init; }

    /// <summary>
    /// Anger successor-stegets sekvensnummer.
    /// </summary>
    public int SuccessorSequenceNumber { get; init; }

    /// <summary>
    /// Innehåller eventuell förklaring till beroendet.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}