using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för supply certifications.
    /// </summary>
    public sealed class SupplyCertificationQueryRequest
    {
        /// <summary>
        /// Begränsar resultatet till en certification scheme-version.
        /// </summary>
        public int? CertificationSchemeId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en scheme jurisdiction.
        /// </summary>
        public int? CertificationSchemeJurisdictionId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en certification scope.
        /// </summary>
        public CertificationScope? Scope { get; init; }

        /// <summary>
        /// Begränsar resultatet till en certification status.
        /// </summary>
        public CertificationStatus? Status { get; init; }


        /// <summary>
        /// Begränsar resultatet till ett certified business relationship.
        /// </summary>
        public Guid? CertifiedBusinessRelationshipId { get; init; }

        /// <summary>
        /// Begränsar resultatet till ett supplier material offering.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en supplier process capability.
        /// </summary>
        public Guid? SupplierProcessCapabilityId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en inventory lot.
        /// </summary>
        public Guid? InventoryLotId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en shipment.
        /// </summary>
        public Guid? ShipmentId { get; init; }

        /// <summary>
        /// Begränsar resultatet till ett supporting supply document.
        /// </summary>
        public Guid? SupplyDocumentId { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public SupplyCertificationSortField SortBy { get; init; } = SupplyCertificationSortField.CreateDateUtc;

        /// <summary>
        /// Anger sorteringsriktning.
        /// </summary>
        public SortDirection SortDirection { get; init; } = SortDirection.Descending;

        /// <summary>
        /// Sidnummer med start på 1.
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Maximalt antal poster per sida.
        /// </summary>
        public int PageSize { get; init; } = 50;
    }

    /// <summary>
    /// Anger tillgängliga sorteringsfält för supply certifications.
    /// </summary>
    public enum SupplyCertificationSortField
    {
        /// <summary>
        /// Sorterar på skapandetid.
        /// </summary>
        CreateDateUtc = 0,

        /// <summary>
        /// Sorterar på validity start.
        /// </summary>
        ValidFromUtc = 1,

        /// <summary>
        /// Sorterar på validity end.
        /// </summary>
        ValidToUtc = 2,

        /// <summary>
        /// Sorterar på status.
        /// </summary>
        Status = 3,


        /// <summary>
        /// Sorterar på scope.
        /// </summary>
        Scope = 4
    }
}
