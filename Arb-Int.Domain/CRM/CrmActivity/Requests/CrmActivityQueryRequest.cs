namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för CRM-aktiviteter.
    /// </summary>
    public sealed class CrmActivityQueryRequest
    {
        /// <summary>
        /// Begränsar resultatet till en primär Party.
        /// </summary>
        public Guid? RegardingPartyId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en specifik affärsrelation.
        /// </summary>
        public Guid? BusinessRelationshipId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en aktivitetstyp.
        /// </summary>
        public CrmActivityType? ActivityType { get; init; }

        /// <summary>
        /// Begränsar resultatet till en status.
        /// </summary>
        public CrmActivityStatus? Status { get; init; }

        /// <summary>
        /// Begränsar resultatet till en riktning.
        /// </summary>
        public CrmActivityDirection? Direction { get; init; }

        /// <summary>
        /// Begränsar resultatet till en ansvarig användare.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Begränsar resultatet till aktiviteter som inträffat från angiven tidpunkt.
        /// </summary>
        public DateTimeOffset? OccurredFromUtc { get; init; }

        /// <summary>
        /// Begränsar resultatet till aktiviteter som inträffat senast vid angiven tidpunkt.
        /// </summary>
        public DateTimeOffset? OccurredToUtc { get; init; }

        /// <summary>
        /// Begränsar resultatet till aktiviteter som förfaller senast vid angiven tidpunkt.
        /// </summary>
        public DateTimeOffset? DueBeforeUtc { get; init; }

        /// <summary>
        /// Fritext som matchas mot rubrik, beskrivning och externa identifierare.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public CrmActivitySortField SortBy { get; init; } = CrmActivitySortField.OccurredUtc;

        /// <summary>
        /// Anger sorteringsriktning.
        /// </summary>
        public SortDirection SortDirection { get; init; } = SortDirection.Descending;

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
    /// Anger tillgängliga sorteringsfält för CRM-aktiviteter.
    /// </summary>
    public enum CrmActivitySortField
    {
        OccurredUtc = 0,
        DueUtc = 1,
        Subject = 2,
        CreateDateUtc = 3
    }
}