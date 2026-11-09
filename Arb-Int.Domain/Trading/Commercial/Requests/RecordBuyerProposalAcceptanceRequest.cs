namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller buyer acceptance av hela eller delar av ett erbjudet proposal-option.
/// </summary>
public sealed class RecordBuyerProposalAcceptanceRequest
{
    /// <summary>
    /// Anger eventuell extern referens från köparens acceptans.
    /// </summary>
    public string? BuyerAcceptanceReference { get; init; }

    /// <summary>
    /// Anger när köparen faktiskt accepterade erbjudandet.
    /// </summary>
    public DateTimeOffset AcceptedAtUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell användare som registrerade buyer acceptance.
    /// </summary>
    public Guid? RecordedByUserId { get; init; }

    /// <summary>
    /// Innehåller de proposal-lines och kvantiteter som köparen accepterade.
    /// </summary>
    public required IReadOnlyList<RecordBuyerProposalAcceptanceLineRequest> Lines { get; init; }

    /// <summary>
    /// Concurrency-token för proposal-roten.
    /// </summary>
    public uint BuyerProposalRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för proposal-revisionen.
    /// </summary>
    public uint BuyerProposalRevisionRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för proposal-optionen.
    /// </summary>
    public uint BuyerProposalOptionRowVersion { get; init; }
}