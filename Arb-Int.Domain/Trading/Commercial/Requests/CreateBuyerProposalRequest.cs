namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att skapa ett buyer proposal tillsammans med revision 1.
/// </summary>
public sealed class CreateBuyerProposalRequest
{
    /// <summary>
    /// Identifierar den ReleasedForMatching RFQ-revision som proposalet baseras på.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Anger det stabila proposal-numret.
    /// </summary>
    public required string ProposalNumber { get; init; }

    /// <summary>
    /// Anger proposalets namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver proposalet.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Identifierar eventuell intern ägare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar på root-nivå.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Identifierar proposal-revisionens valuta.
    /// </summary>
    public int CurrencyId { get; init; }

    /// <summary>
    /// Anger när proposalet börjar vara giltigt.
    /// </summary>
    public DateTimeOffset? ValidFromUtc { get; init; }

    /// <summary>
    /// Anger när proposalet slutar vara giltigt.
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
    /// Innehåller interna anteckningar för revisionen.
    /// </summary>
    public string? RevisionInternalNotes { get; init; }

    /// <summary>
    /// Identifierar eventuell disclosure-revision som ska följa proposalet.
    /// </summary>
    public Guid? DisclosurePackageRevisionId { get; init; }
}