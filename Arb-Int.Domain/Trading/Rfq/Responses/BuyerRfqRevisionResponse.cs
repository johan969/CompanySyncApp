namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller metadata och sammanfattning för en RFQ-revision.
/// </summary>
public sealed class BuyerRfqRevisionResponse
{
    /// <summary>
    /// Identifierar revisionen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar RFQ:n som revisionen tillhör.
    /// </summary>
    public Guid BuyerRfqId { get; init; }

    /// <summary>
    /// Revisionens löpnummer inom RFQ:n.
    /// </summary>
    public int RevisionNumber { get; init; }

    /// <summary>
    /// Revisionens aktuella status.
    /// </summary>
    public BuyerRfqRevisionStatus Status { get; init; }

    /// <summary>
    /// Sammanfattar revisionens innehåll eller förändringar.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Innehåller köparens ursprungliga underlag.
    /// </summary>
    public string? BuyerOriginalText { get; init; }

    /// <summary>
    /// Innehåller den interna tolkningen av köparens underlag.
    /// </summary>
    public string? InternalInterpretation { get; init; }

    /// <summary>
    /// Tidpunkt då svar önskas från leverantörer.
    /// </summary>
    public DateTimeOffset? ResponseRequestedByUtc { get; init; }

    /// <summary>
    /// Tidpunkt då revisionen släpptes för matching.
    /// </summary>
    public DateTimeOffset? ReleasedAtUtc { get; init; }

    /// <summary>
    /// Identifierar användaren som släppte revisionen.
    /// </summary>
    public Guid? ReleasedByUserId { get; init; }

    /// <summary>
    /// Tidpunkt då revisionen ersattes av en senare släppt revision.
    /// </summary>
    public DateTimeOffset? SupersededAtUtc { get; init; }

    /// <summary>
    /// Hash för revisionens låsta snapshot.
    /// </summary>
    public string? SnapshotHash { get; init; }

    /// <summary>
    /// Anger om revisionens innehåll fortfarande får redigeras.
    /// </summary>
    public bool IsEditable { get; init; }

    /// <summary>
    /// Antal RFQ-rader i revisionen.
    /// </summary>
    public int LineCount { get; init; }

    /// <summary>
    /// Antal leveranstillfällen i revisionen.
    /// </summary>
    public int DeliveryCount { get; init; }

    /// <summary>
    /// Antal krav i revisionen.
    /// </summary>
    public int RequirementCount { get; init; }

    /// <summary>
    /// Tidpunkt då revisionen skapades.
    /// </summary>
    public DateTimeOffset? CreateDateUtc { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}