using Arb_Int.Domain.Taxonomy;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för biologiska källor.
    /// </summary>
    public sealed class BiologicalSourceQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot name, slug, description och alternativa namn.
        /// <para>Alternativa namn gör att trivialnamn som "kelp" hittar rätt art.</para>
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till en posttyp.
        /// <para>Biologiska källor har ingen active-flagga. Filtrera på
        /// RecordKind för att utesluta legacy-poster.</para>
        /// </summary>
        public BiologicalSourceRecordKind? RecordKind { get; init; }

        /// <summary>
        /// Begränsar resultatet till en deklarerad taxonomisk nivå.
        /// </summary>
        public TaxonRank? DeclaredRank { get; init; }

        /// <summary>
        /// Begränsar resultatet till direkta barn av en given källa.
        /// </summary>
        public Guid? ParentBiologicalSourceId { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public BiologicalSourceSortField SortBy { get; init; } = BiologicalSourceSortField.Name;

        /// <summary>
        /// Anger sorteringsriktning.
        /// </summary>
        public SortDirection SortDirection { get; init; } = SortDirection.Ascending;

        /// <summary>
        /// Sidnummer med start på 1.
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Maximalt antal poster per sida.
        /// </summary>
        public int PageSize { get; init; } = 100;
    }

    /// <summary>
    /// Anger tillgängliga sorteringsfält för biologiska källor.
    /// </summary>
    public enum BiologicalSourceSortField
    {
        /// <summary>
        /// Sorterar på name.
        /// </summary>
        Name = 0,

        /// <summary>
        /// Sorterar på slug.
        /// </summary>
        Slug = 1,

        /// <summary>
        /// Sorterar på posttyp.
        /// </summary>
        RecordKind = 2
    }
}
