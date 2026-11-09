namespace Arb_Int.Domain
{
    /// <summary>
    /// Innehåller en sida av ett paginerat resultat.
    /// </summary>
    public sealed class PagedResult<T>
    {
        /// <summary>
        /// Objekten på aktuell sida.
        /// </summary>
        public required IReadOnlyList<T> Items { get; init; }

        /// <summary>
        /// Aktuellt sidnummer med start på 1.
        /// </summary>
        public int Page { get; init; }

        /// <summary>
        /// Maximalt antal objekt per sida.
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Totalt antal objekt före paginering.
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// Totalt antal tillgängliga sidor.
        /// </summary>
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}