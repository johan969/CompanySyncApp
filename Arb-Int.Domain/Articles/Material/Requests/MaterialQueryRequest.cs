namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för materials.
    /// </summary>
    public sealed class MaterialQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot name, slug och description.
        /// <para>Material har varken code eller normaliserat namn.</para>
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till en normaliserad materialtyp.
        /// </summary>
        public int? MaterialTypeId { get; init; }

        /// <summary>
        /// Anger om inaktiva materials ska inkluderas.
        /// </summary>
        public bool IncludeInactive { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public MaterialSortField SortBy { get; init; } = MaterialSortField.Name;

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
    /// Anger tillgängliga sorteringsfält för materials.
    /// </summary>
    public enum MaterialSortField
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
        /// Sorterar på active-status.
        /// </summary>
        IsActive = 2
    }
}
