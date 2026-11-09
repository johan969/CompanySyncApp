using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att uppdatera ett biologiskt source-krav.
/// </summary>
public sealed class UpdateBuyerRfqBiologicalSourceRequirementRequest : UpdateBuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Identifierar den biologiska källan.
    /// </summary>
    public Guid BiologicalSourceId { get; init; }

    /// <summary>
    /// Anger operatorn som används vid utvärdering.
    /// </summary>
    public RequirementOperator Operator { get; init; }
}