namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency- och audit-data för intern approval av en buyer acceptance.
/// </summary>
public sealed class ApproveBuyerProposalAcceptanceRequest
{
    /// <summary>
    /// Identifierar eventuell användare som godkänner acceptansen internt.
    /// </summary>
    public Guid? InternallyApprovedByUserId { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar om approval-beslutet.
    /// </summary>
    public string? InternalApprovalNotes { get; init; }

    /// <summary>
    /// Concurrency-token för acceptance-entiteten.
    /// </summary>
    public uint BuyerProposalAcceptanceRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för proposal-roten.
    /// </summary>
    public uint BuyerProposalRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för proposal-revisionen.
    /// </summary>
    public uint BuyerProposalRevisionRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för proposal-optionen.
    /// </summary>
    public uint BuyerProposalOptionRowVersion { get; init; }
}