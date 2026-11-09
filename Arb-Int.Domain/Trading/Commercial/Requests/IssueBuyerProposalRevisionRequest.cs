namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency- och audit-data för att utfärda en proposal-revision.
/// </summary>
public sealed class IssueBuyerProposalRevisionRequest
{
    /// <summary>
    /// Identifierar eventuell användare som utfärdar revisionen.
    /// </summary>
    public Guid? IssuedByUserId { get; init; }

    /// <summary>
    /// Concurrency-token för proposal-roten.
    /// </summary>
    public uint BuyerProposalRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för proposal-revisionen.
    /// </summary>
    public uint BuyerProposalRevisionRowVersion { get; init; }
}