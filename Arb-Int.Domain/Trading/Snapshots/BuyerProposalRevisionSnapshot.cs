namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller det immutable buyer-facing innehållet i en utfärdad proposal-revision.
/// </summary>
public sealed class BuyerProposalRevisionSnapshot
{
    /// <summary>
    /// Anger snapshotformatets version.
    /// </summary>
    public int SchemaVersion { get; init; } = 1;

    /// <summary>
    /// Identifierar buyer-relationen.
    /// </summary>
    public Guid BuyerBusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar buyer-RFQ:n.
    /// </summary>
    public Guid BuyerRfqId { get; init; }

    /// <summary>
    /// Identifierar RFQ-revisionen proposalet svarar mot.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Anger proposal-numret.
    /// </summary>
    public required string ProposalNumber { get; init; }

    /// <summary>
    /// Anger proposalets buyer-facing namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger proposalets buyer-facing beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger revisionsnumret.
    /// </summary>
    public int RevisionNumber { get; init; }

    /// <summary>
    /// Identifierar proposalets valuta.
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
    /// Identifierar eventuell disclosure-revision som hör till erbjudandet.
    /// </summary>
    public Guid? DisclosurePackageRevisionId { get; init; }

    /// <summary>
    /// Binder proposalet till exakt godkänt disclosure-innehåll utan att kopiera innehållet.
    /// </summary>
    public string? DisclosurePackageContentHash { get; init; }

    /// <summary>
    /// Innehåller buyer-facing alternatives.
    /// </summary>
    public required IReadOnlyList<BuyerProposalOptionSnapshot> Options { get; init; }
}