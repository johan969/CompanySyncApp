using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att skapa ett kommersiellt krav.
/// </summary>
public sealed class CreateBuyerRfqCommercialRequirementRequest : BuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Anger typen av kommersiellt krav.
    /// </summary>
    public CommercialRequirementKind Kind { get; init; }

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
    /// Identifierar eventuell valuta.
    /// </summary>
    public int? CurrencyId { get; init; }
}