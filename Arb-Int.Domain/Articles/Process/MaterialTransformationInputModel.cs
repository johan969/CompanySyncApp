using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class MaterialTransformationInput : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid MaterialTransformationId { get; set; }
        public MaterialTransformation MaterialTransformation { get; set; } = null!;

        public Guid MaterialId { get; set; }
        public Material Material { get; set; } = null!;

        public TransformationInputRole Role { get; set; }

        public decimal? Quantity { get; set; }

        public int? UnitOfMeasureId { get; set; }
        public UnitOfMeasure? UnitOfMeasure { get; set; }

        /// <summary>
        /// Valfri procentandel av processens input eller recept.
        /// </summary>
        public decimal? Percentage { get; set; }

        public int SortOrder { get; set; }

        public string? Notes { get; set; }
    }
}