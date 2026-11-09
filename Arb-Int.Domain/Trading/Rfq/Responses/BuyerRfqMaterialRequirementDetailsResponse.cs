namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller typspecifika värden för ett materialkrav.
/// </summary>
public sealed class BuyerRfqMaterialRequirementDetailsResponse
{
    /// <summary>
    /// Identifierar eventuellt specifikt material.
    /// </summary>
    public Guid? MaterialId { get; init; }

    /// <summary>
    /// Identifierar eventuell materialtyp.
    /// </summary>
    public int? MaterialTypeId { get; init; }

    /// <summary>
    /// Anger hur materialet ska matchas.
    /// </summary>
    public MaterialMatchMode MatchMode { get; init; }

    /// <summary>
    /// Anger om substitut får användas.
    /// </summary>
    public bool AllowSubstitutes { get; init; }
}