namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller offering- och materialmapping för en inventory lot.
    /// </summary>
    public sealed class SetInventoryLotMappingRequest
    {
        /// <summary>
        /// Identifierar supplier material offering när loten kan kopplas till ett sådant.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Identifierar kanoniskt material när det är känt.
        /// </summary>
        public Guid? MaterialId { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa lot.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
