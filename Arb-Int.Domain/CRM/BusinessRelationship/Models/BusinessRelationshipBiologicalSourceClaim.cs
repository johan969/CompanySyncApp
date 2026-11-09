using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Ett påstående eller en känd relation mellan motparten och
    /// en biologisk källa.
    /// <para>Samma modell används för både supply- och demand-sidan.</para>
    /// <para>Exempel:</para>
    /// <para>Supplier:<br />
    ///     Cultivates Alaria<br />
    ///     Supplies Alaria esculenta</para>
    /// <para>Buyer:<br />
    ///     SeeksToBuy Alaria esculenta<br />
    ///     Uses Ascophyllum nodosum</para>
    /// </summary>
    public class BusinessRelationshipBiologicalSourceClaim :
        IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid BusinessRelationshipId { get; set; }

        public BusinessRelationship BusinessRelationship { get; set; } = null!;

        /// <summary>
        /// Namnet exakt så som det stod i källan.
        /// <para>Exempel:<br />
        /// Alaria<br />
        /// Alaria esculenta<br />
        /// Atlantic wakame<br />
        /// brown kelp</para>
        /// </summary>
        public string DeclaredName { get; set; } = null!;

        public string NormalizedDeclaredName { get; set; } = null!;

        /// <summary>
        /// Normaliserad intern biologisk källa.<br />
        /// Nullable tills en säker normalisering har gjorts.
        /// <para>Kan peka på genus-, art- eller annan nivå.</para>
        /// </summary>
        public Guid? BiologicalSourceId { get; set; }
        public BiologicalSource? BiologicalSource { get; set; }

        public BiologicalSourceRelationshipKind RelationshipKind { get; set; }

        public ClaimMappingStatus MappingStatus { get; set; }

        public decimal? MappingConfidence { get; set; }

        public ClaimVerificationStatus VerificationStatus { get; set; }

        public DateTimeOffset FirstObservedUtc { get; set; }

        public DateTimeOffset LastObservedUtc { get; set; }

        public DateTimeOffset? LastVerifiedUtc { get; set; }

        public string? Notes { get; set; }

        public ICollection<BusinessRelationshipBiologicalSourceClaimEvidence> Evidence { get; set; } = [];
        public ICollection<BusinessRelationshipBiologicalSourceClaimRegion> RegionLinks { get; set; } = [];
    }
}