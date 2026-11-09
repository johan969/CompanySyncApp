namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett append-only moderation-beslut för en finaliserad supply-solution-candidate.
/// </summary>
public sealed class CreateMatchingModerationDecisionRequest
{
    /// <summary>
    /// Anger vilket moderation-beslut som fattades.
    /// </summary>
    public MatchingModerationDecisionKind Decision { get; init; }

    /// <summary>
    /// Identifierar användaren som fattade beslutet.
    /// </summary>
    public Guid DecidedByUserId { get; init; }

    /// <summary>
    /// Anger om beslutet manuellt ersätter delar av maskinresultatet.
    /// </summary>
    public bool IsManualOverride { get; init; }

    /// <summary>
    /// Anger eventuellt manuellt outcome som gäller för detta beslut.
    /// </summary>
    public SupplySolutionOutcome? OverriddenOutcome { get; init; }

    /// <summary>
    /// Anger eventuell manuellt justerad total score.
    /// </summary>
    public decimal? OverriddenScore { get; init; }

    /// <summary>
    /// Förklarar varför moderation-beslutet fattades.
    /// </summary>
    public required string Reason { get; init; }

    /// <summary>
    /// Innehåller kompletterande interna anteckningar.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa candidate.
    /// </summary>
    public uint SupplySolutionCandidateRowVersion { get; init; }
}