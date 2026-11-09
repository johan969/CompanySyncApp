using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Crm
{
    public class BusinessRelationshipContact : IEntity<Guid>, ISystemVersioned
    {
        public Guid Id { get; set; }

        public Guid BusinessRelationshipId { get; set; }

        public BusinessRelationship BusinessRelationship { get; set; } = null!;

        public Guid OrganizationPersonId { get; set; }

        public OrganizationPerson OrganizationPerson { get; set; } = null!;

        public BusinessRelationshipContactRole Role { get; set; }

        public bool IsPrimary { get; set; }

        public string? Notes { get; set; }
    }
}