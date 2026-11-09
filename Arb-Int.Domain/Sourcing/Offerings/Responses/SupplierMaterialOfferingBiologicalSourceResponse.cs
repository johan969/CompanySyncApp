namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller en biologisk source-koppling för ett supplier material offering.
/// </summary>
public sealed class SupplierMaterialOfferingBiologicalSourceResponse
{
    /// <summary>
    /// Identifierar länken.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar supplier-relationen som både offering och claim tillhör.
    /// </summary>
    public Guid BusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar offeringet.
    /// </summary>
    public Guid SupplierMaterialOfferingId { get; init; }

    /// <summary>
    /// Identifierar det biologiska CRM-claim som länkas.
    /// </summary>
    public Guid BusinessRelationshipBiologicalSourceClaimId { get; init; }

    /// <summary>
    /// Anger den biologiska källans roll i offeringet.
    /// </summary>
    public SupplierOfferingBiologicalSourceRole Role { get; init; }

    /// <summary>
    /// Anger deklarerad andel i procent när offeringet är en blandning.
    /// </summary>
    public decimal? ProportionPercent { get; init; }

    /// <summary>
    /// Innehåller anteckningar om kopplingen.
    /// </summary>
    public string? Notes { get; init; }
}
