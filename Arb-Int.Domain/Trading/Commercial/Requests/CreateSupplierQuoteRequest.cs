namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att skapa en supplier quote och dess första draft-revision.
/// </summary>
public sealed class CreateSupplierQuoteRequest
{
    /// <summary>
    /// Identifierar eventuell RFQ-revision som suppliern svarar på.
    /// </summary>
    public Guid? BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Anger quote-numret.
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
    /// Anger supplierns egen referens.
    /// </summary>
    public string? SupplierReference { get; init; }

    /// <summary>
    /// Identifierar eventuell intern ägare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar på quote-nivå.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Identifierar valutan för revisionens priser.
    /// </summary>
    public int CurrencyId { get; init; }

    /// <summary>
    /// Identifierar eventuell standard-Incoterm.
    /// </summary>
    public int? IncotermRuleId { get; init; }

    /// <summary>
    /// Anger eventuell named place för standard-Incoterm.
    /// </summary>
    public string? NamedPlace { get; init; }

    /// <summary>
    /// Anger supplierns betalningsvillkor.
    /// </summary>
    public string? PaymentTerms { get; init; }

    /// <summary>
    /// Anger supplierns uppgivna ledtid i dagar.
    /// </summary>
    public int? LeadTimeDays { get; init; }

    /// <summary>
    /// Anger när offererten börjar gälla.
    /// </summary>
    public DateTimeOffset? ValidFromUtc { get; init; }

    /// <summary>
    /// Anger när offererten slutar gälla.
    /// </summary>
    public DateTimeOffset? ValidToUtc { get; init; }

    /// <summary>
    /// Refererar eventuellt originaldokumentet som underlaget kommer från.
    /// </summary>
    public string? OriginalDocumentReference { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar för revisionen.
    /// </summary>
    public string? RevisionInternalNotes { get; init; }
}