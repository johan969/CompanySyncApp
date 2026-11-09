using Arb_Int.Domain.Articles;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqMaterialRequirement : BuyerRfqRequirement
    {
        public Guid? MaterialId { get; set; }
        public int? MaterialTypeId { get; set; }
        public MaterialMatchMode MatchMode { get; set; }
        public bool AllowSubstitutes { get; set; }
        public Material? Material { get; set; }
        public MaterialType? MaterialType { get; set; }
    }
}
