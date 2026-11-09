namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller ett versionsstyrt immutable snapshot av det RFQ-underlag som matching ska använda.
/// </summary>
public sealed class BuyerRfqRevisionSnapshot
{
    /// <summary>
    /// Anger snapshot-formatets schema-version.
    /// </summary>
    public int SchemaVersion { get; init; } = 1;

    /// <summary>
    /// Identifierar buyer-relationen som RFQ:n tillhör.
    /// </summary>
    public Guid BuyerBusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar eventuell BuyerDemand som RFQ:n härstammar från.
    /// </summary>
    public Guid? BuyerDemandId { get; init; }

    /// <summary>
    /// RFQ:ns affärsnummer vid release.
    /// </summary>
    public required string RfqNumber { get; init; }

    /// <summary>
    /// RFQ:ns namn vid release.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// RFQ:ns beskrivning vid release.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// RFQ:ns prioritet vid release.
    /// </summary>
    public BuyerRfqPriority Priority { get; init; }

    /// <summary>
    /// Informationssynlighet som gällde vid release.
    /// </summary>
    public InformationVisibility Visibility { get; init; }

    /// <summary>
    /// Köparens eventuella referens.
    /// </summary>
    public string? BuyerReference { get; init; }

    /// <summary>
    /// Revisionens sammanfattning.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Köparens ursprungliga revisionsunderlag.
    /// </summary>
    public string? BuyerOriginalText { get; init; }

    /// <summary>
    /// Intern tolkning som hör till revisionen.
    /// </summary>
    public string? InternalInterpretation { get; init; }

    /// <summary>
    /// Tidpunkt då svar önskas.
    /// </summary>
    public DateTimeOffset? ResponseRequestedByUtc { get; init; }

    /// <summary>
    /// RFQ-rader i stabil radnummerordning.
    /// </summary>
    public required IReadOnlyList<BuyerRfqLineSnapshot> Lines { get; init; }

    /// <summary>
    /// Revisions- och radkrav i stabil ordning.
    /// </summary>
    public required IReadOnlyList<BuyerRfqRequirementSnapshot> Requirements { get; init; }

    /// <summary>
    /// Leveranstillfällen i stabil ordning.
    /// </summary>
    public required IReadOnlyList<BuyerRfqDeliverySnapshot> Deliveries { get; init; }
}