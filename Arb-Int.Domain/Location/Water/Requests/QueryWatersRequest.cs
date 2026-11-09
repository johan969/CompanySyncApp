namespace Arb_Int.Domain.Location;

/// <summary>
/// Anger filter för uppslag av vattenområden.
/// </summary>
public sealed class QueryWatersRequest
{
    /// <summary>
    /// Anger fritextsökning mot namn och beskrivning.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Begränsar resultatet till en viss vattentyp.
    /// </summary>
    public WaterType? Type { get; init; }

    /// <summary>
    /// Begränsar resultatet till en viss geografisk vattenfeature.
    /// </summary>
    public WaterFeatureType? FeatureType { get; init; }
}
