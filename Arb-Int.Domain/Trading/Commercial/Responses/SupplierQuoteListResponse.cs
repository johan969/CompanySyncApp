namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller kompakt information om en SupplierQuote för listor och sökresultat.
/// </summary>
public sealed class SupplierQuoteListResponse
{
    /// <summary>
    /// Identifierar quoten.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar supplier-relationen.
    /// </summary>
    public Guid SupplierBusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar supplier-parten.
    /// </summary>
    public Guid SupplierPartyId { get; init; }

    /// <summary>
    /// Supplierns visningsnamn.
    /// </summary>
    public required string SupplierName { get; init; }

    /// <summary>
    /// Identifierar eventuell BuyerRfq.
    /// </summary>
    public Guid? BuyerRfqId { get; init; }

    /// <summary>
    /// Identifierar eventuell BuyerRfqRevision.
    /// </summary>
    public Guid? BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Anger RFQ-numret när quoten är kopplad till en RFQ.
    /// </summary>
    public string? BuyerRfqNumber { get; init; }

    /// <summary>
    /// Anger quotens stabila nummer.
    /// </summary>
    public required string QuoteNumber { get; init; }

    /// <summary>
    /// Anger quotens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger quote-rootens workflow-status.
    /// </summary>
    public SupplierQuoteStatus Status { get; init; }

    /// <summary>
    /// Anger högsta skapade revisionsnummer.
    /// </summary>
    public int LatestRevisionNumber { get; init; }

    /// <summary>
    /// Anger supplierns externa referens.
    /// </summary>
    public string? SupplierReference { get; init; }

    /// <summary>
    /// Anger när quoten först mottogs.
    /// </summary>
    public DateTimeOffset? ReceivedAtUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell intern ägare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Anger när quoten skapades.
    /// </summary>
    public DateTimeOffset? CreateDateUtc { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}
