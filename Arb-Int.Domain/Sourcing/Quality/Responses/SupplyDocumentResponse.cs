namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller kärndata och subjects för ett supply document.
    /// </summary>
    public sealed class SupplyDocumentResponse
    {
        /// <summary>
        /// Identifierar dokumentet.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar document type.
        /// </summary>
        public int DocumentTypeId { get; init; }

        /// <summary>
        /// Anger dokumentets visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Identifierar business relationship-subject när sådan finns.
        /// </summary>
        public Guid? SubjectBusinessRelationshipId { get; init; }

        /// <summary>
        /// Identifierar supplier material offering-subject när sådan finns.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Identifierar supplier process capability-subject när sådan finns.
        /// </summary>
        public Guid? SupplierProcessCapabilityId { get; init; }

        /// <summary>
        /// Identifierar inventory lot-subject när sådan finns.
        /// </summary>
        public Guid? InventoryLotId { get; init; }

        /// <summary>
        /// Identifierar shipment-subject när sådan finns.
        /// </summary>
        public Guid? ShipmentId { get; init; }

        /// <summary>
        /// Anger dokumentets status.
        /// </summary>
        public DocumentStatus Status { get; init; }


        /// <summary>
        /// Anger när dokumentets giltighet börjar.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Anger när dokumentets giltighet slutar.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Anger när dokumentet registrerades i systemet.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }

        /// <summary>
        /// Concurrency-token för document-aggregatet.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
