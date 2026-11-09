using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqCommercialRequirement : BuyerRfqRequirement
    {
        public CommercialRequirementKind Kind { get; set; }
        public RequirementOperator Operator { get; set; }
        public decimal? NumericValue { get; set; }
        public string? TextValue { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public int? CurrencyId { get; set; }
        public UnitOfMeasure? UnitOfMeasure { get; set; }
        public Currency? Currency { get; set; }
    }
}
