namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för material types.
    /// </summary>
    public sealed class MaterialTypeQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot code, name och description.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till barn av en given material type.
        /// </summary>
        public int? ParentMaterialTypeId { get; init; }

        /// <summary>
        /// Begränsar resultatet till råvarutyper eller förädlade typer.
        /// </summary>
        public bool? IsRawMaterial { get; init; }

        /// <summary>
        /// Anger om inaktiva material types ska inkluderas.
        /// </summary>
        public bool IncludeInactive { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public MaterialTypeSortField SortBy { get; init; } = MaterialTypeSortField.SortOrder;

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
    /// Anger tillgängliga sorteringsfält för material types.
    /// </summary>
    public enum MaterialTypeSortField
    {
        /// <summary>
        /// Sorterar på den kurerade visningsordningen.
        /// </summary>
        SortOrder = 0,

        /// <summary>
        /// Sorterar på name.
        /// </summary>
        Name = 1,

        /// <summary>
        /// Sorterar på code.
        /// </summary>
        Code = 2,

        /// <summary>
        /// Sorterar på active-status.
        /// </summary>
        IsActive = 3
    }
}
