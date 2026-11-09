namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en supplier-offertrad i en draft-revision.
/// </summary>
public sealed class CreateSupplierQuoteLineRequest
{
    /// <summary>
    /// Identifierar eventuell RFQ-line som raden svarar mot.
    /// </summary>
    public Guid? BuyerRfqLineId { get; init; }

    /// <summary>
    /// Anger radens kommersiella typ.
    /// </summary>
    public SupplierLineKind Kind { get; init; }

    /// <summary>
    /// Identifierar eventuellt supplier material offering.
    /// </summary>
    public Guid? SupplierMaterialOfferingId { get; init; }

    /// <summary>
    /// Identifierar eventuell supplier process capability.
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
    /// Anger eventuell fast avgift i revisionens valuta.
    /// </summary>
    public decimal? FixedFee { get; init; }

    /// <summary>
    /// Anger eventuell minsta orderkvantitet.
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
    /// Beskriver prisbasen, exempelvis per kg eller per shipment.
    /// </summary>
    public string? PriceBasis { get; init; }

    /// <summary>
    /// Anger supplierns referens för raden.
    /// </summary>
    public string? SupplierLineReference { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }
}