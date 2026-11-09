namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att lägga till ett materialalternativ på en RFQ-rad.
/// </summary>
public sealed class CreateBuyerRfqLineMaterialOptionRequest
{
    /// <summary>
    /// Anger om alternativet är föredraget, tillåtet eller exkluderat.
    /// </summary>
    public MaterialOptionKind OptionKind { get; init; }

    /// <summary>
    /// Identifierar eventuellt specifikt material.
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
    /// Identifierar eventuell biologisk source-claim.
    /// </summary>
    public Guid? BiologicalSourceClaimId { get; init; }

    /// <summary>
    /// Köparens fria beskrivning av alternativet.
    /// </summary>
    public string? BuyerText { get; init; }

    /// <summary>
    /// Grupperar alternativa val som hör samman.
    /// </summary>
    public string? AlternativeGroupKey { get; init; }

    /// <summary>
    /// Anger eventuell lägsta accepterade mapping-confidence.
    /// </summary>
    public decimal? MinimumMappingConfidence { get; init; }

    /// <summary>
    /// Anger alternativets sorteringsordning.
    /// </summary>
    public int SortOrder { get; init; }
}