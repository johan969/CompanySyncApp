namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en supplier-quote-revision med dess kommersiella villkor och lines.
/// </summary>
public sealed class SupplierQuoteRevisionResponse
{
    /// <summary>
    /// Identifierar revisionen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar quote-roten.
    /// </summary>
    public Guid SupplierQuoteId { get; init; }

    /// <summary>
    /// Identifierar supplier-relationen.
    /// </summary>
    public Guid SupplierBusinessRelationshipId { get; init; }

    /// <summary>
    /// Anger revisionsnumret.
    /// </summary>
    public int RevisionNumber { get; init; }

    /// <summary>
    /// Anger revisionens status.
    /// </summary>
    public SupplierQuoteRevisionStatus Status { get; init; }

    /// <summary>
    /// Identifierar revisionens prisvaluta.
    /// </summary>
    public int CurrencyId { get; init; }

    /// <summary>
    /// Identifierar eventuell övergripande Incoterm.
    /// </summary>
    public int? IncotermRuleId { get; init; }

    /// <summary>
    /// Anger eventuell named place.
    /// </summary>
    public string? NamedPlace { get; init; }

    /// <summary>
    /// Anger betalningsvillkor.
    /// </summary>
    public string? PaymentTerms { get; init; }

    /// <summary>
    /// Anger uppgiven ledtid.
    /// </summary>
    public int? LeadTimeDays { get; init; }

    /// <summary>
    /// Anger giltighetens start.
    /// </summary>
    public DateTimeOffset? ValidFromUtc { get; init; }

    /// <summary>
    /// Anger giltighetens slut.
    /// </summary>
    public DateTimeOffset? ValidToUtc { get; init; }

    /// <summary>
    /// Anger när revisionen mottogs och frystes.
    /// </summary>
    public DateTimeOffset? ReceivedAtUtc { get; init; }

    /// <summary>
    /// Refererar supplierns originaldokument.
    /// </summary>
    public string? OriginalDocumentReference { get; init; }

    /// <summary>
    /// Innehåller det immutable snapshotet efter mottagning.
    /// </summary>
    public string? SnapshotJson { get; init; }

    /// <summary>
    /// Innehåller SHA-256 för snapshotet.
    /// </summary>
    public string? SnapshotHash { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Innehåller revisionens offertrader.
    /// </summary>
    public required IReadOnlyList<SupplierQuoteLineResponse> Lines { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}