namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller detaljerad information om en BuyerRfq och dess revisioner.
/// </summary>
public sealed class BuyerRfqResponse
{
    /// <summary>
    /// Identifierar RFQ:n.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar buyer-relationen.
    /// </summary>
    public Guid BuyerBusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar köparen.
    /// </summary>
    public Guid BuyerPartyId { get; init; }

    /// <summary>
    /// Köparens visningsnamn.
    /// </summary>
    public required string BuyerName { get; init; }

    /// <summary>
    /// Identifierar eventuell BuyerDemand som RFQ:n härstammar från.
    /// </summary>
    public Guid? BuyerDemandId { get; init; }

    /// <summary>
    /// RFQ:ns unika nummer.
    /// </summary>
    public required string RfqNumber { get; init; }

    /// <summary>
    /// RFQ:ns namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// RFQ:ns övergripande beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// RFQ:ns aktuella status.
    /// </summary>
    public BuyerRfqStatus Status { get; init; }

    /// <summary>
    /// RFQ:ns prioritet.
    /// </summary>
    public BuyerRfqPriority Priority { get; init; }

    /// <summary>
    /// Anger informationens synlighet.
    /// </summary>
    public InformationVisibility Visibility { get; init; }

    /// <summary>
    /// Köparens egen referens.
    /// </summary>
    public string? BuyerReference { get; init; }

    /// <summary>
    /// Identifierar ansvarig användare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Senaste revisionsnummer.
    /// </summary>
    public int LatestRevisionNumber { get; init; }

    /// <summary>
    /// Tidpunkt då RFQ:n först blev aktiv.
    /// </summary>
    public DateTimeOffset? OpenedAtUtc { get; init; }

    /// <summary>
    /// Tidpunkt då RFQ:n stängdes eller avbröts.
    /// </summary>
    public DateTimeOffset? ClosedAtUtc { get; init; }

    /// <summary>
    /// Interna anteckningar om RFQ:n.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Tidpunkt då RFQ:n skapades.
    /// </summary>
    public DateTimeOffset? CreateDateUtc { get; init; }

    /// <summary>
    /// RFQ:ns revisioner i fallande revisionsordning.
    /// </summary>
    public required IReadOnlyList<BuyerRfqRevisionResponse> Revisions { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}