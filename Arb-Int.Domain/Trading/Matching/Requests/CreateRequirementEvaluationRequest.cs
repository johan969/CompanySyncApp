using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett requirement-resultat som matchningsmotorn har beräknat för en candidate.
/// </summary>
public sealed class CreateRequirementEvaluationRequest
{
    /// <summary>
    /// Identifierar RFQ-kravet som utvärderades.
    /// </summary>
    public Guid BuyerRfqRequirementId { get; init; }

    /// <summary>
    /// Identifierar eventuell solution line som resultatet gäller.
    /// </summary>
    public Guid? SupplySolutionLineId { get; init; }

    /// <summary>
    /// Identifierar eventuell supplier-allokering som resultatet gäller.
    /// </summary>
    public Guid? SupplyAllocationId { get; init; }

    /// <summary>
    /// Identifierar eventuellt solution step som resultatet gäller.
    /// </summary>
    public Guid? SupplySolutionStepId { get; init; }

    /// <summary>
    /// Anger om kravet passerade, misslyckades eller inte kunde avgöras.
    /// </summary>
    public RequirementEvaluationOutcome Outcome { get; init; }

    /// <summary>
    /// Anger om resultatet ska behandlas som ett blockerande hard fail.
    /// </summary>
    public bool IsHardFail { get; init; }

    /// <summary>
    /// Anger verifieringsstatus för underlaget bakom resultatet.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Numeriskt faktiskt värde när kravet utvärderas numeriskt.
    /// </summary>
    public decimal? ActualNumericValue { get; init; }

    /// <summary>
    /// Textbaserat faktiskt värde när kravet utvärderas kvalitativt.
    /// </summary>
    public string? ActualTextValue { get; init; }

    /// <summary>
    /// Identifierar eventuell enhet för ActualNumericValue.
    /// </summary>
    public int? UnitOfMeasureId { get; init; }

    /// <summary>
    /// Förklarar varför evalueringen fick sitt outcome.
    /// </summary>
    public string? Explanation { get; init; }

    /// <summary>
    /// Innehåller immutable evidence som användes vid evalueringen.
    /// </summary>
    public string? EvidenceSnapshotJson { get; init; }

    /// <summary>
    /// Innehåller regelspåret som förklarar hur evalueringen beräknades.
    /// </summary>
    public string? RuleTraceJson { get; init; }
}