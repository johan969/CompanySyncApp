namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en certification assertion mot exakt ett supply-subject.
    /// </summary>
    public sealed class SupplyCertificationResponse
    {
        /// <summary>
        /// Identifierar certificationen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar certification scheme-versionen.
        /// </summary>
        public int CertificationSchemeId { get; init; }

        /// <summary>
        /// Identifierar jurisdiction inom scheme-versionen när sådan används.
        /// </summary>
        public int? CertificationSchemeJurisdictionId { get; init; }

        /// <summary>
        /// Identifierar certified business relationship när det är subject.
        /// </summary>
        public Guid? CertifiedBusinessRelationshipId { get; init; }

        /// <summary>
        /// Identifierar supplier material offering när det är subject.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Identifierar supplier process capability när det är subject.
        /// </summary>
        public Guid? SupplierProcessCapabilityId { get; init; }

        /// <summary>
        /// Identifierar inventory lot när det är subject.
        /// </summary>
        public Guid? InventoryLotId { get; init; }

        /// <summary>
        /// Identifierar shipment när det är subject.
        /// </summary>
        public Guid? ShipmentId { get; init; }

        /// <summary>
        /// Anger certificationens scope.
        /// </summary>
        public CertificationScope Scope { get; init; }

        /// <summary>
        /// Anger certificationens status.
        /// </summary>
        public CertificationStatus Status { get; init; }


        /// <summary>
        /// Anger när certificationens giltighet börjar.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Anger när certificationens giltighet slutar.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Identifierar supporting supply document när ett sådant finns.
        /// </summary>
        public Guid? SupplyDocumentId { get; init; }

        /// <summary>
        /// Anger när certificationen registrerades i systemet.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }

        /// <summary>
        /// Concurrency-token för certification-aggregatet.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
