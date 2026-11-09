using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för supply documents.
    /// </summary>
    public sealed class SupplyDocumentQueryRequest
    {
        /// <summary>
        /// Begränsar resultatet till en document type.
        /// </summary>
        public int? DocumentTypeId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en document-status.
        /// </summary>
        public DocumentStatus? Status { get; init; }


        /// <summary>
        /// Begränsar resultatet till en business relationship.
        /// </summary>
        public Guid? SubjectBusinessRelationshipId { get; init; }

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
        /// Anger sorteringsfält.
        /// </summary>
        public SupplyDocumentSortField SortBy { get; init; } = SupplyDocumentSortField.CreateDateUtc;

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
    /// Anger tillgängliga sorteringsfält för supply documents.
    /// </summary>
    public enum SupplyDocumentSortField
    {
        /// <summary>
        /// Sorterar på skapandetid.
        /// </summary>
        CreateDateUtc = 0,

        /// <summary>
        /// Sorterar på giltighetens start.
        /// </summary>
        ValidFromUtc = 1,

        /// <summary>
        /// Sorterar på giltighetens slut.
        /// </summary>
        ValidToUtc = 2,

        /// <summary>
        /// Sorterar på document-status.
        /// </summary>
        Status = 3,

    }
}
