namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en matchningsbar rad i en RFQ-revision.
/// </summary>
public sealed class BuyerRfqLineResponse
{
    /// <summary>
    /// Identifierar RFQ-raden.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar revisionen.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Identifierar RFQ:n.
    /// </summary>
    public Guid BuyerRfqId { get; init; }

    /// <summary>
    /// Revisionens aktuella status.
    /// </summary>
    public BuyerRfqRevisionStatus RevisionStatus { get; init; }

    /// <summary>
    /// Identifierar eventuell BuyerDemandScope som raden härstammar från.
    /// </summary>
    public Guid? BuyerDemandScopeId { get; init; }

    /// <summary>
    /// Radens nummer inom revisionen.
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
    /// Köparens ursprungliga text för raden.
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
    /// Anger efterfrågad mängd.
    /// </summary>
    public decimal RequiredQuantity { get; init; }

    /// <summary>
    /// Identifierar mängdenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger tillåten nedre kvantitetstolerans i procent.
    /// </summary>
    public decimal? LowerQuantityTolerancePercent { get; init; }

    /// <summary>
    /// Anger tillåten övre kvantitetstolerans i procent.
    /// </summary>
    public decimal? UpperQuantityTolerancePercent { get; init; }

    /// <summary>
    /// Anger om raden får uppfyllas delvis.
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
    /// Anger om samma certifiering krävs för hela lösningen.
    /// </summary>
    public bool RequireUniformCertification { get; init; }

    /// <summary>
    /// Anger om samma specifikation krävs för hela lösningen.
    /// </summary>
    public bool RequireUniformSpecification { get; init; }

    /// <summary>
    /// Anger eventuellt maximalt antal leverantörer.
    /// </summary>
    public int? MaximumSupplierCount { get; init; }

    /// <summary>
    /// Interna anteckningar om raden.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Materialalternativ för raden.
    /// </summary>
    public required IReadOnlyList<BuyerRfqLineMaterialOptionResponse> MaterialOptions { get; init; }

    /// <summary>
    /// Antal krav som är direkt kopplade till raden.
    /// </summary>
    public int RequirementCount { get; init; }

    /// <summary>
    /// Antal leveransallokeringar som använder raden.
    /// </summary>
    public int DeliveryAllocationCount { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}