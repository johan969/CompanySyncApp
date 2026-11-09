using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Anger filtrering, sortering och paginering för supplier material offerings.
/// </summary>
public sealed class SupplierMaterialOfferingQueryRequest
{
    /// <summary>
    /// Fritext som matchas mot offering key, namn, supplier SKU, deklarerat format,
    /// mappat material, materialtyp och kopplade biologiska källor.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Begränsar resultatet till en offering-status.
    /// </summary>
    public SupplierOfferingStatus? Status { get; init; }

    /// <summary>
    /// Begränsar resultatet till en mapping-status.
    /// </summary>
    public SourcingMappingStatus? MappingStatus { get; init; }

    /// <summary>
    /// Begränsar resultatet till ett mappat kanoniskt material.
    /// </summary>
    public Guid? MappedMaterialId { get; init; }

    /// <summary>
    /// Begränsar resultatet till en normaliserad materialtyp.
    /// </summary>
    public int? OfferedMaterialTypeId { get; init; }

    /// <summary>
    /// Begränsar resultatet till offerings som är kopplade till en normaliserad
    /// biologisk källa via en claim på samma affärsrelation.
    /// <para>Matchningen är exakt. En claim mappad till genusnivå matchar inte
    /// ett artfilter och omvänt.</para>
    /// </summary>
    public Guid? BiologicalSourceId { get; init; }

    /// <summary>
    /// Begränsar en organisationsvid sökning till en enskild affärsrelation.
    /// <para>Ignoreras när sökningen redan är scopad till en relation via routen.</para>
    /// </summary>
    public Guid? BusinessRelationshipId { get; init; }

    /// <summary>
    /// Anger sorteringsfält.
    /// </summary>
    public SupplierMaterialOfferingSortField SortBy { get; init; } = SupplierMaterialOfferingSortField.Name;

    /// <summary>
    /// Anger sorteringsriktning.
    /// </summary>
    public SortDirection SortDirection { get; init; } = SortDirection.Ascending;

    /// <summary>
    /// Sidnummer med start på 1.
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Maximalt antal poster per sida.
    /// </summary>
    public int PageSize { get; init; } = 50;
}

/// <summary>
/// Anger tillgängliga sorteringsfält för supplier material offerings.
/// </summary>
public enum SupplierMaterialOfferingSortField
{
    /// <summary>
    /// Sorterar på offeringens namn.
    /// </summary>
    Name = 0,

    /// <summary>
    /// Sorterar på offeringens status.
    /// </summary>
    Status = 1,

    /// <summary>
    /// Sorterar på mapping-status.
    /// </summary>
    MappingStatus = 2,

    /// <summary>
    /// Sorterar på första observation.
    /// </summary>
    FirstObservedUtc = 3,

    /// <summary>
    /// Sorterar på senaste observation.
    /// </summary>
    LastObservedUtc = 4,

    /// <summary>
    /// Sorterar på senaste verifiering.
    /// </summary>
    LastVerifiedUtc = 5,

    /// <summary>
    /// Sorterar på skapandetid.
    /// </summary>
    CreateDateUtc = 6
}
