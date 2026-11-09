using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för document types.
    /// </summary>
    public sealed class DocumentTypeQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot code, name och description.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till en document category.
        /// </summary>
        public DocumentCategory? Category { get; init; }

        /// <summary>
        /// Anger om inaktiva document types ska inkluderas.
        /// </summary>
        public bool IncludeInactive { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public DocumentTypeSortField SortBy { get; init; } = DocumentTypeSortField.Name;

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
    /// Anger tillgängliga sorteringsfält för document types.
    /// </summary>
    public enum DocumentTypeSortField
    {
        /// <summary>
        /// Sorterar på name.
        /// </summary>
        Name = 0,

        /// <summary>
        /// Sorterar på code.
        /// </summary>
        Code = 1,

        /// <summary>
        /// Sorterar på category.
        /// </summary>
        Category = 2,

        /// <summary>
        /// Sorterar på active-status.
        /// </summary>
        IsActive = 3
    }
}
