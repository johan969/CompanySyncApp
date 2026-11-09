namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att konvertera en BuyerDemand till en BuyerRfq.
/// </summary>
public sealed class CreateBuyerRfqFromDemandRequest
{
    /// <summary>
    /// RFQ:ns unika nummer.
    /// </summary>
    public required string RfqNumber { get; init; }

    /// <summary>
    /// Ersätter demandens namn om ett särskilt RFQ-namn önskas.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Ersätter demandens beskrivning om en särskild RFQ-beskrivning önskas.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// RFQ:ns prioritet.
    /// </summary>
    public BuyerRfqPriority Priority { get; init; } = BuyerRfqPriority.Normal;

    /// <summary>
    /// Anger informationens initiala synlighet.
    /// </summary>
    public InformationVisibility Visibility { get; init; } = InformationVisibility.InternalOnly;

    /// <summary>
    /// Köparens eventuella referens.
    /// </summary>
    public string? BuyerReference { get; init; }

    /// <summary>
    /// Identifierar ansvarig användare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Interna anteckningar om RFQ:n.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Sammanfattning för revision 1.
    /// </summary>
    public string? RevisionSummary { get; init; }

    /// <summary>
    /// Köparens ursprungliga RFQ-underlag.
    /// </summary>
    public string? BuyerOriginalText { get; init; }

    /// <summary>
    /// Intern tolkning av köparens underlag.
    /// </summary>
    public string? InternalInterpretation { get; init; }

    /// <summary>
    /// Tidpunkt då svar önskas.
    /// </summary>
    public DateTimeOffset? ResponseRequestedByUtc { get; init; }

    /// <summary>
    /// Identifierar användaren som utför konverteringen.
    /// </summary>
    public Guid? ConvertedByUserId { get; init; }

    /// <summary>
    /// Anteckning som sparas i BuyerDemand state history.
    /// </summary>
    public string? ConversionNotes { get; init; }

    /// <summary>
    /// BuyerDemand-concurrency-token från senast lästa version.
    /// </summary>
    public uint BuyerDemandRowVersion { get; init; }
}