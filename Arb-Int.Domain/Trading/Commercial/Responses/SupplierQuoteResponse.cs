namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller supplier-quote-roten och dess kompletta revisionshistorik.
/// </summary>
public sealed class SupplierQuoteResponse
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
    /// Identifierar eventuell RFQ-revision.
    /// </summary>
    public Guid? BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Anger det stabila quote-numret.
    /// </summary>
    public required string QuoteNumber { get; init; }

    /// <summary>
    /// Anger quotens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver quoten.
    /// </summary>
    public required string Description { get; init; }

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
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Innehåller revisionshistoriken med senaste revision först.
    /// </summary>
    public required IReadOnlyList<SupplierQuoteRevisionResponse> Revisions { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}