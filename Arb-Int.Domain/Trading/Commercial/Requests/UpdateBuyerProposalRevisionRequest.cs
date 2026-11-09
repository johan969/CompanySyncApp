namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara villkor för en Draft proposal-revision.
/// </summary>
public sealed class UpdateBuyerProposalRevisionRequest
{
    /// <summary>
    /// Identifierar proposalets buyer-facing valuta.
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
    /// Identifierar eventuell disclosure-revision.
    /// </summary>
    public Guid? DisclosurePackageRevisionId { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa revision.
    /// </summary>
    public uint RowVersion { get; init; }
}