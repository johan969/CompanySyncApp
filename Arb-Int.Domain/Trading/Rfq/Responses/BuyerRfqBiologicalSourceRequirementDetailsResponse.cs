using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller typspecifika värden för ett biologiskt source-krav.
/// </summary>
public sealed class BuyerRfqBiologicalSourceRequirementDetailsResponse
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