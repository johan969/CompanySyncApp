using System.Linq.Expressions;

namespace Arb_Int.Domain.Sourcing.Projections
{
    /// <summary>
    /// Innehåller återanvändbar EF-projektion för supply certifications.
    /// </summary>
    public static class SupplyCertificationProjections
    {
        /// <summary>
        /// Projekterar en supply certification till response-modellen.
        /// </summary>
        public static Expression<Func<SupplyCertification, SupplyCertificationResponse>> Details => certification => new SupplyCertificationResponse
        {
            Id = certification.Id,
            CertificationSchemeId = certification.CertificationSchemeId,
            CertificationSchemeJurisdictionId = certification.CertificationSchemeJurisdictionId,
            CertifiedBusinessRelationshipId = certification.CertifiedBusinessRelationshipId,
            SupplierMaterialOfferingId = certification.SupplierMaterialOfferingId,
            SupplierProcessCapabilityId = certification.SupplierProcessCapabilityId,
            InventoryLotId = certification.InventoryLotId,
            ShipmentId = certification.ShipmentId,
            Scope = certification.Scope,
            Status = certification.Status,
            ValidFromUtc = certification.ValidFromUtc,
            ValidToUtc = certification.ValidToUtc,
            SupplyDocumentId = certification.SupplyDocumentId,
            CreateDateUtc = certification.CreateDateUtc,
            RowVersion = certification.RowVersion
        };
    }
}
