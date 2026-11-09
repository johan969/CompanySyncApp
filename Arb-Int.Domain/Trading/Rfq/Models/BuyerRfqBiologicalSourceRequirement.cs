using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqBiologicalSourceRequirement : BuyerRfqRequirement
    {
        public Guid BiologicalSourceId { get; set; }
        public RequirementOperator Operator { get; set; }
        public BiologicalSource BiologicalSource { get; set; } = null!;
    }
}
