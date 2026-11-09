using System.Linq.Expressions;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller EF Core-projections för CRM-aktiviteter.
    /// </summary>
    public static class CrmActivityProjections
    {
        /// <summary>
        /// Projekterar en CRM-aktivitet till ett kompakt listobjekt.
        /// </summary>
        public static Expression<Func<CrmActivity, CrmActivityListResponse>> List => activity => new CrmActivityListResponse
        {
            Id = activity.Id,
            RegardingPartyId = activity.RegardingPartyId,
            RegardingPartyName = activity.RegardingParty.Name,
            BusinessRelationshipId = activity.BusinessRelationshipId,
            BusinessRelationshipTypeCode = activity.BusinessRelationship == null ? null : activity.BusinessRelationship.BusinessRelationshipType.Code,
            ActivityType = activity.ActivityType,
            Status = activity.Status,
            Direction = activity.Direction,
            Subject = activity.Subject,
            OccurredUtc = activity.OccurredUtc,
            DueUtc = activity.DueUtc,
            CompletedUtc = activity.CompletedUtc,
            OwnerUserId = activity.OwnerUserId,
            ParticipantCount = activity.Participants.Count
        };

        /// <summary>
        /// Projekterar en CRM-aktivitet med deltagare till ett komplett response-objekt.
        /// </summary>
        public static Expression<Func<CrmActivity, CrmActivityResponse>> Details => activity => new CrmActivityResponse
        {
            Id = activity.Id,
            RegardingPartyId = activity.RegardingPartyId,
            RegardingPartyName = activity.RegardingParty.Name,
            BusinessRelationshipId = activity.BusinessRelationshipId,
            BusinessRelationshipTypeCode = activity.BusinessRelationship == null ? null : activity.BusinessRelationship.BusinessRelationshipType.Code,
            BusinessRelationshipStageCode = activity.BusinessRelationship == null ? null : activity.BusinessRelationship.BusinessRelationshipStage.Code,
            ActivityType = activity.ActivityType,
            Status = activity.Status,
            Direction = activity.Direction,
            Subject = activity.Subject,
            Description = activity.Description,
            OccurredUtc = activity.OccurredUtc,
            DueUtc = activity.DueUtc,
            CompletedUtc = activity.CompletedUtc,
            OwnerUserId = activity.OwnerUserId,
            ExternalSystem = activity.ExternalSystem,
            ExternalRecordId = activity.ExternalRecordId,
            CreateDateUtc = activity.CreateDateUtc,
            Participants = activity.Participants
                .OrderBy(x => x.Role)
                .ThenBy(x => x.Party.Name)
                .Select(x => new CrmActivityParticipantResponse
                {
                    Id = x.Id,
                    CrmActivityId = x.CrmActivityId,
                    PartyId = x.PartyId,
                    PartyName = x.Party.Name,
                    Role = x.Role
                })
                .ToList()
        };
    }
}