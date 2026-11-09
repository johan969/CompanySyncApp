namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett matchningsregelverk och dess versionshistorik.
/// </summary>
public sealed class MatchingRuleSetResponse
{
    /// <summary>
    /// Identifierar regelverket.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Stabil maskinläsbar kod.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Regelverkets visningsnamn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Regelverkets beskrivning.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Regelverkets livscykelstatus.
    /// </summary>
    public MatchingRuleSetStatus Status { get; init; }

    /// <summary>
    /// Högsta skapade versionsnummer.
    /// </summary>
    public int LatestVersionNumber { get; init; }

    /// <summary>
    /// Versionshistoriken med senaste version först.
    /// </summary>
    public required IReadOnlyList<MatchingRuleSetVersionResponse> Versions { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}