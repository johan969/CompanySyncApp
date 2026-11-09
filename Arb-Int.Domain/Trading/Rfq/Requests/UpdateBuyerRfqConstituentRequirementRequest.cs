using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att uppdatera ett constituent-krav.
/// </summary>
public sealed class UpdateBuyerRfqConstituentRequirementRequest : UpdateBuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Identifierar constituent som ska mätas.
    /// </summary>
    public Guid ConstituentId { get; init; }

    /// <summary>
    /// Anger operatorn som används vid utvärdering.
    /// </summary>
    public RequirementOperator Operator { get; init; }

    /// <summary>
    /// Anger eventuellt minimivärde.
    /// </summary>
    public decimal? MinimumValue { get; init; }

    /// <summary>
    /// Anger eventuellt maximivärde.
    /// </summary>
    public decimal? MaximumValue { get; init; }

    /// <summary>
    /// Anger eventuellt målvärde.
    /// </summary>
    public decimal? TargetValue { get; init; }

    /// <summary>
    /// Identifierar eventuell mätenhet.
    /// </summary>
    public int? UnitOfMeasureId { get; init; }

    /// <summary>
    /// Anger vilken measurement basis värdet gäller.
    /// </summary>
    public MeasurementBasis Basis { get; init; }

    /// <summary>
    /// Anger eventuell analysmetod.
    /// </summary>
    public string? AnalysisMethod { get; init; }

    /// <summary>
    /// Anger om värdet måste verifieras för varje lot.
    /// </summary>
    public bool MustBeVerifiedForEveryLot { get; init; }

    /// <summary>
    /// Anger om ett ackrediterat laboratorium krävs.
    /// </summary>
    public bool AccreditedLaboratoryRequired { get; init; }
}