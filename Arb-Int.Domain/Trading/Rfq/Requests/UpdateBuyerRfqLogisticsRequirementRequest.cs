using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;
/// <summary>
/// Innehåller data för att uppdatera ett logistikkrav.
/// </summary>
public sealed class UpdateBuyerRfqLogisticsRequirementRequest : UpdateBuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Anger typen av logistikkrav.
    /// </summary>
    public LogisticsRequirementKind Kind { get; init; }

    /// <summary>
    /// Anger operatorn som används vid utvärdering.
    /// </summary>
    public RequirementOperator Operator { get; init; }

    /// <summary>
    /// Anger eventuellt numeriskt värde.
    /// </summary>
    public decimal? NumericValue { get; init; }

    /// <summary>
    /// Anger eventuellt textvärde.
    /// </summary>
    public string? TextValue { get; init; }

    /// <summary>
    /// Identifierar eventuell mätenhet.
    /// </summary>
    public int? UnitOfMeasureId { get; init; }

    /// <summary>
    /// Identifierar eventuell Incoterm-regel.
    /// </summary>
    public int? IncotermRuleId { get; init; }
}