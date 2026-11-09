namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett historiskt moderation-beslut och dess eventuella override av maskinresultatet.
/// </summary>
public sealed class MatchingModerationDecisionResponse
{
    /// <summary>
    /// Identifierar moderation-beslutet.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar kandidaten.
    /// </summary>
    public Guid SupplySolutionCandidateId { get; init; }

    /// <summary>
    /// Anger moderation-beslutets typ.
    /// </summary>
    public MatchingModerationDecisionKind Decision { get; init; }

    /// <summary>
    /// Identifierar användaren som fattade beslutet.
    /// </summary>
    public Guid DecidedByUserId { get; init; }

    /// <summary>
    /// Anger när beslutet fattades.
    /// </summary>
    public DateTimeOffset DecidedAtUtc { get; init; }

    /// <summary>
    /// Anger om beslutet ersätter delar av maskinresultatet.
    /// </summary>
    public bool IsManualOverride { get; init; }

    /// <summary>
    /// Anger eventuellt outcome som override:ar maskinresultatet.
    /// </summary>
    public SupplySolutionOutcome? OverriddenOutcome { get; init; }

    /// <summary>
    /// Anger eventuell score som override:ar maskinresultatet.
    /// </summary>
    public decimal? OverriddenScore { get; init; }

    /// <summary>
    /// Anger outcome som gäller för just detta moderation-beslut efter eventuell override.
    /// </summary>
    public SupplySolutionOutcome EffectiveOutcome { get; init; }

    /// <summary>
    /// Anger score som gäller för just detta moderation-beslut efter eventuell override.
    /// </summary>
    public decimal EffectiveScore { get; init; }

    /// <summary>
    /// Förklarar varför beslutet fattades.
    /// </summary>
    public required string Reason { get; init; }

    /// <summary>
    /// Innehåller kompletterande interna anteckningar.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}