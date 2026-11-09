namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller kärndata för att registrera en certification assertion mot exakt ett subject.
    /// </summary>
    public sealed class CreateSupplyCertificationRequest
    {
        /// <summary>
        /// Identifierar certification scheme-versionen.
        /// </summary>
        public int CertificationSchemeId { get; init; }

        /// <summary>
        /// Identifierar jurisdiction inom vald scheme-version när sådan används.
        /// </summary>
        public int? CertificationSchemeJurisdictionId { get; init; }

        /// <summary>
        /// Identifierar business relationship när certificationen gäller organisationen eller relationen.
        /// </summary>
        public Guid? CertifiedBusinessRelationshipId { get; init; }

        /// <summary>
        /// Identifierar supplier material offering när certificationen gäller offeringet.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Identifierar supplier process capability när certificationen gäller processen.
        /// </summary>
        public Guid? SupplierProcessCapabilityId { get; init; }

        /// <summary>
        /// Identifierar inventory lot när certificationen gäller loten.
        /// </summary>
        public Guid? InventoryLotId { get; init; }

        /// <summary>
        /// Identifierar shipment när certificationen gäller shipmenten.
        /// </summary>
        public Guid? ShipmentId { get; init; }

        /// <summary>
        /// Anger certificationens scope.
        /// </summary>
        public CertificationScope Scope { get; init; }

        /// <summary>
        /// Anger när certificationens giltighet börjar när den är känd.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Anger när certificationens giltighet slutar när den är känd.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Identifierar supporting supply document när ett sådant redan finns.
        /// </summary>
        public Guid? SupplyDocumentId { get; init; }
    }
}
