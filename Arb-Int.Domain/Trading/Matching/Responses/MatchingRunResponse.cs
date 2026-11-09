namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller status och immutable input för en matchningskörning.
/// </summary>
public sealed class MatchingRunResponse
{
    /// <summary>
    /// Identifierar körningen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar RFQ:n.
    /// </summary>
    public Guid BuyerRfqId { get; init; }

    /// <summary>
    /// Identifierar den släppta RFQ-revisionen.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// RFQ-revisionens nummer.
    /// </summary>
    public int BuyerRfqRevisionNumber { get; init; }

    /// <summary>
    /// Identifierar regelversionen.
    /// </summary>
    public Guid MatchingRuleSetVersionId { get; init; }

    /// <summary>
    /// Identifierar regelverksroten.
    /// </summary>
    public Guid MatchingRuleSetId { get; init; }

    /// <summary>
    /// Regelverkets stabila kod.
    /// </summary>
    public required string MatchingRuleSetCode { get; init; }

    /// <summary>
    /// Regelversionens nummer.
    /// </summary>
    public int MatchingRuleSetVersionNumber { get; init; }

    /// <summary>
    /// Körningens aktuella status.
    /// </summary>
    public MatchingRunStatus Status { get; init; }

    /// <summary>
    /// Tidpunkt då körningen köades.
    /// </summary>
    public DateTimeOffset QueuedAtUtc { get; init; }

    /// <summary>
    /// Tidpunkt då körningen började exekveras.
    /// </summary>
    public DateTimeOffset? StartedAtUtc { get; init; }

    /// <summary>
    /// Tidpunkt då körningen avslutades.
    /// </summary>
    public DateTimeOffset? CompletedAtUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell användare som initierade körningen.
    /// </summary>
    public Guid? StartedByUserId { get; init; }

    /// <summary>
    /// Beskriver triggern som initierade körningen.
    /// </summary>
    public string? TriggerName { get; init; }

    /// <summary>
    /// Korrelations-ID för körningen.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Komplett canonical input-snapshot för körningen.
    /// </summary>
    public string? InputSnapshotJson { get; init; }

    /// <summary>
    /// SHA-256 för input-snapshotet.
    /// </summary>
    public string? InputSnapshotHash { get; init; }

    /// <summary>
    /// Eventuellt felmeddelande för en failed körning.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Frysta RFQ-rader som körningen använder.
    /// </summary>
    public required IReadOnlyList<MatchingRunLineResponse> Lines { get; init; }

    /// <summary>
    /// Antal lösningskandidater som körningen producerat.
    /// </summary>
    public int CandidateCount { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}