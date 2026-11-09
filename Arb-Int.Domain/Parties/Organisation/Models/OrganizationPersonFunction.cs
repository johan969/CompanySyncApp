using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Parties
{
    public class OrganizationPersonFunction : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid OrganizationPersonId { get; set; }
        public OrganizationPerson OrganizationPerson { get; set; } = null!;

        public int ContactFunctionTypeId { get; set; }
        public ContactFunctionType ContactFunctionType { get; set; } = null!;
    }
}