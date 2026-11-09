using System.Linq.Expressions;

namespace Arb_Int.Domain.Sourcing.Projections
{
    /// <summary>
    /// Innehåller återanvändbara EF-projektioner för supply documents.
    /// </summary>
    public static class SupplyDocumentProjections
    {
        /// <summary>
        /// Projekterar ett document till listmodellen.
        /// </summary>
        public static Expression<Func<SupplyDocument, SupplyDocumentListResponse>> List => document => new SupplyDocumentListResponse
        {
            Id = document.Id,
            DocumentTypeId = document.DocumentTypeId,
            Name = document.Name,
            SubjectBusinessRelationshipId = document.SubjectBusinessRelationshipId,
            SupplierMaterialOfferingId = document.SupplierMaterialOfferingId,
            SupplierProcessCapabilityId = document.SupplierProcessCapabilityId,
            InventoryLotId = document.InventoryLotId,
            ShipmentId = document.ShipmentId,
            Status = document.Status,
            ValidFromUtc = document.ValidFromUtc,
            ValidToUtc = document.ValidToUtc,
            CreateDateUtc = document.CreateDateUtc,
            RowVersion = document.RowVersion
        };

        /// <summary>
        /// Projekterar ett document till detaljmodellen.
        /// </summary>
        public static Expression<Func<SupplyDocument, SupplyDocumentResponse>> Details => document => new SupplyDocumentResponse
        {
            Id = document.Id,
            DocumentTypeId = document.DocumentTypeId,
            Name = document.Name,
            SubjectBusinessRelationshipId = document.SubjectBusinessRelationshipId,
            SupplierMaterialOfferingId = document.SupplierMaterialOfferingId,
            SupplierProcessCapabilityId = document.SupplierProcessCapabilityId,
            InventoryLotId = document.InventoryLotId,
            ShipmentId = document.ShipmentId,
            Status = document.Status,
            ValidFromUtc = document.ValidFromUtc,
            ValidToUtc = document.ValidToUtc,
            CreateDateUtc = document.CreateDateUtc,
            RowVersion = document.RowVersion
        };
    }
}
