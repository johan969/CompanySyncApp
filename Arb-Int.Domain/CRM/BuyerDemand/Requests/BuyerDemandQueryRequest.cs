namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för BuyerDemand.
    /// </summary>
    public sealed class BuyerDemandQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot demandens namn och motpartens namn.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till en specifik buyer-relation.
        /// </summary>
        public Guid? BusinessRelationshipId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en specifik motpart.
        /// </summary>
        public Guid? CounterpartyId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en kunskapsnivå.
        /// </summary>
        public BuyerDemandKnowledgeLevel? KnowledgeLevel { get; init; }

        /// <summary>
        /// Begränsar resultatet till en demand-status.
        /// </summary>
        public BuyerDemandStatus? Status { get; init; }

        /// <summary>
        /// Begränsar resultatet till en ansvarig användare.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Begränsar resultatet till lägsta confidence.
        /// </summary>
        public decimal? MinConfidence { get; init; }

        /// <summary>
        /// Begränsar resultatet till högsta confidence.
        /// </summary>
        public decimal? MaxConfidence { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public BuyerDemandSortField SortBy { get; init; } = BuyerDemandSortField.Name;

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
    /// Anger tillgängliga sorteringsfält för BuyerDemand.
    /// </summary>
    public enum BuyerDemandSortField
    {
        Name = 0,
        CounterpartyName = 1,
        Confidence = 2,
        LastConfirmedAtUtc = 3,
        CreateDateUtc = 4
    }
}
