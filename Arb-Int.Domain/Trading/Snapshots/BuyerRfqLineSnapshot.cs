namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller en RFQ-rad i ett immutable snapshot.
/// </summary>
public sealed class BuyerRfqLineSnapshot
{
    /// <summary>
    /// Identifierar eventuell BuyerDemandScope som raden härstammar från.
    /// </summary>
    public Guid? BuyerDemandScopeId { get; init; }

    /// <summary>
    /// Radens stabila nummer inom revisionen.
    /// </summary>
    public int LineNumber { get; init; }

    /// <summary>
    /// Radens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Radens beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Köparens eventuella radreferens.
    /// </summary>
    public string? BuyerLineReference { get; init; }

    /// <summary>
    /// Köparens ursprungliga radtext.
    /// </summary>
    public string? BuyerOriginalText { get; init; }

    /// <summary>
    /// Identifierar eventuellt specifikt material.
    /// </summary>
    public Guid? MaterialId { get; init; }

    /// <summary>
    /// Identifierar eventuell materialtyp.
    /// </summary>
    public int? MaterialTypeId { get; init; }

    /// <summary>
    /// Identifierar eventuell avsedd användningskaraktäristik.
    /// </summary>
    public Guid? IntendedUseCharacteristicId { get; init; }

    /// <summary>
    /// Efterfrågad kvantitet.
    /// </summary>
    public decimal RequiredQuantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetsenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Nedre tolerans i procent.
    /// </summary>
    public decimal? LowerQuantityTolerancePercent { get; init; }

    /// <summary>
    /// Övre tolerans i procent.
    /// </summary>
    public decimal? UpperQuantityTolerancePercent { get; init; }

    /// <summary>
    /// Anger om delvis uppfyllnad tillåts.
    /// </summary>
    public bool AllowPartialFulfillment { get; init; }

    /// <summary>
    /// Anger om flera leverantörer får kombineras.
    /// </summary>
    public bool AllowMultipleSuppliers { get; init; }

    /// <summary>
    /// Anger om flera ursprung får kombineras.
    /// </summary>
    public bool AllowMultipleOrigins { get; init; }

    /// <summary>
    /// Anger om flera lots får kombineras.
    /// </summary>
    public bool AllowMultipleLots { get; init; }

    /// <summary>
    /// Anger om exakt en leverantör krävs.
    /// </summary>
    public bool RequireSingleSupplier { get; init; }

    /// <summary>
    /// Anger om exakt ett ursprung krävs.
    /// </summary>
    public bool RequireSingleOrigin { get; init; }

    /// <summary>
    /// Anger om exakt en lot krävs.
    /// </summary>
    public bool RequireSingleLot { get; init; }

    /// <summary>
    /// Anger om uniform certifiering krävs.
    /// </summary>
    public bool RequireUniformCertification { get; init; }

    /// <summary>
    /// Anger om uniform specifikation krävs.
    /// </summary>
    public bool RequireUniformSpecification { get; init; }

    /// <summary>
    /// Anger maximalt antal leverantörer.
    /// </summary>
    public int? MaximumSupplierCount { get; init; }

    /// <summary>
    /// Interna anteckningar som ingår i revisionssnapshotet.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Materialalternativ i stabil ordning.
    /// </summary>
    public required IReadOnlyList<BuyerRfqLineMaterialOptionSnapshot> MaterialOptions { get; init; }
}