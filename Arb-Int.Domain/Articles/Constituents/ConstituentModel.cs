using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class Constituent : IEntity<Guid>, INamed, INormalizedName
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string NormalizedName { get; set; } = null!;

        public int? ConstituentTypeId { get; set; }
        public ConstituentType? ConstituentType { get; set; }

        public ICollection<BiologicalSourceConstituent> BiologicalSourceLinks { get; set; } = [];

        public ICollection<MaterialConstituent> MaterialLinks { get; set; } = [];
    }
}