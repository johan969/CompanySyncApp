namespace Arb_Int.Domain.Location;

/// <summary>
/// Representerar full referensdata för ett vattenområde.
/// </summary>
public sealed class WaterResponse
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
    /// Anger beskrivningen.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger överordnat vattenområde när sådant finns.
    /// </summary>
    public Guid? ParentWaterId { get; init; }

    /// <summary>
    /// Anger överordnat vattenområdes namn när sådant finns.
    /// </summary>
    public string? ParentWaterName { get; init; }

    /// <summary>
    /// Anger direkt underordnade vattenområden.
    /// </summary>
    public IReadOnlyList<WaterListResponse> Children { get; init; } = [];

    /// <summary>
    /// Anger länder som är kopplade till vattenområdet.
    /// </summary>
    public IReadOnlyList<CountryListResponse> Countries { get; init; } = [];
}
