using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    public class BusinessRelationshipBiologicalSourceClaimEvidence : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid BusinessRelationshipBiologicalSourceClaimId { get; set; }

        public BusinessRelationshipBiologicalSourceClaim Claim { get; set; } = null!;

        public ClaimEvidenceType EvidenceType { get; set; }

        public string SourceSystem { get; set; } = null!;

        public string SourceRecordKey { get; set; } = null!;

        public string? SourceUrl { get; set; }

        public DateTimeOffset ObservedUtc { get; set; }

        public string? Notes { get; set; }

        public string? ContentHash { get; set; }
    }
}