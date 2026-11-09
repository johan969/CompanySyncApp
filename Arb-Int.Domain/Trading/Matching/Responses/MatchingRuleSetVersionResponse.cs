namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en versionslåst matchningskonfiguration.
/// </summary>
public sealed class MatchingRuleSetVersionResponse
{
    /// <summary>
    /// Identifierar regelversionen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar regelverksroten.
    /// </summary>
    public Guid MatchingRuleSetId { get; init; }

    /// <summary>
    /// Versionens löpnummer.
    /// </summary>
    public int VersionNumber { get; init; }

    /// <summary>
    /// Beskrivning av versionens syfte eller ändring.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Canonical JSON-konfiguration.
    /// </summary>
    public required string ConfigurationJson { get; init; }

    /// <summary>
    /// SHA-256-hash för konfigurationen.
    /// </summary>
    public required string ConfigurationHash { get; init; }

    /// <summary>
    /// Tidpunkt då versionen tidigast är effektiv.
    /// </summary>
    public DateTimeOffset? EffectiveFromUtc { get; init; }

    /// <summary>
    /// Tidpunkt då versionen slutar vara effektiv.
    /// </summary>
    public DateTimeOffset? EffectiveToUtc { get; init; }

    /// <summary>
    /// Tidpunkt då versionen publicerades.
    /// </summary>
    public DateTimeOffset? PublishedAtUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell användare som publicerade versionen.
    /// </summary>
    public Guid? PublishedByUserId { get; init; }

    /// <summary>
    /// Antal matchningskörningar som använder exakt denna version.
    /// </summary>
    public int MatchingRunCount { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}