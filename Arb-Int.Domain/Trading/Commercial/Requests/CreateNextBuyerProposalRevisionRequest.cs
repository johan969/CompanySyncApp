namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller target-RFQ-revision och concurrency-data för nästa proposal-revision.
/// </summary>
public sealed class CreateNextBuyerProposalRevisionRequest
{
    /// <summary>
    /// Identifierar RFQ-revisionen som den nya proposal-revisionen ska baseras på.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Concurrency-token för proposal-roten.
    /// </summary>
    public uint BuyerProposalRowVersion { get; init; }
}