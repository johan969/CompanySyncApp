namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller normaliserad process- och materialtypmapping för en supplier process capability.
    /// </summary>
    public sealed class SetSupplierProcessCapabilityMappingRequest
    {
        /// <summary>
        /// Identifierar normaliserad process när den är känd.
        /// </summary>
        public int? ProcessTypeId { get; init; }
        /// <summary>
        /// Identifierar accepterad input-materialtyp när den är känd.
        /// </summary>
        public int? InputMaterialTypeId { get; init; }
        /// <summary>
        /// Identifierar normal output-materialtyp när den är känd.
        /// </summary>
        public int? OutputMaterialTypeId { get; init; }
        /// <summary>
        /// Anger mappingens aktuella status.
        /// </summary>
        public SourcingMappingStatus MappingStatus { get; init; }
        /// <summary>
        /// Anger mappingens confidence mellan 0 och 1.
        /// </summary>
        public decimal? MappingConfidence { get; init; }
        /// <summary>
        /// Concurrency-token från senast lästa capability.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
