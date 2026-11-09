namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller en geografisk origin-koppling för ett supplier material offering.
/// </summary>
public sealed class SupplierMaterialOfferingOriginResponse
{
    /// <summary>
    /// Identifierar origin-kopplingen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar offeringet.
    /// </summary>
    public Guid SupplierMaterialOfferingId { get; init; }

    /// <summary>
    /// Identifierar den geografiska platsen.
    /// </summary>
    public Guid GeographicLocationId { get; init; }

    /// <summary>
    /// Anger vilken typ av origin platsen representerar.
    /// </summary>
    public SupplierOfferingOriginRole Role { get; init; }

    /// <summary>
    /// Anger om platsen är primär inom offeringet och den valda rollen.
    /// </summary>
    public bool IsPrimary { get; init; }

    /// <summary>
    /// Innehåller anteckningar om origin-kopplingen.
    /// </summary>
    public string? Notes { get; init; }
}
