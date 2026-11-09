namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar availability-data utan att byta offering eller inventory lot.
    /// </summary>
    public sealed class UpdateSupplierOfferingAvailabilityRequest
    {
        /// <summary>
        /// Anger availability-postens aktuella status.
        /// </summary>
        public SupplyAvailabilityStatus Status { get; init; }

        /// <summary>
        /// Anger total kvantitet som availability-posten beskriver som tillgänglig.
        /// </summary>
        public decimal AvailableQuantity { get; init; }

        /// <summary>
        /// Anger den del av availability-kvantiteten som redan är reserverad.
        /// </summary>
        public decimal ReservedQuantity { get; init; }

        /// <summary>
        /// Identifierar måttenheten för availability-kvantiteterna.
        /// </summary>
        public int QuantityUnitId { get; init; }

        /// <summary>
        /// Anger minsta orderkvantitet när en sådan finns.
        /// </summary>
        public decimal? MinimumOrderQuantity { get; init; }

        /// <summary>
        /// Identifierar måttenheten för minsta orderkvantitet.
        /// </summary>
        public int? MinimumOrderUnitId { get; init; }

        /// <summary>
        /// Anger när availability-fönstret börjar.
        /// </summary>
        public DateTimeOffset AvailableFromUtc { get; init; }

        /// <summary>
        /// Anger när availability-fönstret slutar när det är tidsbegränsat.
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
        /// Innehåller supplier- eller källreferens för availability-posten.
        /// </summary>
        public string? SourceReference { get; init; }

        /// <summary>
        /// Anger när availability senast bekräftades.
        /// </summary>
        public DateTimeOffset? LastConfirmedAtUtc { get; init; }

        /// <summary>
        /// Concurrency-token för availability-raden.
        /// </summary>
        public uint RowVersion { get; init; }

        /// <summary>
        /// Concurrency-token för offering-aggregatet.
        /// </summary>
        public uint SupplierMaterialOfferingRowVersion { get; init; }
    }
}
