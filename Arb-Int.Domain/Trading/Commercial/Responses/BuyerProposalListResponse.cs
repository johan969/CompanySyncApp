namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller kompakt information om ett BuyerProposal för listor och sökresultat.
/// </summary>
public sealed class BuyerProposalListResponse
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
    /// Identifierar buyer-parten.
    /// </summary>
    public Guid BuyerPartyId { get; init; }

    /// <summary>
    /// Köparens visningsnamn.
    /// </summary>
    public required string BuyerName { get; init; }

    /// <summary>
    /// Identifierar RFQ-roten.
    /// </summary>
    public Guid BuyerRfqId { get; init; }

    /// <summary>
    /// Anger RFQ-numret.
    /// </summary>
    public required string BuyerRfqNumber { get; init; }

    /// <summary>
    /// Anger proposalets stabila nummer.
    /// </summary>
    public required string ProposalNumber { get; init; }

    /// <summary>
    /// Anger proposalets namn.
    /// </summary>
    public required string Name { get; init; }

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
    /// Anger när proposalet skapades.
    /// </summary>
    public DateTimeOffset? CreateDateUtc { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}
