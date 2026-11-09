namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar metadata för en biologisk source-koppling.
    /// </summary>
    public sealed class UpdateSupplierMaterialOfferingBiologicalSourceRequest
    {
        /// <summary>
        /// Anger claimets roll i offeringet.
        /// </summary>
        public SupplierOfferingBiologicalSourceRole Role { get; init; }

        /// <summary>
        /// Anger deklarerad andel i procent när offeringet består av flera biologiska källor.
        /// </summary>
        public decimal? ProportionPercent { get; init; }

        /// <summary>
        /// Innehåller anteckningar om kopplingen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token för offering-aggregatet.
        /// </summary>
        public uint SupplierMaterialOfferingRowVersion { get; init; }
    }
}
