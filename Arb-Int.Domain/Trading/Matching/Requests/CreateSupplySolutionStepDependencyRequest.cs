namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att skapa en riktad dependency mellan två solution steps.
/// </summary>
public sealed class CreateSupplySolutionStepDependencyRequest
{
    /// <summary>
    /// Identifierar steget som måste ske först.
    /// </summary>
    public Guid PredecessorStepId { get; init; }

    /// <summary>
    /// Identifierar steget som är beroende av predecessor-steget.
    /// </summary>
    public Guid SuccessorStepId { get; init; }

    /// <summary>
    /// Innehåller eventuell förklaring till beroendet.
    /// </summary>
    public string? Notes { get; init; }
}