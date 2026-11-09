namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller kompakt information om en BuyerRfq för listor och sökresultat.
/// </summary>
public sealed class BuyerRfqListResponse
{
    /// <summary>
    /// Identifierar RFQ:n.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// RFQ:ns unika nummer.
    /// </summary>
    public required string RfqNumber { get; init; }

    /// <summary>
    /// RFQ:ns namn.
    /// </summary>
    public required string Name { get; init; }

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
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}