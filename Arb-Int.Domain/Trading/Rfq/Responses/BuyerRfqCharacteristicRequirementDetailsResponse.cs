using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller typspecifika värden för ett characteristic-krav.
/// </summary>
public sealed class BuyerRfqCharacteristicRequirementDetailsResponse
{
    /// <summary>
    /// Identifierar characteristic som ska utvärderas.
    /// </summary>
    public Guid CharacteristicId { get; init; }

    /// <summary>
    /// Anger operatorn som används vid utvärdering.
    /// </summary>
    public RequirementOperator Operator { get; init; }

    /// <summary>
    /// Anger eventuellt förväntat textvärde.
    /// </summary>
    public string? ExpectedTextValue { get; init; }

    /// <summary>
    /// Anger eventuellt förväntat numeriskt värde.
    /// </summary>
    public decimal? ExpectedNumericValue { get; init; }

    /// <summary>
    /// Identifierar eventuell mätenhet.
    /// </summary>
    public int? UnitOfMeasureId { get; init; }
}