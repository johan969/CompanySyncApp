using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class ConstituentType : NamedReferenceEntity, IEntity<int>, ISortOrder
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
        public ICollection<Constituent> Constituents { get; set; } = [];
    }
}