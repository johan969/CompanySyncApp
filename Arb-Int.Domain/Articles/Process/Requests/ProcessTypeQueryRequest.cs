namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för process types.
    /// </summary>
    public sealed class ProcessTypeQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot code, name och description.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Anger om inaktiva process types ska inkluderas.
        /// </summary>
        public bool IncludeInactive { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public ProcessTypeSortField SortBy { get; init; } = ProcessTypeSortField.SortOrder;

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
    /// Anger tillgängliga sorteringsfält för process types.
    /// </summary>
    public enum ProcessTypeSortField
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
