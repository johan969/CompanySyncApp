namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller data för att koppla en characteristic till ett supplier material offering.
    /// </summary>
    public sealed class CreateSupplierMaterialOfferingCharacteristicRequest
    {
        /// <summary>
        /// Identifierar characteristic som ska kopplas till offeringet.
        /// </summary>
        public Guid CharacteristicId { get; init; }

        /// <summary>
        /// Concurrency-token för offering-aggregatet.
        /// </summary>
        public uint SupplierMaterialOfferingRowVersion { get; init; }
    }
}
