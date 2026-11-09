using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// CrmActivity kan hantera:
    /// <para>anteckningar,<br />
    /// e-post,<br />
    /// telefonsamtal,<br />
    /// möten,<br />
    /// uppgifter,<br />
    /// webbplatsgranskningar,<br />
    /// dokumentgranskningar.</para>
    /// </summary>
    public class CrmActivity :
        IEntity<Guid>,
        ICreateTracker,
        IActionTracker
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Den primära part som aktiviteten gäller.
        /// </summary>
        public Guid RegardingPartyId { get; set; }
        public Party RegardingParty { get; set; } = null!;

        /// <summary>
        /// Valfri specifik affärsrelation.
        /// <para>Exempelvis kan ett möte gälla leverantörsrelationen men inte
        /// samma organisations köparrelation.</para>
        /// </summary>
        public Guid? BusinessRelationshipId { get; set; }

        public BusinessRelationship? BusinessRelationship { get; set; }

        public CrmActivityType ActivityType { get; set; }

        public CrmActivityStatus Status { get; set; }

        public CrmActivityDirection Direction { get; set; }

        public string Subject { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Tidpunkten då samtalet, mötet, mailet eller anteckningen faktiskt inträffade.
        /// </summary>
        public DateTimeOffset? OccurredUtc { get; set; }

        public DateTimeOffset? DueUtc { get; set; }

        public DateTimeOffset? CompletedUtc { get; set; }

        public Guid? OwnerUserId { get; set; }

        /// <summary>
        /// Exempelvis gmail, outlook, teams eller legacy-crm.
        /// </summary>
        public string? ExternalSystem { get; set; }

        public string? ExternalRecordId { get; set; }

        public DateTimeOffset? CreateDateUtc { get; set; }

        public string? ActionName { get; set; }

        public DateTimeOffset? ActionDateUtc { get; set; }

        public Guid? ActionUserId { get; set; }

        public ICollection<CrmActivityParticipant> Participants { get; set; } = [];
        /// <summary>
        /// Schemalagda worker-uppgifter som hör till CRM-aktiviteten.
        /// </summary>
        public List<CrmActivityScheduledTaskLink> ScheduledTaskLinks { get; set; } = [];
    }
}