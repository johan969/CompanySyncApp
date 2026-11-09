using System.Linq.Expressions;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller EF Core-projections för affärsrelationer.
    /// </summary>
    public static class BusinessRelationshipProjections
    {
        /// <summary>
        /// Projekterar en affärsrelation till ett kompakt listobjekt.
        /// </summary>
        public static Expression<Func<BusinessRelationship, BusinessRelationshipListResponse>> List => relationship => new BusinessRelationshipListResponse
        {
            Id = relationship.Id,
            OwningOrganizationId = relationship.OwningOrganizationId,
            CounterpartyId = relationship.CounterpartyId,
            CounterpartyName = relationship.Counterparty.Name,
            Priority = relationship.Priority,
            Score = relationship.Score,
            OwnerUserId = relationship.OwnerUserId,
            LastContactedUtc = relationship.LastContactedUtc,
            NextActionUtc = relationship.NextActionUtc,
            ClosedUtc = relationship.ClosedUtc,
            RelationshipType = new BusinessRelationshipTypeResponse
            {
                Id = relationship.BusinessRelationshipType.Id,
                Code = relationship.BusinessRelationshipType.Code,
                Name = relationship.BusinessRelationshipType.Name,
                IsActive = relationship.BusinessRelationshipType.IsActive
            },
            Stage = new BusinessRelationshipStageResponse
            {
                Id = relationship.BusinessRelationshipStage.Id,
                BusinessRelationshipTypeId = relationship.BusinessRelationshipStage.BusinessRelationshipTypeId,
                Code = relationship.BusinessRelationshipStage.Code,
                Name = relationship.BusinessRelationshipStage.Name,
                SortOrder = relationship.BusinessRelationshipStage.SortOrder,
                IsClosed = relationship.BusinessRelationshipStage.IsClosed,
                IsSuccessful = relationship.BusinessRelationshipStage.IsSuccessful
            }
        };

        /// <summary>
        /// Projekterar en affärsrelation till ett detaljerat response-objekt.
        /// </summary>
        public static Expression<Func<BusinessRelationship, BusinessRelationshipResponse>> Details => relationship => new BusinessRelationshipResponse
        {
            Id = relationship.Id,
            OwningOrganizationId = relationship.OwningOrganizationId,
            OwningOrganizationName = relationship.OwningOrganization.Name,
            CounterpartyId = relationship.CounterpartyId,
            CounterpartyName = relationship.Counterparty.Name,
            CounterpartyIsActive = relationship.Counterparty.IsActive,
            Priority = relationship.Priority,
            Score = relationship.Score,
            LeadSource = relationship.LeadSource,
            OwnerUserId = relationship.OwnerUserId,
            FirstObservedUtc = relationship.FirstObservedUtc,
            FirstContactedUtc = relationship.FirstContactedUtc,
            LastContactedUtc = relationship.LastContactedUtc,
            NextActionUtc = relationship.NextActionUtc,
            ClosedUtc = relationship.ClosedUtc,
            Notes = relationship.Notes,
            CreateDateUtc = relationship.CreateDateUtc,
            ContactCount = relationship.Contacts.Count,
            BiologicalSourceClaimCount = relationship.BiologicalSourceClaims.Count,
            ActivityCount = relationship.Activities.Count,
            BuyerDemandCount = relationship.BuyerDemands.Count,
            RelationshipType = new BusinessRelationshipTypeResponse
            {
                Id = relationship.BusinessRelationshipType.Id,
                Code = relationship.BusinessRelationshipType.Code,
                Name = relationship.BusinessRelationshipType.Name,
                IsActive = relationship.BusinessRelationshipType.IsActive
            },
            Stage = new BusinessRelationshipStageResponse
            {
                Id = relationship.BusinessRelationshipStage.Id,
                BusinessRelationshipTypeId = relationship.BusinessRelationshipStage.BusinessRelationshipTypeId,
                Code = relationship.BusinessRelationshipStage.Code,
                Name = relationship.BusinessRelationshipStage.Name,
                SortOrder = relationship.BusinessRelationshipStage.SortOrder,
                IsClosed = relationship.BusinessRelationshipStage.IsClosed,
                IsSuccessful = relationship.BusinessRelationshipStage.IsSuccessful
            }
        };

        /// <summary>
        /// Projekterar stage history till response-objekt.
        /// </summary>
        public static Expression<Func<BusinessRelationshipStageHistory, BusinessRelationshipStageHistoryResponse>> StageHistory => history => new BusinessRelationshipStageHistoryResponse
        {
            Id = history.Id,
            BusinessRelationshipId = history.BusinessRelationshipId,
            ChangedUtc = history.ChangedUtc,
            ChangedByUserId = history.ChangedByUserId,
            Notes = history.Notes,
            FromStage = history.FromStage == null
                ? null
                : new BusinessRelationshipStageResponse
                {
                    Id = history.FromStage.Id,
                    BusinessRelationshipTypeId = history.FromStage.BusinessRelationshipTypeId,
                    Code = history.FromStage.Code,
                    Name = history.FromStage.Name,
                    SortOrder = history.FromStage.SortOrder,
                    IsClosed = history.FromStage.IsClosed,
                    IsSuccessful = history.FromStage.IsSuccessful
                },
            ToStage = new BusinessRelationshipStageResponse
            {
                Id = history.ToStage.Id,
                BusinessRelationshipTypeId = history.ToStage.BusinessRelationshipTypeId,
                Code = history.ToStage.Code,
                Name = history.ToStage.Name,
                SortOrder = history.ToStage.SortOrder,
                IsClosed = history.ToStage.IsClosed,
                IsSuccessful = history.ToStage.IsSuccessful
            }
        };
    }
}