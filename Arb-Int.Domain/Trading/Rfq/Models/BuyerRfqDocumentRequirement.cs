using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqDocumentRequirement : BuyerRfqRequirement
    {
        public int DocumentTypeId { get; set; }
        public RequirementOperator Operator { get; set; }
        public bool MustBeLotSpecific { get; set; }
        public bool MustBeIssuedByAccreditedParty { get; set; }
        public int? MaximumAgeDays { get; set; }
        public DocumentType DocumentType { get; set; } = null!;
    }
}
