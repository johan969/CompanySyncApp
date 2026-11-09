namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data och interna anteckningar när en buyer acceptance nekas internt.
/// </summary>
public sealed class RejectBuyerProposalAcceptanceRequest
{
    /// <summary>
    /// Förklarar den interna bedömningen eller varför acceptansen inte godkändes.
    /// </summary>
    public string? InternalApprovalNotes { get; init; }

    /// <summary>
    /// Concurrency-token för acceptance-entiteten.
    /// </summary>
    public uint RowVersion { get; init; }
}