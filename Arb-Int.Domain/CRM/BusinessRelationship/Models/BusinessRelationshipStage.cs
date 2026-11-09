using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    public class BusinessRelationshipStage : IEntity<int>, INamed, ICode
    {
        public int Id { get; set; }

        public int BusinessRelationshipTypeId { get; set; }

        public BusinessRelationshipType BusinessRelationshipType { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsClosed { get; set; }

        public bool IsSuccessful { get; set; }

        public ICollection<BusinessRelationship> Relationships { get; set; } = [];
    }
}