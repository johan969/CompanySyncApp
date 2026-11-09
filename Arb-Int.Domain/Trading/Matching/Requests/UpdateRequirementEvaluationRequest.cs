using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara resultatvärden för en befintlig requirement evaluation.
/// </summary>
public sealed class UpdateRequirementEvaluationRequest
{
    /// <summary>
    /// Anger evaluationens outcome.
    /// </summary>
    public RequirementEvaluationOutcome Outcome { get; init; }

    /// <summary>
    /// Anger om resultatet är ett blockerande hard fail.
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
    /// Identifierar eventuell enhet för ActualNumericValue.
    /// </summary>
    public int? UnitOfMeasureId { get; init; }

    /// <summary>
    /// Förklarar evaluationens resultat.
    /// </summary>
    public string? Explanation { get; init; }

    /// <summary>
    /// Innehåller immutable evidence som användes.
    /// </summary>
    public string? EvidenceSnapshotJson { get; init; }

    /// <summary>
    /// Innehåller regelspåret bakom resultatet.
    /// </summary>
    public string? RuleTraceJson { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa evaluation.
    /// </summary>
    public uint RowVersion { get; init; }
}