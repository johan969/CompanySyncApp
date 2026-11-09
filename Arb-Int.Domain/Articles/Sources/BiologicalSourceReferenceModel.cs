using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class BiologicalSourceReference : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid BiologicalSourceId { get; set; }
        public BiologicalSource BiologicalSource { get; set; } = null!;

        public Guid ReferenceEntryId { get; set; }
        public ReferenceEntry ReferenceEntry { get; set; } = null!;

        public int SortOrder { get; set; }
    }
}