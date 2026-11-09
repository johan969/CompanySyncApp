using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    public class SupplierProcessCapabilityEvidence : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid SupplierProcessCapabilityId { get; set; }

        public SupplierProcessCapability SupplierProcessCapability { get; set; } = null!;

        public SourcingEvidenceType EvidenceType { get; set; }

        public string SourceSystem { get; set; } = null!;

        public string SourceRecordKey { get; set; } = null!;

        public string? SourceUrl { get; set; }

        public DateTimeOffset FirstObservedUtc { get; set; }

        public DateTimeOffset LastObservedUtc { get; set; }

        public string? Notes { get; set; }

        public string? ContentHash { get; set; }
    }
}