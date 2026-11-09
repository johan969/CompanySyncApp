namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller en characteristic-koppling för ett supplier material offering.
/// </summary>
public sealed class SupplierMaterialOfferingCharacteristicResponse
{
    /// <summary>
    /// Identifierar kopplingen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar offeringet.
    /// </summary>
    public Guid SupplierMaterialOfferingId { get; init; }

    /// <summary>
    /// Identifierar characteristic som kopplats till offeringet.
    /// </summary>
    public Guid CharacteristicId { get; init; }
}
