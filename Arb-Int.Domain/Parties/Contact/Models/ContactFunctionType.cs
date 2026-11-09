using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Parties
{
    public class ContactFunctionType : NamedReferenceEntity, IEntity<int>, ISortOrder
    {
        public int Id { get; set; }

        /// <summary>
        /// Exempel:<br />
        /// purchasing<br />
        /// sales<br />
        /// technical<br />
        /// quality<br />
        /// finance<br />
        /// logistics<br />
        /// sustainability<br />
        /// executive
        /// </summary>
        public int SortOrder { get; set; }

        public ICollection<OrganizationPersonFunction> OrganizationPersonLinks { get; set; } = [];
    }
}