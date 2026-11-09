namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara buyer-facing och interna värden för ett Draft-alternativ.
/// </summary>
public sealed class UpdateBuyerProposalOptionRequest
{
    /// <summary>
    /// Anger alternativets nummer.
    /// </summary>
    public int OptionNumber { get; init; }

    /// <summary>
    /// Anger alternativets buyer-facing namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger alternativets buyer-facing beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger uppskattad total ledtid.
    /// </summary>
    public int? EstimatedLeadTimeDays { get; init; }

    /// <summary>
    /// Innehåller en buyer-facing sammanfattning.
    /// </summary>
    public string? BuyerFacingSummary { get; init; }

    /// <summary>
    /// Innehåller det interna margin-underlaget.
    /// </summary>
    public string? InternalMarginSnapshotJson { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa option.
    /// </summary>
    public uint RowVersion { get; init; }
}