using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller den sammanfattning som regelmotorn har beräknat från kandidatens evaluations och score contributions.
/// </summary>
public sealed class SetCandidateEvaluationSummaryRequest
{
    /// <summary>
    /// Anger kandidatens maskinella slutresultat.
    /// </summary>
    public SupplySolutionOutcome Outcome { get; init; }

    /// <summary>
    /// Anger kandidatens sammanvägda verifieringsstatus.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Anger den slutliga score som regelmotorn beräknade.
    /// </summary>
    public decimal TotalScore { get; init; }

    /// <summary>
    /// Anger om kandidatens utvärdering innehåller ett blockerande hard fail.
    /// </summary>
    public bool ContainsHardFail { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa candidate.
    /// </summary>
    public uint RowVersion { get; init; }
}