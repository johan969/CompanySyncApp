using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class MaterialConstituent : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid MaterialId { get; set; }
        public Material Material { get; set; } = null!;

        public Guid ConstituentId { get; set; }
        public Constituent Constituent { get; set; } = null!;

        public ContentEvidenceLevel EvidenceLevel { get; set; }

        public string? Notes { get; set; }
    }
}