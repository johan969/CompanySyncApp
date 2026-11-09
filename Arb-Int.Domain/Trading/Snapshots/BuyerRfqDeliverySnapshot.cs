namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller ett leveranstillfälle i ett RFQ-snapshot.
/// </summary>
public sealed class BuyerRfqDeliverySnapshot
{
    /// <summary>
    /// Leveranstillfällets stabila nummer.
    /// </summary>
    public int DeliveryNumber { get; init; }

    /// <summary>
    /// Identifierar eventuell destinationsadress.
    /// </summary>
    public Guid? DestinationAddressId { get; init; }

    /// <summary>
    /// Identifierar eventuell geografisk destination.
    /// </summary>
    public Guid? DestinationLocationId { get; init; }

    /// <summary>
    /// Tidigaste leveranstid.
    /// </summary>
    public DateTimeOffset RequestedDeliveryFromUtc { get; init; }

    /// <summary>
    /// Senaste leveranstid.
    /// </summary>
    public DateTimeOffset RequestedDeliveryToUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell Incoterm-regel.
    /// </summary>
    public int? IncotermRuleId { get; init; }

    /// <summary>
    /// Namngiven plats för leveransvillkoret.
    /// </summary>
    public string? NamedPlace { get; init; }

    /// <summary>
    /// Anger eventuell lägsta temperatur.
    /// </summary>
    public decimal? MinimumTemperatureCelsius { get; init; }

    /// <summary>
    /// Anger eventuell högsta temperatur.
    /// </summary>
    public decimal? MaximumTemperatureCelsius { get; init; }

    /// <summary>
    /// Innehåller emballagekrav.
    /// </summary>
    public string? PackagingNotes { get; init; }

    /// <summary>
    /// Innehåller övriga leveransanteckningar.
    /// </summary>
    public string? DeliveryNotes { get; init; }

    /// <summary>
    /// Anger sorteringsordningen.
    /// </summary>
    public int SortOrder { get; init; }

    /// <summary>
    /// Radallokeringar i stabil radnummerordning.
    /// </summary>
    public required IReadOnlyList<BuyerRfqDeliveryLineAllocationSnapshot> LineAllocations { get; init; }
}