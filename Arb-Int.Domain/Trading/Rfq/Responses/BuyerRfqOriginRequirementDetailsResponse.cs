using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller typspecifika värden för ett ursprungskrav.
/// </summary>
public sealed class BuyerRfqOriginRequirementDetailsResponse
{
    /// <summary>
    /// Identifierar eventuell geografisk plats.
    /// </summary>
    public Guid? GeographicLocationId { get; init; }

    /// <summary>
    /// Identifierar eventuellt land.
    /// </summary>
    public Guid? CountryId { get; init; }

    /// <summary>
    /// Anger vilken typ av ursprung kravet gäller.
    /// </summary>
    public OriginRole OriginRole { get; init; }

    /// <summary>
    /// Anger operatorn som används vid utvärdering.
    /// </summary>
    public RequirementOperator Operator { get; init; }
}