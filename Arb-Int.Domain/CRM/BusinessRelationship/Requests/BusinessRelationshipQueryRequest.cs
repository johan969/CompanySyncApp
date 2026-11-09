using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för affärsrelationer.
    /// </summary>
    public sealed class BusinessRelationshipQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot motpartens namn och lead source.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till en relationstyp.
        /// </summary>
        public int? BusinessRelationshipTypeId { get; init; }

        /// <summary>
        /// Begränsar resultatet till ett steg.
        /// </summary>
        public int? BusinessRelationshipStageId { get; init; }

        /// <summary>
        /// Begränsar resultatet efter prioritet.
        /// </summary>
        public RelationshipPriority? Priority { get; init; }

        /// <summary>
        /// Begränsar resultatet till en ansvarig användare.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Begränsar resultatet efter om relationen är stängd.
        /// </summary>
        public bool? IsClosed { get; init; }

        /// <summary>
        /// Begränsar resultatet till nästa åtgärd senast vid angiven tidpunkt.
        /// </summary>
        public DateTimeOffset? NextActionBeforeUtc { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public BusinessRelationshipSortField SortBy { get; init; } = BusinessRelationshipSortField.CounterpartyName;

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
        public int PageSize { get; init; } = 50;
    }

    /// <summary>
    /// Anger tillgängliga sorteringsfält för affärsrelationer.
    /// </summary>
    public enum BusinessRelationshipSortField
    {
        CounterpartyName = 0,
        FirstObservedUtc = 1,
        LastContactedUtc = 2,
        NextActionUtc = 3,
        Score = 4
    }
}