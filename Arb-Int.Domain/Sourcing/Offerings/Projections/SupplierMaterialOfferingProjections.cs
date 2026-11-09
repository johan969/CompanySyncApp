using System.Linq.Expressions;

namespace Arb_Int.Domain.Sourcing.Projections;

/// <summary>
/// Innehåller återanvändbara EF-projektioner för supplier material offerings.
/// </summary>
public static class SupplierMaterialOfferingProjections
{
    /// <summary>
    /// Projekterar ett offering till den kompakta listmodellen.
    /// </summary>
    public static Expression<Func<SupplierMaterialOffering, SupplierMaterialOfferingListResponse>> List => offering => new SupplierMaterialOfferingListResponse
    {
        Id = offering.Id,
        BusinessRelationshipId = offering.BusinessRelationshipId,
        SupplierName = offering.BusinessRelationship.Counterparty.Name,
        OfferingKey = offering.OfferingKey,
        Name = offering.Name,
        SupplierSku = offering.SupplierSku,
        DeclaredFormat = offering.DeclaredFormat,
        OfferedMaterialTypeId = offering.OfferedMaterialTypeId,
        OfferedMaterialTypeName = offering.OfferedMaterialType == null
            ? null
            : offering.OfferedMaterialType.Name,
        MappedMaterialId = offering.MappedMaterialId,
        MappedMaterialName = offering.MappedMaterial == null
            ? null
            : offering.MappedMaterial.Name,
        MappingStatus = offering.MappingStatus,
        MappingConfidence = offering.MappingConfidence,
        Status = offering.Status,
        LastObservedUtc = offering.LastObservedUtc,
        LastVerifiedUtc = offering.LastVerifiedUtc,
        RowVersion = offering.RowVersion
    };

    /// <summary>
    /// Projekterar ett offering med offering-ägda child-objekt.
    /// </summary>
    public static Expression<Func<SupplierMaterialOffering, SupplierMaterialOfferingResponse>> Details => offering => new SupplierMaterialOfferingResponse
    {
        Id = offering.Id,
        BusinessRelationshipId = offering.BusinessRelationshipId,
        OfferingKey = offering.OfferingKey,
        Name = offering.Name,
        Description = offering.Description,
        SupplierSku = offering.SupplierSku,
        DeclaredFormat = offering.DeclaredFormat,
        OfferedMaterialTypeId = offering.OfferedMaterialTypeId,
        MappedMaterialId = offering.MappedMaterialId,
        MappingStatus = offering.MappingStatus,
        MappingConfidence = offering.MappingConfidence,
        MappingNotes = offering.MappingNotes,
        Status = offering.Status,
        FirstObservedUtc = offering.FirstObservedUtc,
        LastObservedUtc = offering.LastObservedUtc,
        LastVerifiedUtc = offering.LastVerifiedUtc,
        CreateDateUtc = offering.CreateDateUtc,
        BiologicalSources = offering.BiologicalSourceLinks
            .OrderBy(x => x.Role)
            .ThenBy(x => x.BusinessRelationshipBiologicalSourceClaimId)
            .ThenBy(x => x.Id)
            .Select(x => new SupplierMaterialOfferingBiologicalSourceResponse
            {
                Id = x.Id,
                BusinessRelationshipId = x.BusinessRelationshipId,
                SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                BusinessRelationshipBiologicalSourceClaimId = x.BusinessRelationshipBiologicalSourceClaimId,
                Role = x.Role,
                ProportionPercent = x.ProportionPercent,
                Notes = x.Notes
            })
            .ToList(),
        Origins = offering.OriginLinks
            .OrderBy(x => x.Role)
            .ThenByDescending(x => x.IsPrimary)
            .ThenBy(x => x.GeographicLocationId)
            .ThenBy(x => x.Id)
            .Select(x => new SupplierMaterialOfferingOriginResponse
            {
                Id = x.Id,
                SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                GeographicLocationId = x.GeographicLocationId,
                Role = x.Role,
                IsPrimary = x.IsPrimary,
                Notes = x.Notes
            })
            .ToList(),
        Characteristics = offering.CharacteristicLinks
            .OrderBy(x => x.CharacteristicId)
            .ThenBy(x => x.Id)
            .Select(x => new SupplierMaterialOfferingCharacteristicResponse
            {
                Id = x.Id,
                SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                CharacteristicId = x.CharacteristicId
            })
            .ToList(),
        Evidence = offering.Evidence
            .OrderByDescending(x => x.LastObservedUtc)
            .ThenBy(x => x.SourceSystem)
            .ThenBy(x => x.SourceRecordKey)
            .ThenBy(x => x.Id)
            .Select(x => new SupplierMaterialOfferingEvidenceResponse
            {
                Id = x.Id,
                SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                EvidenceType = x.EvidenceType,
                SourceSystem = x.SourceSystem,
                SourceRecordKey = x.SourceRecordKey,
                SourceUrl = x.SourceUrl,
                FirstObservedUtc = x.FirstObservedUtc,
                LastObservedUtc = x.LastObservedUtc,
                Notes = x.Notes,
                ContentHash = x.ContentHash
            })
            .ToList(),
        Availabilities = offering.Availabilities
            .OrderByDescending(x => x.AvailableFromUtc)
            .ThenBy(x => x.Id)
            .Select(x => new SupplierOfferingAvailabilityResponse
            {
                Id = x.Id,
                SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                InventoryLotId = x.InventoryLotId,
                Status = x.Status,
                AvailableQuantity = x.AvailableQuantity,
                ReservedQuantity = x.ReservedQuantity,
                QuantityUnitId = x.QuantityUnitId,
                MinimumOrderQuantity = x.MinimumOrderQuantity,
                MinimumOrderUnitId = x.MinimumOrderUnitId,
                AvailableFromUtc = x.AvailableFromUtc,
                AvailableToUtc = x.AvailableToUtc,
                LeadTimeDays = x.LeadTimeDays,
                AvailabilityLocationId = x.AvailabilityLocationId,
                SourceReference = x.SourceReference,
                LastConfirmedAtUtc = x.LastConfirmedAtUtc,
                RowVersion = x.RowVersion
            })
            .ToList(),
        Specifications = offering.Specifications
            .OrderBy(x => x.Kind)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new SupplierOfferingSpecificationResponse
            {
                Id = x.Id,
                SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                Kind = x.Kind,
                Name = x.Name,
                Description = x.Description,
                ConstituentId = x.ConstituentId,
                CharacteristicId = x.CharacteristicId,
                Operator = x.Operator,
                MinimumValue = x.MinimumValue,
                MaximumValue = x.MaximumValue,
                TargetValue = x.TargetValue,
                TextValue = x.TextValue,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Basis = x.Basis,
                AnalysisMethod = x.AnalysisMethod,
                VerificationState = x.VerificationState,
                ValidFromUtc = x.ValidFromUtc,
                ValidToUtc = x.ValidToUtc,
                SupplyDocumentId = x.SupplyDocumentId,
                RowVersion = x.RowVersion
            })
            .ToList(),
        RowVersion = offering.RowVersion
    };
}
