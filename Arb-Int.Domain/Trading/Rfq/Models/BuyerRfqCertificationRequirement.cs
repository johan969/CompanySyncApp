using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqCertificationRequirement : BuyerRfqRequirement
    {
        public int CertificationSchemeId { get; set; }
        public RequirementOperator Operator { get; set; }
        public CertificationScope RequiredScope { get; set; }
        public int? CertificationSchemeJurisdictionId { get; set; }
        public bool MustBeValidAtDelivery { get; set; } = true;
        public CertificationScheme CertificationScheme { get; set; } = null!;
        public CertificationSchemeJurisdiction? CertificationSchemeJurisdiction { get; set; }
    }
}
