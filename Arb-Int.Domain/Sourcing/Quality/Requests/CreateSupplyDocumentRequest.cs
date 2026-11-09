namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller kärndata för att registrera ett supply document och dess subjects.
    /// </summary>
    public sealed class CreateSupplyDocumentRequest
    {
        /// <summary>
        /// Identifierar document type.
        /// </summary>
        public int DocumentTypeId { get; init; }

        /// <summary>
        /// Anger dokumentets visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Identifierar business relationship när dokumentet gäller relationen.
        /// </summary>
        public Guid? SubjectBusinessRelationshipId { get; init; }

        /// <summary>
        /// Identifierar supplier material offering när dokumentet gäller offeringet.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Identifierar supplier process capability när dokumentet gäller capabilityn.
        /// </summary>
        public Guid? SupplierProcessCapabilityId { get; init; }

        /// <summary>
        /// Identifierar inventory lot när dokumentet gäller loten.
        /// </summary>
        public Guid? InventoryLotId { get; init; }

        /// <summary>
        /// Identifierar shipment när dokumentet gäller shipmenten.
        /// </summary>
        public Guid? ShipmentId { get; init; }

        /// <summary>
        /// Anger när dokumentets giltighet börjar när den är känd.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Anger när dokumentets giltighet slutar när den är känd.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }
    }
}
