using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Avgränsar en processkapacitet till en biologisk CRM-claim.
    /// <para>Exempel:<br />
    /// Leverantören kan frystorka Alaria-biomassa.</para>
    /// </summary>
    public class SupplierProcessCapabilityBiologicalSourceScope : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid BusinessRelationshipId { get; set; }

        public Guid SupplierProcessCapabilityId { get; set; }

        public SupplierProcessCapability SupplierProcessCapability { get; set; } = null!;

        public Guid BusinessRelationshipBiologicalSourceClaimId { get; set; }

        public BusinessRelationshipBiologicalSourceClaim BiologicalSourceClaim { get; set; } = null!;

        public string? Notes { get; set; }
    }
}