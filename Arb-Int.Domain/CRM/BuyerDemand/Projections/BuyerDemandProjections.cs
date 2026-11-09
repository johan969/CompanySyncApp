using System.Linq.Expressions;

namespace Arb_Int.Domain.Crm;

/// <summary>
/// Innehåller EF Core-projections för BuyerDemand.
/// </summary>
public static class BuyerDemandProjections
{
    /// <summary>
    /// Projekterar en BuyerDemand till ett kompakt listobjekt.
    /// </summary>
    public static Expression<Func<BuyerDemand, BuyerDemandListResponse>> List => demand => new BuyerDemandListResponse
    {
        Id = demand.Id,
        BusinessRelationshipId = demand.BusinessRelationshipId,
        CounterpartyId = demand.BusinessRelationship.CounterpartyId,
        Name = demand.Name,
        CounterpartyName = demand.BusinessRelationship.Counterparty.Name,
        KnowledgeLevel = demand.KnowledgeLevel,
        Status = demand.Status,
        Confidence = demand.Confidence,
        LastConfirmedAtUtc = demand.LastConfirmedAtUtc,
        OwnerUserId = demand.OwnerUserId,
        CreateDateUtc = demand.CreateDateUtc,
        ScopeCount = demand.Scopes.Count,
        EvidenceCount = demand.Evidence.Count,
        RowVersion = demand.RowVersion
    };

    /// <summary>
    /// Projekterar en BuyerDemand med scopes och evidence till ett komplett response-objekt.
    /// </summary>
    public static Expression<Func<BuyerDemand, BuyerDemandResponse>> Details => demand => new BuyerDemandResponse
    {
        Id = demand.Id,
        BusinessRelationshipId = demand.BusinessRelationshipId,
        CounterpartyId = demand.BusinessRelationship.CounterpartyId,
        CounterpartyName = demand.BusinessRelationship.Counterparty.Name,
        Name = demand.Name,
        Description = demand.Description,
        KnowledgeLevel = demand.KnowledgeLevel,
        Status = demand.Status,
        Confidence = demand.Confidence,
        ValidFromUtc = demand.ValidFromUtc,
        ValidToUtc = demand.ValidToUtc,
        LastConfirmedAtUtc = demand.LastConfirmedAtUtc,
        OwnerUserId = demand.OwnerUserId,
        InternalNotes = demand.InternalNotes,
        CreateDateUtc = demand.CreateDateUtc,
        RowVersion = demand.RowVersion,
        Scopes = demand.Scopes
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(x => new BuyerDemandScopeResponse
            {
                Id = x.Id,
                BuyerDemandId = x.BuyerDemandId,
                BusinessRelationshipId = x.BusinessRelationshipId,
                BiologicalSourceClaimId = x.BiologicalSourceClaimId,
                MaterialId = x.MaterialId,
                MaterialTypeId = x.MaterialTypeId,
                IntendedUseCharacteristicId = x.IntendedUseCharacteristicId,
                DeclaredFormat = x.DeclaredFormat,
                IntendedUseText = x.IntendedUseText,
                TypicalQuantity = x.TypicalQuantity,
                TypicalQuantityUnitId = x.TypicalQuantityUnitId,
                RecurrenceDescription = x.RecurrenceDescription,
                SortOrder = x.SortOrder,
                RowVersion = x.RowVersion
            })
            .ToList(),
        Evidence = demand.Evidence
            .OrderByDescending(x => x.ObservedAtUtc)
            .ThenBy(x => x.Id)
            .Select(x => new BuyerDemandEvidenceResponse
            {
                Id = x.Id,
                BuyerDemandId = x.BuyerDemandId,
                SourceKind = x.SourceKind,
                Position = x.Position,
                ReferenceEntryId = x.ReferenceEntryId,
                CrmActivityId = x.CrmActivityId,
                SourceUri = x.SourceUri,
                SourceExternalId = x.SourceExternalId,
                Excerpt = x.Excerpt,
                Notes = x.Notes,
                Confidence = x.Confidence,
                ObservedAtUtc = x.ObservedAtUtc,
                EvidenceHash = x.EvidenceHash,
                RowVersion = x.RowVersion
            })
            .ToList()
    };

    /// <summary>
    /// Projekterar BuyerDemandStateHistory till response-objekt.
    /// </summary>
    public static Expression<Func<BuyerDemandStateHistory, BuyerDemandStateHistoryResponse>> StateHistory => history => new BuyerDemandStateHistoryResponse
    {
        Id = history.Id,
        BuyerDemandId = history.BuyerDemandId,
        FromKnowledgeLevel = history.FromKnowledgeLevel,
        ToKnowledgeLevel = history.ToKnowledgeLevel,
        FromStatus = history.FromStatus,
        ToStatus = history.ToStatus,
        BuyerDemandEvidenceId = history.BuyerDemandEvidenceId,
        Confidence = history.Confidence,
        ChangedAtUtc = history.ChangedAtUtc,
        ChangedByUserId = history.ChangedByUserId,
        Notes = history.Notes
    };
}