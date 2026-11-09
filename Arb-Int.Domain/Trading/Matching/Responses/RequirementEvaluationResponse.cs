using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett spårbart resultat för ett enskilt RFQ-requirement.
/// </summary>
public sealed class RequirementEvaluationResponse
{
    /// <summary>
    /// Identifierar evalueringen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar kandidaten.
    /// </summary>
    public Guid SupplySolutionCandidateId { get; init; }

    /// <summary>
    /// Identifierar RFQ-kravet.
    /// </summary>
    public Guid BuyerRfqRequirementId { get; init; }

    /// <summary>
    /// RFQ-kravets namn.
    /// </summary>
    public required string BuyerRfqRequirementName { get; init; }

    /// <summary>
    /// Anger den konkreta TPH-typen för RFQ-kravet.
    /// </summary>
    public required string BuyerRfqRequirementType { get; init; }

    /// <summary>
    /// Anger requirement-nivån.
    /// </summary>
    public RequirementLevel RequirementLevel { get; init; }

    /// <summary>
    /// Identifierar eventuell solution line.
    /// </summary>
    public Guid? SupplySolutionLineId { get; init; }

    /// <summary>
    /// Identifierar eventuell allocation.
    /// </summary>
    public Guid? SupplyAllocationId { get; init; }

    /// <summary>
    /// Identifierar eventuellt solution step.
    /// </summary>
    public Guid? SupplySolutionStepId { get; init; }

    /// <summary>
    /// Anger evaluationens outcome.
    /// </summary>
    public RequirementEvaluationOutcome Outcome { get; init; }

    /// <summary>
    /// Anger om evaluationen är ett hard fail.
    /// </summary>
    public bool IsHardFail { get; init; }

    /// <summary>
    /// Anger verifieringsstatus för underlaget.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Numeriskt faktiskt värde.
    /// </summary>
    public decimal? ActualNumericValue { get; init; }

    /// <summary>
    /// Textbaserat faktiskt värde.
    /// </summary>
    public string? ActualTextValue { get; init; }

    /// <summary>
    /// Identifierar eventuell mätenhet.
    /// </summary>
    public int? UnitOfMeasureId { get; init; }

    /// <summary>
    /// Förklarar resultatet.
    /// </summary>
    public string? Explanation { get; init; }

    /// <summary>
    /// Innehåller evidence som användes vid evalueringen.
    /// </summary>
    public string? EvidenceSnapshotJson { get; init; }

    /// <summary>
    /// Innehåller regelspåret bakom resultatet.
    /// </summary>
    public string? RuleTraceJson { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}