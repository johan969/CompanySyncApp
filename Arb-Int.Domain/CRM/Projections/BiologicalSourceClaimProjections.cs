using System.Linq.Expressions;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller EF Core-projections för biologiska source-claims.
    /// </summary>
    public static class BiologicalSourceClaimProjections
    {
        /// <summary>
        /// Projekterar ett claim till ett kompakt listobjekt.
        /// </summary>
        public static Expression<Func<BusinessRelationshipBiologicalSourceClaim, BiologicalSourceClaimListResponse>> List => claim => new BiologicalSourceClaimListResponse
        {
            Id = claim.Id,
            BusinessRelationshipId = claim.BusinessRelationshipId,
            CounterpartyName = claim.BusinessRelationship.Counterparty.Name,
            BusinessRelationshipTypeCode = claim.BusinessRelationship.BusinessRelationshipType.Code,
            DeclaredName = claim.DeclaredName,
            BiologicalSourceId = claim.BiologicalSourceId,
            MappedBiologicalSourceName = claim.BiologicalSource == null
                ? null
                : claim.BiologicalSource.Name,
            RelationshipKind = claim.RelationshipKind,
            MappingStatus = claim.MappingStatus,
            MappingConfidence = claim.MappingConfidence,
            VerificationStatus = claim.VerificationStatus,
            LastObservedUtc = claim.LastObservedUtc,
            EvidenceCount = claim.Evidence.Count,
            RegionCount = claim.RegionLinks.Count
        };

        /// <summary>
        /// Projekterar ett claim med evidens och regioner till ett komplett response-objekt.
        /// </summary>
        public static Expression<Func<BusinessRelationshipBiologicalSourceClaim, BiologicalSourceClaimResponse>> Details => claim => new BiologicalSourceClaimResponse
        {
            Id = claim.Id,
            BusinessRelationshipId = claim.BusinessRelationshipId,
            DeclaredName = claim.DeclaredName,
            BiologicalSourceId = claim.BiologicalSourceId,
            RelationshipKind = claim.RelationshipKind,
            MappingStatus = claim.MappingStatus,
            MappingConfidence = claim.MappingConfidence,
            VerificationStatus = claim.VerificationStatus,
            FirstObservedUtc = claim.FirstObservedUtc,
            LastObservedUtc = claim.LastObservedUtc,
            LastVerifiedUtc = claim.LastVerifiedUtc,
            Notes = claim.Notes,
            Evidence = claim.Evidence
                .OrderByDescending(x => x.ObservedUtc)
                .ThenBy(x => x.Id)
                .Select(x => new BiologicalSourceClaimEvidenceResponse
                {
                    Id = x.Id,
                    BusinessRelationshipBiologicalSourceClaimId = x.BusinessRelationshipBiologicalSourceClaimId,
                    EvidenceType = x.EvidenceType,
                    SourceSystem = x.SourceSystem,
                    SourceRecordKey = x.SourceRecordKey,
                    SourceUrl = x.SourceUrl,
                    ObservedUtc = x.ObservedUtc,
                    Notes = x.Notes,
                    ContentHash = x.ContentHash
                })
                .ToList(),
            Regions = claim.RegionLinks
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.GeographicLocationId)
                .Select(x => new BiologicalSourceClaimRegionResponse
                {
                    Id = x.Id,
                    BusinessRelationshipBiologicalSourceClaimId = x.BusinessRelationshipBiologicalSourceClaimId,
                    GeographicLocationId = x.GeographicLocationId,
                    Role = x.Role,
                    IsPrimary = x.IsPrimary,
                    Notes = x.Notes
                })
                .ToList()
        };
    }
}