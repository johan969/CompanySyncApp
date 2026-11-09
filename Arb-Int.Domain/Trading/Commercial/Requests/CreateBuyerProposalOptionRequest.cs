namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett buyer-facing alternativ byggt från en godkänd supply-solution-candidate.
/// </summary>
public sealed class CreateBuyerProposalOptionRequest
{
    /// <summary>
    /// Identifierar den modererade supply-solution-candidate som ligger bakom alternativet.
    /// </summary>
    public Guid SupplySolutionCandidateId { get; init; }

    /// <summary>
    /// Anger alternativets nummer inom revisionen.
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
    /// Innehåller ett internt margin-underlag som aldrig exponeras i proposal-snapshotet.
    /// </summary>
    public string? InternalMarginSnapshotJson { get; init; }
}