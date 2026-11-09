using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class UnitOfMeasure : NamedReferenceEntity, IEntity<int>, ISortOrder
    {
        public int Id { get; set; }

        public string Symbol { get; set; } = null!;

        /// <summary>
        /// Exempelvis mass, volume, count eller concentration.
        /// </summary>
        public UnitDimension? Dimension { get; set; }
        public decimal FactorToBaseUnit { get; set; }
        public bool IsBaseUnit { get; set; }
        public int SortOrder { get; set; }


        public ICollection<MaterialTransformationInput> TransformationInputs { get; set; } = [];

        public ICollection<MaterialTransformationOutput> TransformationOutputs { get; set; } = [];
    }
}