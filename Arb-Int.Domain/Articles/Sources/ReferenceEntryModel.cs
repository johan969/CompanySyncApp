using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class ReferenceEntry : IEntity<Guid>
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Öppen typkod från källan, exempelvis:
        /// scientific, government, regulatory, industry eller technical.
        /// </summary>
        public string Type { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Url { get; set; }

        public string? Notes { get; set; }

        /// <summary>
        /// Deterministisk nyckel som skapas av importören,
        /// exempelvis SHA-256 av normaliserad Type + Title + Url.
        /// </summary>
        public string ReferenceKey { get; set; } = null!;

        public ICollection<BiologicalSourceReference> BiologicalSourceLinks { get; set; } = [];

        public ICollection<MaterialReference> MaterialLinks { get; set; } = [];
        public ICollection<ProductReference> ProductLinks { get; set; } = [];
    }
}