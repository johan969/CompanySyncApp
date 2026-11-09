namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbar metadata för en BuyerRfq.
/// </summary>
public sealed class UpdateBuyerRfqRequest
{
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
    public BuyerRfqPriority Priority { get; init; }

    /// <summary>
    /// Anger informationens synlighet.
    /// </summary>
    public InformationVisibility Visibility { get; init; }

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
    /// Concurrency-token från senast lästa version.
    /// </summary>
    public uint RowVersion { get; init; }
}