namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara värden för en opublicerad regelversion.
/// </summary>
public sealed class UpdateMatchingRuleSetVersionRequest
{
    /// <summary>
    /// Beskrivning av versionsändringen.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// JSON-konfiguration som används av matchningsmotorn.
    /// </summary>
    public required string ConfigurationJson { get; init; }

    /// <summary>
    /// Tidpunkt då versionen tidigast får användas.
    /// </summary>
    public DateTimeOffset? EffectiveFromUtc { get; init; }

    /// <summary>
    /// Tidpunkt då versionen slutar vara effektiv.
    /// </summary>
    public DateTimeOffset? EffectiveToUtc { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa version.
    /// </summary>
    public uint RowVersion { get; init; }
}