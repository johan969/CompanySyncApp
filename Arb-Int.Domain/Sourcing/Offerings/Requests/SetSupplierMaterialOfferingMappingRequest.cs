namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller den önskade materialmappningen för ett supplier material offering.
/// </summary>
public sealed class SetSupplierMaterialOfferingMappingRequest
{
    /// <summary>
    /// Identifierar normaliserad terminal materialtyp när den är känd.
    /// </summary>
    public int? OfferedMaterialTypeId { get; init; }

    /// <summary>
    /// Identifierar det kanoniska Material som offeringet mappas till.
    /// </summary>
    public Guid? MappedMaterialId { get; init; }

    /// <summary>
    /// Anger mappingens aktuella status.
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
    /// Concurrency-token från senast lästa offering.
    /// </summary>
    public uint RowVersion { get; init; }
}
