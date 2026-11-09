namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar metadata för en origin-koppling på ett supplier material offering.
    /// </summary>
    public sealed class UpdateSupplierMaterialOfferingOriginRequest
    {
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
