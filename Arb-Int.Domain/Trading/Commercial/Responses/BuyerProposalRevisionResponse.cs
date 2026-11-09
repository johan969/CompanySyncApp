namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en buyer-proposal-revision med options och immutable issue-snapshot.
/// </summary>
public sealed class BuyerProposalRevisionResponse
{
    /// <summary>
    /// Identifierar revisionen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar proposal-roten.
    /// </summary>
    public Guid BuyerProposalId { get; init; }

    /// <summary>
    /// Identifierar RFQ-roten.
    /// </summary>
    public Guid BuyerRfqId { get; init; }

    /// <summary>
    /// Identifierar exakt RFQ-revision som proposalet bygger på.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Anger revisionsnumret.
    /// </summary>
    public int RevisionNumber { get; init; }

    /// <summary>
    /// Anger revisionsstatus.
    /// </summary>
    public BuyerProposalRevisionStatus Status { get; init; }

    /// <summary>
    /// Identifierar proposal-valutan.
    /// </summary>
    public int CurrencyId { get; init; }

    /// <summary>
    /// Anger giltighetens start.
    /// </summary>
    public DateTimeOffset? ValidFromUtc { get; init; }

    /// <summary>
    /// Anger giltighetens slut.
    /// </summary>
    public DateTimeOffset? ValidToUtc { get; init; }

    /// <summary>
    /// Innehåller buyer-facing kommersiella villkor.
    /// </summary>
    public string? CommercialTerms { get; init; }

    /// <summary>
    /// Innehåller buyer-facing anteckningar.
    /// </summary>
    public string? BuyerFacingNotes { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Identifierar eventuell disclosure-package-revision.
    /// </summary>
    public Guid? DisclosurePackageRevisionId { get; init; }

    /// <summary>
    /// Anger när revisionen utfärdades.
    /// </summary>
    public DateTimeOffset? IssuedAtUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell användare som utfärdade revisionen.
    /// </summary>
    public Guid? IssuedByUserId { get; init; }

    /// <summary>
    /// Innehåller immutable buyer-facing snapshot.
    /// </summary>
    public string? SnapshotJson { get; init; }

    /// <summary>
    /// Innehåller SHA-256 för snapshotet.
    /// </summary>
    public string? SnapshotHash { get; init; }

    /// <summary>
    /// Innehåller revisionens buyer-facing alternatives.
    /// </summary>
    public required IReadOnlyList<BuyerProposalOptionResponse> Options { get; init; }

    /// <summary>
    /// Innehåller buyer acceptances registrerade mot denna proposal-revision.
    /// </summary>
    public required IReadOnlyList<BuyerProposalAcceptanceResponse> Acceptances { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}