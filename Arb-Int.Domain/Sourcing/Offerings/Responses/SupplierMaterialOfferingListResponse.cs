namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller kompakt information om ett supplier material offering i en lista.
/// </summary>
public sealed class SupplierMaterialOfferingListResponse
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
    /// Anger motpartens visningsnamn. Behövs när listan sträcker sig över flera relationer.
    /// </summary>
    public string? SupplierName { get; init; }

    /// <summary>
    /// Anger offeringens stabila nyckel inom supplier-relationen.
    /// </summary>
    public required string OfferingKey { get; init; }

    /// <summary>
    /// Anger offeringens visningsnamn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger leverantörens eget artikelnummer när det är känt.
    /// </summary>
    public string? SupplierSku { get; init; }

    /// <summary>
    /// Anger formatet så som det deklarerades i källan.
    /// </summary>
    public string? DeclaredFormat { get; init; }

    /// <summary>
    /// Identifierar normaliserad materialtyp när den är känd.
    /// </summary>
    public int? OfferedMaterialTypeId { get; init; }

    /// <summary>
    /// Anger den erbjudna materialtypens visningsnamn.
    /// </summary>
    public string? OfferedMaterialTypeName { get; init; }

    /// <summary>
    /// Identifierar mappat kanoniskt material när det är känt.
    /// </summary>
    public Guid? MappedMaterialId { get; init; }

    /// <summary>
    /// Anger det mappade kanoniska materialets visningsnamn.
    /// </summary>
    public string? MappedMaterialName { get; init; }

    /// <summary>
    /// Anger materialmappingens status.
    /// </summary>
    public SourcingMappingStatus MappingStatus { get; init; }

    /// <summary>
    /// Anger mappingens confidence mellan 0 och 1.
    /// </summary>
    public decimal? MappingConfidence { get; init; }

    /// <summary>
    /// Anger offeringens aktuella status.
    /// </summary>
    public SupplierOfferingStatus Status { get; init; }

    /// <summary>
    /// Anger senaste kända observationen av offeringet.
    /// </summary>
    public DateTimeOffset LastObservedUtc { get; init; }

    /// <summary>
    /// Anger senaste verifieringen när sådan finns.
    /// </summary>
    public DateTimeOffset? LastVerifiedUtc { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}
