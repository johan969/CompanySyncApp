namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller ett materialalternativ i ett revisionssnapshot.
/// </summary>
public sealed class BuyerRfqLineMaterialOptionSnapshot
{
    /// <summary>
    /// Anger alternativets semantik.
    /// </summary>
    public MaterialOptionKind OptionKind { get; init; }

    /// <summary>
    /// Identifierar eventuellt material.
    /// </summary>
    public Guid? MaterialId { get; init; }

    /// <summary>
    /// Identifierar eventuell materialtyp.
    /// </summary>
    public int? MaterialTypeId { get; init; }

    /// <summary>
    /// Identifierar eventuell biologisk källa.
    /// </summary>
    public Guid? BiologicalSourceId { get; init; }

    /// <summary>
    /// Identifierar eventuell buyer-claim.
    /// </summary>
    public Guid? BiologicalSourceClaimId { get; init; }

    /// <summary>
    /// Köparens fria beskrivning.
    /// </summary>
    public string? BuyerText { get; init; }

    /// <summary>
    /// Grupperar alternativa materialval.
    /// </summary>
    public string? AlternativeGroupKey { get; init; }

    /// <summary>
    /// Lägsta accepterade mapping-confidence.
    /// </summary>
    public decimal? MinimumMappingConfidence { get; init; }

    /// <summary>
    /// Alternativets sorteringsordning.
    /// </summary>
    public int SortOrder { get; init; }
}