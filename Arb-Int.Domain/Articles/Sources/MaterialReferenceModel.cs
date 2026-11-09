using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class MaterialReference : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid MaterialId { get; set; }
        public Material Material { get; set; } = null!;

        public Guid ReferenceEntryId { get; set; }
        public ReferenceEntry ReferenceEntry { get; set; } = null!;

        public int SortOrder { get; set; }
    }
}