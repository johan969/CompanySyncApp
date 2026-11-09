namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller ett buyer-facing alternativ utan supplier- eller candidate-identitet.
/// </summary>
public sealed class BuyerProposalOptionSnapshot
{
    /// <summary>
    /// Anger alternativets nummer.
    /// </summary>
    public int OptionNumber { get; init; }

    /// <summary>
    /// Anger buyer-facing namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger buyer-facing beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger totalt net amount.
    /// </summary>
    public decimal TotalNetAmount { get; init; }

    /// <summary>
    /// Anger totalt skattebelopp när skatt används.
    /// </summary>
    public decimal? TotalTaxAmount { get; init; }

    /// <summary>
    /// Anger totalt gross amount.
    /// </summary>
    public decimal TotalGrossAmount { get; init; }

    /// <summary>
    /// Identifierar valutan.
    /// </summary>
    public int CurrencyId { get; init; }

    /// <summary>
    /// Anger uppskattad ledtid.
    /// </summary>
    public int? EstimatedLeadTimeDays { get; init; }

    /// <summary>
    /// Innehåller buyer-facing sammanfattning.
    /// </summary>
    public string? BuyerFacingSummary { get; init; }

    /// <summary>
    /// Innehåller buyer-facing lines.
    /// </summary>
    public required IReadOnlyList<BuyerProposalLineSnapshot> Lines { get; init; }
}