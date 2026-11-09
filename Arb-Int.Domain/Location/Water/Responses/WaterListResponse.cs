namespace Arb_Int.Domain.Location;

/// <summary>
/// Representerar ett vattenområde i listor och uppslagsfält.
/// </summary>
public sealed class WaterListResponse
{
    /// <summary>
    /// Anger vattenområdets id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Anger vattenområdets namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger vattenområdets vattentyp.
    /// </summary>
    public WaterType Type { get; init; }

    /// <summary>
    /// Anger vattenområdets geografiska featuretyp.
    /// </summary>
    public WaterFeatureType FeatureType { get; init; }

    /// <summary>
    /// Anger överordnat vattenområde när sådant finns.
    /// </summary>
    public Guid? ParentWaterId { get; init; }

    /// <summary>
    /// Anger överordnat vattenområdes namn när sådant finns.
    /// </summary>
    public string? ParentWaterName { get; init; }
}
