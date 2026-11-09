using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class ProductReference : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public Guid ReferenceEntryId { get; set; }
        public ReferenceEntry ReferenceEntry { get; set; } = null!;

        public int SortOrder { get; set; }
    }
}