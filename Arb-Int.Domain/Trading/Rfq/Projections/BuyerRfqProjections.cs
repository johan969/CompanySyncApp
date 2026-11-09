using System.Linq.Expressions;
namespace Arb_Int.Domain.Trading.Projections;

/// <summary>
/// Innehåller EF Core-projections för BuyerRfq och BuyerRfqRevision.
/// </summary>
public static class BuyerRfqProjections
{
    /// <summary>
    /// Projekterar en BuyerRfq till ett kompakt listobjekt.
    /// </summary>
    public static Expression<Func<BuyerRfq, BuyerRfqListResponse>> List => rfq => new BuyerRfqListResponse
    {
        Id = rfq.Id,
        RfqNumber = rfq.RfqNumber,
        Name = rfq.Name,
        BuyerBusinessRelationshipId = rfq.BuyerBusinessRelationshipId,
        BuyerPartyId = rfq.BuyerBusinessRelationship.CounterpartyId,
        BuyerName = rfq.BuyerBusinessRelationship.Counterparty.Name,
        BuyerDemandId = rfq.BuyerDemandId,
        Status = rfq.Status,
        Priority = rfq.Priority,
        Visibility = rfq.Visibility,
        OwnerUserId = rfq.OwnerUserId,
        LatestRevisionNumber = rfq.LatestRevisionNumber,
        OpenedAtUtc = rfq.OpenedAtUtc,
        ClosedAtUtc = rfq.ClosedAtUtc,
        RowVersion = rfq.RowVersion
    };

    /// <summary>
    /// Projekterar en BuyerRfq med revisionsmetadata till ett komplett response-objekt.
    /// </summary>
    public static Expression<Func<BuyerRfq, BuyerRfqResponse>> Details => rfq => new BuyerRfqResponse
    {
        Id = rfq.Id,
        BuyerBusinessRelationshipId = rfq.BuyerBusinessRelationshipId,
        BuyerPartyId = rfq.BuyerBusinessRelationship.CounterpartyId,
        BuyerName = rfq.BuyerBusinessRelationship.Counterparty.Name,
        BuyerDemandId = rfq.BuyerDemandId,
        RfqNumber = rfq.RfqNumber,
        Name = rfq.Name,
        Description = rfq.Description,
        Status = rfq.Status,
        Priority = rfq.Priority,
        Visibility = rfq.Visibility,
        BuyerReference = rfq.BuyerReference,
        OwnerUserId = rfq.OwnerUserId,
        LatestRevisionNumber = rfq.LatestRevisionNumber,
        OpenedAtUtc = rfq.OpenedAtUtc,
        ClosedAtUtc = rfq.ClosedAtUtc,
        InternalNotes = rfq.InternalNotes,
        CreateDateUtc = rfq.CreateDateUtc,
        RowVersion = rfq.RowVersion,
        Revisions = rfq.Revisions
            .OrderByDescending(x => x.RevisionNumber)
            .Select(x => new BuyerRfqRevisionResponse
            {
                Id = x.Id,
                BuyerRfqId = x.BuyerRfqId,
                RevisionNumber = x.RevisionNumber,
                Status = x.Status,
                Summary = x.Summary,
                BuyerOriginalText = x.BuyerOriginalText,
                InternalInterpretation = x.InternalInterpretation,
                ResponseRequestedByUtc = x.ResponseRequestedByUtc,
                ReleasedAtUtc = x.ReleasedAtUtc,
                ReleasedByUserId = x.ReleasedByUserId,
                SupersededAtUtc = x.SupersededAtUtc,
                SnapshotHash = x.SnapshotHash,
                IsEditable = x.Status == BuyerRfqRevisionStatus.Draft,
                LineCount = x.Lines.Count,
                DeliveryCount = x.Deliveries.Count,
                RequirementCount = x.Requirements.Count,
                CreateDateUtc = x.CreateDateUtc,
                RowVersion = x.RowVersion
            })
            .ToList()
    };

    /// <summary>
    /// Projekterar en enskild RFQ-revision till response-objekt.
    /// </summary>
    public static Expression<Func<BuyerRfqRevision, BuyerRfqRevisionResponse>> Revision => revision => new BuyerRfqRevisionResponse
    {
        Id = revision.Id,
        BuyerRfqId = revision.BuyerRfqId,
        RevisionNumber = revision.RevisionNumber,
        Status = revision.Status,
        Summary = revision.Summary,
        BuyerOriginalText = revision.BuyerOriginalText,
        InternalInterpretation = revision.InternalInterpretation,
        ResponseRequestedByUtc = revision.ResponseRequestedByUtc,
        ReleasedAtUtc = revision.ReleasedAtUtc,
        ReleasedByUserId = revision.ReleasedByUserId,
        SupersededAtUtc = revision.SupersededAtUtc,
        SnapshotHash = revision.SnapshotHash,
        IsEditable = revision.Status == BuyerRfqRevisionStatus.Draft,
        LineCount = revision.Lines.Count,
        DeliveryCount = revision.Deliveries.Count,
        RequirementCount = revision.Requirements.Count,
        CreateDateUtc = revision.CreateDateUtc,
        RowVersion = revision.RowVersion
    };
}