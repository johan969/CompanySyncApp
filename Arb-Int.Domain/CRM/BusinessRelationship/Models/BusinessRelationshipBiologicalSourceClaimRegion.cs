using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// En geografisk plats som hör till det biologiska påståendet.
    /// <para>Exempel:<br />
    /// Supplier A odlar Alaria i Norge.<br />
    /// Supplier B vildskördar Chondrus crispus i Bretagne.</para>
    /// </summary>
    public class BusinessRelationshipBiologicalSourceClaimRegion : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid BusinessRelationshipBiologicalSourceClaimId { get; set; }

        public BusinessRelationshipBiologicalSourceClaim Claim { get; set; } = null!;

        public Guid GeographicLocationId { get; set; }

        public GeographicLocation GeographicLocation { get; set; } = null!;

        public BiologicalSourceClaimLocationRole Role { get; set; }

        public bool IsPrimary { get; set; }

        public string? Notes { get; set; }
    }
}