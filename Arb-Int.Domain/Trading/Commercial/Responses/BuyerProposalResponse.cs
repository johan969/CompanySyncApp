namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller buyer-proposal-roten och dess revisionshistorik.
/// </summary>
public sealed class BuyerProposalResponse
{
    /// <summary>
    /// Identifierar proposalet.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar buyer-relationen.
    /// </summary>
    public Guid BuyerBusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar RFQ-roten.
    /// </summary>
    public Guid BuyerRfqId { get; init; }

    /// <summary>
    /// Anger det stabila proposal-numret.
    /// </summary>
    public required string ProposalNumber { get; init; }

    /// <summary>
    /// Anger proposalets namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver proposalet.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger proposalets lifecycle-status.
    /// </summary>
    public BuyerProposalStatus Status { get; init; }

    /// <summary>
    /// Anger högsta skapade revisionsnummer.
    /// </summary>
    public int LatestRevisionNumber { get; init; }

    /// <summary>
    /// Identifierar eventuell intern ägare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Anger när proposalet först utfärdades.
    /// </summary>
    public DateTimeOffset? IssuedAtUtc { get; init; }

    /// <summary>
    /// Anger när proposalet stängdes.
    /// </summary>
    public DateTimeOffset? ClosedAtUtc { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Innehåller hela revisionshistoriken.
    /// </summary>
    public required IReadOnlyList<BuyerProposalRevisionResponse> Revisions { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}