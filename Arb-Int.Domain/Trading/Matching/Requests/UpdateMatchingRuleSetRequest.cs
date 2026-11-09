namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbar metadata för ett matchningsregelverk.
/// </summary>
public sealed class UpdateMatchingRuleSetRequest
{
    /// <summary>
    /// Visningsnamn för regelverket.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskrivning av regelverkets syfte.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa version.
    /// </summary>
    public uint RowVersion { get; init; }
}