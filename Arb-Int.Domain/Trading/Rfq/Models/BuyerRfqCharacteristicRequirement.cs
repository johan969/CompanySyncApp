using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqCharacteristicRequirement : BuyerRfqRequirement
    {
        public Guid CharacteristicId { get; set; }
        public RequirementOperator Operator { get; set; }
        public string? ExpectedTextValue { get; set; }
        public decimal? ExpectedNumericValue { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public Characteristic Characteristic { get; set; } = null!;
        public UnitOfMeasure? UnitOfMeasure { get; set; }
    }
}
