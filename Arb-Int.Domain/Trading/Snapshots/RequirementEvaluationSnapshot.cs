using System.Text.Json;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller ett fryst requirement-resultat.
/// </summary>
public sealed class RequirementEvaluationSnapshot
{
    /// <summary>
    /// Anger ett deterministiskt lokalt nummer som score contributions kan referera till.
    /// </summary>
    public int EvaluationNumber { get; init; }

    /// <summary>
    /// Identifierar RFQ-kravet.
    /// </summary>
    public Guid BuyerRfqRequirementId { get; init; }

    /// <summary>
    /// Anger RFQ-kravets namn för läsbarhet.
    /// </summary>
    public required string BuyerRfqRequirementName { get; init; }

    /// <summary>
    /// Anger requirementets nivå.
    /// </summary>
    public RequirementLevel RequirementLevel { get; init; }

    /// <summary>
    /// Anger eventuell RFQ-line som evaluationen gäller.
    /// </summary>
    public int? BuyerRfqLineNumber { get; init; }

    /// <summary>
    /// Anger eventuell allocation genom dess sekvensnummer.
    /// </summary>
    public int? AllocationSequence { get; init; }

    /// <summary>
    /// Anger eventuellt solution step genom dess sekvensnummer.
    /// </summary>
    public int? SupplySolutionStepSequenceNumber { get; init; }

    /// <summary>
    /// Anger evaluationens outcome.
    /// </summary>
    public RequirementEvaluationOutcome Outcome { get; init; }

    /// <summary>
    /// Anger om requirement-regeln är markerad som hard fail.
    /// </summary>
    public bool IsHardFail { get; init; }

    /// <summary>
    /// Anger verifieringsstatus för underlaget.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Anger eventuellt faktiskt numeriskt värde.
    /// </summary>
    public decimal? ActualNumericValue { get; init; }

    /// <summary>
    /// Anger eventuellt faktiskt textvärde.
    /// </summary>
    public string? ActualTextValue { get; init; }

    /// <summary>
    /// Identifierar eventuell enhet för det numeriska värdet.
    /// </summary>
    public int? UnitOfMeasureId { get; init; }

    /// <summary>
    /// Förklarar evaluationens resultat.
    /// </summary>
    public string? Explanation { get; init; }

    /// <summary>
    /// Innehåller evidence som användes av regelmotorn.
    /// </summary>
    public JsonElement? Evidence { get; init; }

    /// <summary>
    /// Innehåller regelspåret bakom resultatet.
    /// </summary>
    public JsonElement? RuleTrace { get; init; }
}