namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara värden för en supplier-quote-line i en Draft-revision.
/// </summary>
public sealed class UpdateSupplierQuoteLineRequest
{
    /// <summary>
    /// Identifierar eventuell RFQ-line som offertraden svarar mot.
    /// </summary>
    public Guid? BuyerRfqLineId { get; init; }

    /// <summary>
    /// Anger radens kommersiella typ.
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
    /// Anger radnumret inom revisionen.
    /// </summary>
    public int LineNumber { get; init; }

    /// <summary>
    /// Anger radens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver supplierns erbjudande.
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
    /// Anger pris per enhet i revisionens valuta.
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
    /// Anger eventuell line-specifik named place.
    /// </summary>
    public string? NamedPlace { get; init; }

    /// <summary>
    /// Beskriver supplierns prisbas.
    /// </summary>
    public string? PriceBasis { get; init; }

    /// <summary>
    /// Anger supplierns externa radreferens.
    /// </summary>
    public string? SupplierLineReference { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa line.
    /// </summary>
    public uint RowVersion { get; init; }
}