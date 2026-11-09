using System.Linq.Expressions;

namespace Arb_Int.Domain.Sourcing.Projections
{
    /// <summary>
    /// Innehåller återanvändbara EF-projektioner för supplier process capabilities.
    /// </summary>
    public static class SupplierProcessCapabilityProjections
    {
        /// <summary>
        /// Projekterar en capability till den kompakta listmodellen.
        /// </summary>
        public static Expression<Func<SupplierProcessCapability, SupplierProcessCapabilityListResponse>> List => capability => new SupplierProcessCapabilityListResponse
        {
            Id = capability.Id,
            BusinessRelationshipId = capability.BusinessRelationshipId,
            SupplierName = capability.BusinessRelationship.Counterparty.Name,
            CapabilityKey = capability.CapabilityKey,
            Name = capability.Name,
            DeclaredProcessName = capability.DeclaredProcessName,
            ProcessTypeId = capability.ProcessTypeId,
            ProcessTypeName = capability.ProcessType == null
                ? null
                : capability.ProcessType.Name,
            InputMaterialTypeId = capability.InputMaterialTypeId,
            InputMaterialTypeName = capability.InputMaterialType == null
                ? null
                : capability.InputMaterialType.Name,
            OutputMaterialTypeId = capability.OutputMaterialTypeId,
            OutputMaterialTypeName = capability.OutputMaterialType == null
                ? null
                : capability.OutputMaterialType.Name,
            MappingStatus = capability.MappingStatus,
            MappingConfidence = capability.MappingConfidence,
            Status = capability.Status,
            VerificationStatus = capability.VerificationStatus,
            LastObservedUtc = capability.LastObservedUtc,
            LastVerifiedUtc = capability.LastVerifiedUtc,
            RowVersion = capability.RowVersion
        };

        /// <summary>
        /// Projekterar en capability med capability-ägda scopes och evidens.
        /// </summary>
        public static Expression<Func<SupplierProcessCapability, SupplierProcessCapabilityResponse>> Details => capability => new SupplierProcessCapabilityResponse
        {
            Id = capability.Id,
            BusinessRelationshipId = capability.BusinessRelationshipId,
            CapabilityKey = capability.CapabilityKey,
            Name = capability.Name,
            DeclaredProcessName = capability.DeclaredProcessName,
            ProcessTypeId = capability.ProcessTypeId,
            InputMaterialTypeId = capability.InputMaterialTypeId,
            OutputMaterialTypeId = capability.OutputMaterialTypeId,
            Description = capability.Description,
            Notes = capability.Notes,
            MappingStatus = capability.MappingStatus,
            MappingConfidence = capability.MappingConfidence,
            Status = capability.Status,
            VerificationStatus = capability.VerificationStatus,
            FirstObservedUtc = capability.FirstObservedUtc,
            LastObservedUtc = capability.LastObservedUtc,
            LastVerifiedUtc = capability.LastVerifiedUtc,
            CreateDateUtc = capability.CreateDateUtc,
            BiologicalSourceScopes = capability.BiologicalSourceScopes
                .OrderBy(x => x.BusinessRelationshipBiologicalSourceClaimId)
                .ThenBy(x => x.Id)
                .Select(x => new SupplierProcessCapabilityBiologicalSourceScopeResponse
                {
                    Id = x.Id,
                    BusinessRelationshipId = x.BusinessRelationshipId,
                    SupplierProcessCapabilityId = x.SupplierProcessCapabilityId,
                    BusinessRelationshipBiologicalSourceClaimId = x.BusinessRelationshipBiologicalSourceClaimId,
                    Notes = x.Notes
                }).ToList(),
            Evidence = capability.Evidence
                .OrderByDescending(x => x.LastObservedUtc)
                .ThenBy(x => x.SourceSystem)
                .ThenBy(x => x.SourceRecordKey)
                .ThenBy(x => x.Id)
                .Select(x => new SupplierProcessCapabilityEvidenceResponse
                {
                    Id = x.Id,
                    SupplierProcessCapabilityId = x.SupplierProcessCapabilityId,
                    EvidenceType = x.EvidenceType,
                    SourceSystem = x.SourceSystem,
                    SourceRecordKey = x.SourceRecordKey,
                    SourceUrl = x.SourceUrl,
                    FirstObservedUtc = x.FirstObservedUtc,
                    LastObservedUtc = x.LastObservedUtc,
                    Notes = x.Notes,
                    ContentHash = x.ContentHash
                }).ToList(),
            RowVersion = capability.RowVersion
        };
    }
}
