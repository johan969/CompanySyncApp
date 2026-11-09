namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara värden för ett leveranstillfälle i en draft-revision.
/// </summary>
public sealed class UpdateBuyerRfqDeliveryRequest
{
    /// <summary>
    /// Leveranstillfällets nummer inom revisionen.
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
    /// Tidigaste accepterade leveranstid.
    /// </summary>
    public DateTimeOffset RequestedDeliveryFromUtc { get; init; }

    /// <summary>
    /// Senaste accepterade leveranstid.
    /// </summary>
    public DateTimeOffset RequestedDeliveryToUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell Incoterm-regel.
    /// </summary>
    public int? IncotermRuleId { get; init; }

    /// <summary>
    /// Innehåller eventuell namngiven plats för leveransvillkoret.
    /// </summary>
    public string? NamedPlace { get; init; }

    /// <summary>
    /// Anger eventuell lägsta accepterade temperatur i Celsius.
    /// </summary>
    public decimal? MinimumTemperatureCelsius { get; init; }

    /// <summary>
    /// Anger eventuell högsta accepterade temperatur i Celsius.
    /// </summary>
    public decimal? MaximumTemperatureCelsius { get; init; }

    /// <summary>
    /// Innehåller krav eller anteckningar om emballage.
    /// </summary>
    public string? PackagingNotes { get; init; }

    /// <summary>
    /// Innehåller övriga anteckningar om leveransen.
    /// </summary>
    public string? DeliveryNotes { get; init; }

    /// <summary>
    /// Anger leveranstillfällets sorteringsordning.
    /// </summary>
    public int SortOrder { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa version.
    /// </summary>
    public uint RowVersion { get; init; }
}