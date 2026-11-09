using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Taxonomy
{
    public class Taxon : IEntity<Guid>, INamed, INormalizedName, ISlug
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Det taxonomiska namnet.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Normaliserat namn för jämförelser och importer.
        /// Exempelvis "helianthus annuus".
        /// </summary>
        public string NormalizedName { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public TaxonRank Rank { get; set; }

        public TaxonomicStatus Status { get; set; }

        /// <summary>
        /// Deterministisk nyckel för hela taxonomivägen.
        ///
        /// Exempel:
        /// kingdom:plantae/phylum:tracheophyta/class:magnoliopsida/
        /// order:asterales/family:asteraceae/genus:helianthus/
        /// species:helianthus-annuus
        /// </summary>
        public string PathKey { get; set; } = null!;

        public Guid? ParentTaxonId { get; set; }
        public Taxon? ParentTaxon { get; set; }

        /// <summary>
        /// Används när detta taxon är en synonym eller äldre klassificering.
        /// </summary>
        public Guid? AcceptedTaxonId { get; set; }
        public Taxon? AcceptedTaxon { get; set; }

        public ICollection<Taxon> Children { get; set; } = [];

        public ICollection<Taxon> Synonyms { get; set; } = [];

        public ICollection<BiologicalSource> BiologicalSources { get; set; } = [];
    }
}