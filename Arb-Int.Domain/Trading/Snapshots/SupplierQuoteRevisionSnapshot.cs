namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller supplierns immutable kommersiella underlag när en revision tas emot.
/// </summary>
public sealed class SupplierQuoteRevisionSnapshot
{
    /// <summary>
    /// Anger snapshotformatets version.
    /// </summary>
    public int SchemaVersion { get; init; } = 1;

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
    /// Anger quotens namn när revisionen mottogs.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger quotens beskrivning när revisionen mottogs.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger supplierns externa referens.
    /// </summary>
    public string? SupplierReference { get; init; }

    /// <summary>
    /// Anger revisionsnumret.
    /// </summary>
    public int RevisionNumber { get; init; }

    /// <summary>
    /// Identifierar prisvalutan.
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
    /// Anger uppgiven ledtid i dagar.
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
    /// Refererar supplierns originaldokument.
    /// </summary>
    public string? OriginalDocumentReference { get; init; }

    /// <summary>
    /// Innehåller revisionens offertrader i stabil LineNumber-ordning.
    /// </summary>
    public required IReadOnlyList<SupplierQuoteLineSnapshot> Lines { get; init; }
}

/// <summary>
/// Innehåller en immutable supplier-quote-line.
/// </summary>
public sealed class SupplierQuoteLineSnapshot
{
    /// <summary>
    /// Anger radnumret.
    /// </summary>
    public int LineNumber { get; init; }

    /// <summary>
    /// Identifierar eventuell RFQ-line.
    /// </summary>
    public Guid? BuyerRfqLineId { get; init; }

    /// <summary>
    /// Anger radens typ.
    /// </summary>
    public SupplierLineKind Kind { get; init; }

    /// <summary>
    /// Identifierar eventuellt supplier-material-erbjudande.
    /// </summary>
    public Guid? SupplierMaterialOfferingId { get; init; }

    /// <summary>
    /// Identifierar eventuell supplier-processkapacitet.
    /// </summary>
    public Guid? SupplierProcessCapabilityId { get; init; }

    /// <summary>
    /// Identifierar eventuellt supply-solution-step.
    /// </summary>
    public Guid? SupplySolutionStepId { get; init; }

    /// <summary>
    /// Identifierar eventuell inventory lot.
    /// </summary>
    public Guid? InventoryLotId { get; init; }

    /// <summary>
    /// Anger radens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger radens beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger erbjuden kvantitet.
    /// </summary>
    public decimal OfferedQuantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetsenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger pris per enhet.
    /// </summary>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// Anger eventuell fast avgift.
    /// </summary>
    public decimal? FixedFee { get; init; }

    /// <summary>
    /// Anger eventuell minimum order quantity.
    /// </summary>
    public decimal? MinimumOrderQuantity { get; init; }

    /// <summary>
    /// Anger tidigaste tillgänglighet.
    /// </summary>
    public DateTimeOffset? AvailableFromUtc { get; init; }

    /// <summary>
    /// Anger senaste tillgänglighet.
    /// </summary>
    public DateTimeOffset? AvailableToUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell line-specifik Incoterm.
    /// </summary>
    public int? IncotermRuleId { get; init; }

    /// <summary>
    /// Anger eventuell named place.
    /// </summary>
    public string? NamedPlace { get; init; }

    /// <summary>
    /// Anger supplierns prisbas.
    /// </summary>
    public string? PriceBasis { get; init; }

    /// <summary>
    /// Anger supplierns externa radreferens.
    /// </summary>
    public string? SupplierLineReference { get; init; }
}