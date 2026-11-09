using Arb_Int.Domain.Base;
using Arb_Int.Domain.Taxonomy;

namespace Arb_Int.Domain.Articles
{
    public class BiologicalSource :
        IEntity<Guid>,
        INamed,
        IDescribable,
        ICreateTracker,
        IActionTracker,
        INormalizedName
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Namnet så som det visas eller importerades.
        /// </summary>
        public string Name { get; set; } = null!;

        public string NormalizedName { get; set; } = null!;

        /// <summary>
        /// Stabil import- och URL-nyckel.
        /// </summary>
        public string Slug { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public BiologicalSourceRecordKind RecordKind { get; set; }

        /// <summary>
        /// Används även när TaxonId är null, exempelvis för en
        /// importerad kommersiell grupp som uppges ligga på genusnivå.
        /// </summary>
        public TaxonRank? DeclaredRank { get; set; }

        /// <summary>
        /// Nullable eftersom en kommersiell grupp eller unresolved post
        /// inte alltid kan kopplas till ett enskilt taxon.
        /// </summary>
        public Guid? TaxonId { get; set; }
        public Taxon? Taxon { get; set; }

        /// <summary>
        /// Den importerade eller innehållsmässiga föräldern.
        /// <para>Exempel:<br />
        /// Alaria<br />
        ///   ├─ Alaria esculenta<br />
        ///   ├─ Alaria marginata<br />
        ///   └─ Alaria sp.</para>
        /// </summary>
        public Guid? ParentBiologicalSourceId { get; set; }
        public BiologicalSource? ParentBiologicalSource { get; set; }

        public RegulatoryRiskLevel? RegulatoryRisk { get; set; }

        public string? RegulatoryRiskReason { get; set; }

        public string? TypicalHabitat { get; set; }

        public string? DistributionSummary { get; set; }

        public DateTimeOffset? CreateDateUtc { get; set; }

        public string? ActionName { get; set; }

        public DateTimeOffset? ActionDateUtc { get; set; }

        public Guid? ActionUserId { get; set; }

        public ICollection<BiologicalSource> Children { get; set; } = [];

        public ICollection<BiologicalSourceName> AlternativeNames { get; set; } = [];

        public ICollection<BiologicalSourceCharacteristic> CharacteristicLinks { get; set; } = [];

        public ICollection<BiologicalSourceConstituent> ConstituentLinks { get; set; } = [];

        public ICollection<BiologicalSourceRegion> RegionLinks { get; set; } = [];

        public ICollection<BiologicalSourceReference> ReferenceLinks { get; set; } = [];

        public ICollection<MaterialBiologicalSource> MaterialLinks { get; set; } = [];
    }
}