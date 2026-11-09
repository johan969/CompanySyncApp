namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller tidsbegränsad och kvantifierad availability för ett supplier offering.
/// </summary>
public sealed class SupplierOfferingAvailabilityResponse
{
    /// <summary>
    /// Identifierar availability-posten.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar offeringet.
    /// </summary>
    public Guid SupplierMaterialOfferingId { get; init; }

    /// <summary>
    /// Identifierar konkret inventory lot när availability gäller en viss lot.
    /// </summary>
    public Guid? InventoryLotId { get; init; }

    /// <summary>
    /// Anger availability-status.
    /// </summary>
    public SupplyAvailabilityStatus Status { get; init; }

    /// <summary>
    /// Anger total tillgänglig kvantitet för availability-posten.
    /// </summary>
    public decimal AvailableQuantity { get; init; }

    /// <summary>
    /// Anger reserverad del av den tillgängliga kvantiteten.
    /// </summary>
    public decimal ReservedQuantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetsenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger minsta orderkvantitet när sådan finns.
    /// </summary>
    public decimal? MinimumOrderQuantity { get; init; }

    /// <summary>
    /// Identifierar enheten för minsta orderkvantitet.
    /// </summary>
    public int? MinimumOrderUnitId { get; init; }

    /// <summary>
    /// Anger första tidpunkt då availability gäller.
    /// </summary>
    public DateTimeOffset AvailableFromUtc { get; init; }

    /// <summary>
    /// Anger sista tidpunkt då availability gäller.
    /// </summary>
    public DateTimeOffset? AvailableToUtc { get; init; }

    /// <summary>
    /// Anger uppskattad ledtid i dagar när den är känd.
    /// </summary>
    public int? LeadTimeDays { get; init; }

    /// <summary>
    /// Identifierar platsen där materialet är tillgängligt när den är känd.
    /// </summary>
    public Guid? AvailabilityLocationId { get; init; }

    /// <summary>
    /// Anger supplier- eller källreferens för availability-posten.
    /// </summary>
    public string? SourceReference { get; init; }

    /// <summary>
    /// Anger när availability senast bekräftades.
    /// </summary>
    public DateTimeOffset? LastConfirmedAtUtc { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}
