using System.Text.Json;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller hela reproducerbara inputen för en matchningskörning.
/// </summary>
public sealed class MatchingRunInputSnapshot
{
    /// <summary>
    /// Version för run-inputformatet.
    /// </summary>
    public int SchemaVersion { get; init; } = 1;

    /// <summary>
    /// Identifierar RFQ-revisionen.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// SHA-256 för RFQ-revisionens snapshot.
    /// </summary>
    public required string BuyerRfqSnapshotHash { get; init; }

    /// <summary>
    /// RFQ-revisionens immutable snapshot.
    /// </summary>
    public JsonElement BuyerRfqSnapshot { get; init; }

    /// <summary>
    /// Identifierar regelverksroten.
    /// </summary>
    public Guid MatchingRuleSetId { get; init; }

    /// <summary>
    /// Regelverkets stabila kod.
    /// </summary>
    public required string MatchingRuleSetCode { get; init; }

    /// <summary>
    /// Identifierar exakt regelversion.
    /// </summary>
    public Guid MatchingRuleSetVersionId { get; init; }

    /// <summary>
    /// Regelversionens nummer.
    /// </summary>
    public int MatchingRuleSetVersionNumber { get; init; }

    /// <summary>
    /// SHA-256 för regelkonfigurationen.
    /// </summary>
    public required string MatchingConfigurationHash { get; init; }

    /// <summary>
    /// Immutable regelkonfiguration.
    /// </summary>
    public JsonElement MatchingConfiguration { get; init; }
}