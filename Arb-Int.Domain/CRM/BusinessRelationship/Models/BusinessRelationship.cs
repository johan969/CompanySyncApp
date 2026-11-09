using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// En organisation kan ha två parallella relationer:
    /// <para>BusinessRelationship 1:<br />
    /// Counterparty = Example Algae AB<br />
    /// Type = Supplier<br />
    /// Stage = Active</para>
    /// <para>BusinessRelationship 2:<br />
    /// Counterparty = Example Algae AB<br />
    /// Type = Buyer<br />
    /// Stage = Prospect</para>
    /// </summary>
    public class BusinessRelationship :
        IEntity<Guid>,
        ICreateTracker,
        IActionTracker,
        ISystemVersioned
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Den interna organisation som äger relationen.
        /// </summary>
        public Guid OwningOrganizationId { get; set; }
        public Organization OwningOrganization { get; set; } = null!;

        /// <summary>
        /// Den externa parten.
        /// <para>Vanligen en Organization, men kan även vara en Person,
        /// exempelvis en enskild konsult eller enskild näringsidkare.</para>
        /// </summary>
        public Guid CounterpartyId { get; set; }
        public Party Counterparty { get; set; } = null!;

        public int BusinessRelationshipTypeId { get; set; }

        public BusinessRelationshipType BusinessRelationshipType { get; set; } = null!;

        public int BusinessRelationshipStageId { get; set; }

        public BusinessRelationshipStage BusinessRelationshipStage { get; set; } = null!;

        public RelationshipPriority? Priority { get; set; }

        /// <summary>
        /// Valfri CRM-poäng mellan 0 och 100.
        /// </summary>
        public int? Score { get; set; }

        /// <summary>
        /// Exempelvis supplier-directory, referral, conference,
        /// inbound-request eller manual-research.
        /// </summary>
        public string? LeadSource { get; set; }

        public Guid? OwnerUserId { get; set; }

        public DateTimeOffset FirstObservedUtc { get; set; }

        public DateTimeOffset? FirstContactedUtc { get; set; }

        public DateTimeOffset? LastContactedUtc { get; set; }

        public DateTimeOffset? NextActionUtc { get; set; }

        public DateTimeOffset? ClosedUtc { get; set; }

        public string? Notes { get; set; }

        public DateTimeOffset? CreateDateUtc { get; set; }

        public string? ActionName { get; set; }

        public DateTimeOffset? ActionDateUtc { get; set; }

        public Guid? ActionUserId { get; set; }

        public ICollection<BusinessRelationshipContact> Contacts { get; set; } = [];

        public ICollection<BusinessRelationshipStageHistory> StageHistory { get; set; } = [];

        public ICollection<BusinessRelationshipBiologicalSourceClaim> BiologicalSourceClaims { get; set; } = [];

        public ICollection<CrmActivity> Activities { get; set; } = [];

        public ICollection<BuyerDemand> BuyerDemands { get; set; } = [];
    }
}