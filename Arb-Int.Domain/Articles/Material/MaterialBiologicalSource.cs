using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class MaterialBiologicalSource : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid MaterialId { get; set; }
        public Material Material { get; set; } = null!;

        public Guid BiologicalSourceId { get; set; }
        public BiologicalSource BiologicalSource { get; set; } = null!;

        public MaterialBiologicalSourceRole Role { get; set; }

        /// <summary>
        /// Valfri deklarerad andel i procent, exempelvis för en
        /// blandad biologisk råvara.
        /// </summary>
        public decimal? ProportionPercent { get; set; }

        public string? Notes { get; set; }
    }
}