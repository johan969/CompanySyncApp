using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    public class BusinessRelationshipType : NamedReferenceEntity, IEntity<int>
    {
        public int Id { get; set; }

        public ICollection<BusinessRelationshipStage> Stages { get; set; } = [];

        public ICollection<BusinessRelationship> Relationships { get; set; } = [];
    }
}