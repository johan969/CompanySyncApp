namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller data för att lägga till ett geografiskt ursprung på ett supplier material offering.
    /// </summary>
    public sealed class CreateSupplierMaterialOfferingOriginRequest
    {
        /// <summary>
        /// Identifierar den geografiska plats som ska kopplas till offeringet.
        /// </summary>
        public Guid GeographicLocationId { get; init; }

        /// <summary>
        /// Anger vilken typ av ursprung platsen representerar.
        /// </summary>
        public SupplierOfferingOriginRole Role { get; init; }

        /// <summary>
        /// Anger om platsen är primär inom offeringet och den valda rollen.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Innehåller anteckningar om origin-kopplingen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token för offering-aggregatet.
        /// </summary>
        public uint SupplierMaterialOfferingRowVersion { get; init; }
    }
}
