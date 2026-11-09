namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller ett komplett supplier material offering med offering-ägda child-data.
/// </summary>
public sealed class SupplierMaterialOfferingResponse
{
    /// <summary>
    /// Identifierar offeringet.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar supplier-relationen.
    /// </summary>
    public Guid BusinessRelationshipId { get; init; }

    /// <summary>
    /// Anger offeringens stabila nyckel inom supplier-relationen.
    /// </summary>
    public required string OfferingKey { get; init; }

    /// <summary>
    /// Anger offeringens visningsnamn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver erbjudandet.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger leverantörens eget artikelnummer när det är känt.
    /// </summary>
    public string? SupplierSku { get; init; }

    /// <summary>
    /// Anger formatet så som det deklarerades i källan.
    /// </summary>
    public string? DeclaredFormat { get; init; }

    /// <summary>
    /// Identifierar normaliserad terminal materialtyp när den är känd.
    /// </summary>
    public int? OfferedMaterialTypeId { get; init; }

    /// <summary>
    /// Identifierar mappat kanoniskt material när det är känt.
    /// </summary>
    public Guid? MappedMaterialId { get; init; }

    /// <summary>
    /// Anger materialmappingens status.
    /// </summary>
    public SourcingMappingStatus MappingStatus { get; init; }

    /// <summary>
    /// Anger mappingens confidence mellan 0 och 1.
    /// </summary>
    public decimal? MappingConfidence { get; init; }

    /// <summary>
    /// Innehåller anteckningar om mappingen.
    /// </summary>
    public string? MappingNotes { get; init; }

    /// <summary>
    /// Anger offeringens aktuella status.
    /// </summary>
    public SupplierOfferingStatus Status { get; init; }

    /// <summary>
    /// Anger när offeringet först observerades.
    /// </summary>
    public DateTimeOffset FirstObservedUtc { get; init; }

    /// <summary>
    /// Anger när offeringet senast observerades.
    /// </summary>
    public DateTimeOffset LastObservedUtc { get; init; }

    /// <summary>
    /// Anger när offeringet senast verifierades.
    /// </summary>
    public DateTimeOffset? LastVerifiedUtc { get; init; }

    /// <summary>
    /// Anger när offeringet skapades i systemet.
    /// </summary>
    public DateTimeOffset? CreateDateUtc { get; init; }

    /// <summary>
    /// Innehåller biologiska source-claims som offeringet bygger på.
    /// </summary>
    public required IReadOnlyList<SupplierMaterialOfferingBiologicalSourceResponse> BiologicalSources { get; init; }

    /// <summary>
    /// Innehåller deklarerade eller normaliserade geografiska ursprung.
    /// </summary>
    public required IReadOnlyList<SupplierMaterialOfferingOriginResponse> Origins { get; init; }

    /// <summary>
    /// Innehåller characteristics som taggar offeringet.
    /// </summary>
    public required IReadOnlyList<SupplierMaterialOfferingCharacteristicResponse> Characteristics { get; init; }

    /// <summary>
    /// Innehåller evidens för att offeringet finns eller har observerats.
    /// </summary>
    public required IReadOnlyList<SupplierMaterialOfferingEvidenceResponse> Evidence { get; init; }

    /// <summary>
    /// Innehåller känd eller indikativ availability för offeringet.
    /// </summary>
    public required IReadOnlyList<SupplierOfferingAvailabilityResponse> Availabilities { get; init; }

    /// <summary>
    /// Innehåller mätbara eller semantiska specifications för offeringet.
    /// </summary>
    public required IReadOnlyList<SupplierOfferingSpecificationResponse> Specifications { get; init; }

    /// <summary>
    /// Concurrency-token för offering-aggregatet.
    /// </summary>
    public uint RowVersion { get; init; }
}
