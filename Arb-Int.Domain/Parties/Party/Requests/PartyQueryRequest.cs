namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Anger filtrering, sortering och paginering vid sökning efter parter.
    /// </summary>
    public sealed class PartyQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot relevanta namn.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till en viss Party-typ.
        /// </summary>
        public PartyType? PartyType { get; init; }

        /// <summary>
        /// Begränsar resultatet efter aktiv status.
        /// </summary>
        public bool? IsActive { get; init; }

        /// <summary>
        /// Fält som används för sortering.
        /// </summary>
        public PartySortField SortBy { get; init; } = PartySortField.Name;

        /// <summary>
        /// Sorteringsriktning.
        /// </summary>
        public SortDirection SortDirection { get; init; } = SortDirection.Ascending;

        /// <summary>
        /// Sidnummer med start på 1.
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Maximalt antal poster per sida.
        /// </summary>
        public int PageSize { get; init; } = 50;
    }

    /// <summary>
    /// Anger tillgängliga sorteringsfält för Party-sökningar.
    /// </summary>
    public enum PartySortField
    {
        Name = 0,
        CreateDateUtc = 1
    }

}
