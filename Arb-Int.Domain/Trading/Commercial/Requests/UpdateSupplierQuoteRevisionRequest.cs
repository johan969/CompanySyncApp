namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara villkor för en draft-revision.
/// </summary>
public sealed class UpdateSupplierQuoteRevisionRequest
{
    /// <summary>
    /// Identifierar revisionens valuta.
    /// </summary>
    public int CurrencyId { get; init; }

    /// <summary>
    /// Identifierar eventuell Incoterm.
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
    /// Anger ledtid i dagar.
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
    /// Refererar originaldokumentet.
    /// </summary>
    public string? OriginalDocumentReference { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa revision.
    /// </summary>
    public uint RowVersion { get; init; }
}