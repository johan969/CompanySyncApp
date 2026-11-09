using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class CharacteristicType : NamedReferenceEntity, IEntity<int>, ISortOrder
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }

        public ICollection<Characteristic> Characteristics { get; set; } = [];
    }
}