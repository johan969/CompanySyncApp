namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att skapa en BuyerRfq med en första draft-revision.
/// </summary>
public sealed class CreateBuyerRfqRequest
{
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
    /// Köparens ursprungliga underlag för revision 1.
    /// </summary>
    public string? BuyerOriginalText { get; init; }

    /// <summary>
    /// Intern tolkning av köparens underlag.
    /// </summary>
    public string? InternalInterpretation { get; init; }

    /// <summary>
    /// Tidpunkt då svar önskas för revision 1.
    /// </summary>
    public DateTimeOffset? ResponseRequestedByUtc { get; init; }
}