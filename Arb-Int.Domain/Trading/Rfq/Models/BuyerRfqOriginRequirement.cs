using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Location;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqOriginRequirement : BuyerRfqRequirement
    {
        public Guid? GeographicLocationId { get; set; }
        public Guid? CountryId { get; set; }
        public OriginRole OriginRole { get; set; }
        public RequirementOperator Operator { get; set; }
        public GeographicLocation? GeographicLocation { get; set; }
        public Country? Country { get; set; }
    }
}
