namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett buyer-facing proposal-alternativ och dess interna candidate-provenance.
/// </summary>
public sealed class BuyerProposalOptionResponse
{
    /// <summary>
    /// Identifierar optionen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar proposal-revisionen.
    /// </summary>
    public Guid BuyerProposalRevisionId { get; init; }

    /// <summary>
    /// Identifierar den interna supply-solution-candidate som alternativet bygger på.
    /// </summary>
    public Guid SupplySolutionCandidateId { get; init; }

    /// <summary>
    /// Anger option-numret.
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
    /// Anger workflow-status.
    /// </summary>
    public BuyerProposalOptionStatus Status { get; init; }

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
    /// Identifierar buyer-facing valuta.
    /// </summary>
    public int CurrencyId { get; init; }

    /// <summary>
    /// Anger uppskattad total ledtid.
    /// </summary>
    public int? EstimatedLeadTimeDays { get; init; }

    /// <summary>
    /// Innehåller buyer-facing sammanfattning.
    /// </summary>
    public string? BuyerFacingSummary { get; init; }

    /// <summary>
    /// Innehåller internt margin-underlag.
    /// </summary>
    public string? InternalMarginSnapshotJson { get; init; }

    /// <summary>
    /// Innehåller optionens proposal-lines.
    /// </summary>
    public required IReadOnlyList<BuyerProposalLineResponse> Lines { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}