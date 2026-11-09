using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqLogisticsRequirement : BuyerRfqRequirement
    {
        public LogisticsRequirementKind Kind { get; set; }
        public RequirementOperator Operator { get; set; }
        public decimal? NumericValue { get; set; }
        public string? TextValue { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public int? IncotermRuleId { get; set; }
        public UnitOfMeasure? UnitOfMeasure { get; set; }
        public IncotermRule? IncotermRule { get; set; }
    }
}
