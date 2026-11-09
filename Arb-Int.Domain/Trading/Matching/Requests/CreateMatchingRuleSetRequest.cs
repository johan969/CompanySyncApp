namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att skapa ett matchningsregelverk och dess första draft-version.
/// </summary>
public sealed class CreateMatchingRuleSetRequest
{
    /// <summary>
    /// Stabil maskinläsbar kod för regelverket.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Visningsnamn för regelverket.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskrivning av regelverkets syfte.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Beskrivning av den första konfigurationsversionen.
    /// </summary>
    public string? InitialVersionDescription { get; init; }

    /// <summary>
    /// JSON-konfiguration för version 1.
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
}