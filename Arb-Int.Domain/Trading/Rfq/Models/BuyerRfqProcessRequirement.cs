using Arb_Int.Domain.Articles;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqProcessRequirement : BuyerRfqRequirement
    {
        public int ProcessTypeId { get; set; }
        public ProcessRequirementMode Mode { get; set; }
        public string? ProcessParametersJson { get; set; }
        public ProcessType ProcessType { get; set; } = null!;
    }
}
