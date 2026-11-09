namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller evidens som stödjer, motsäger eller beskriver en BuyerDemand.
    /// </summary>
    public sealed class BuyerDemandEvidenceResponse
    {
        /// <summary>
        /// Identifierar evidensposten.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar efterfrågan som evidensen tillhör.
        /// </summary>
        public Guid BuyerDemandId { get; init; }

        /// <summary>
        /// Anger vilken typ av källa evidensen kommer från.
        /// </summary>
        public EvidenceSourceKind SourceKind { get; init; }

        /// <summary>
        /// Anger om evidensen stödjer, motsäger eller är neutral till efterfrågan.
        /// </summary>
        public EvidencePosition Position { get; init; }

        /// <summary>
        /// Identifierar eventuell ReferenceEntry.
        /// </summary>
        public Guid? ReferenceEntryId { get; init; }

        /// <summary>
        /// Identifierar eventuell CRM-aktivitet.
        /// </summary>
        public Guid? CrmActivityId { get; init; }

        /// <summary>
        /// Eventuell URI till källan.
        /// </summary>
        public string? SourceUri { get; init; }

        /// <summary>
        /// Eventuell extern identifierare för källposten.
        /// </summary>
        public string? SourceExternalId { get; init; }

        /// <summary>
        /// Relevant utdrag från källan.
        /// </summary>
        public string? Excerpt { get; init; }

        /// <summary>
        /// Interna anteckningar om evidensen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Confidence mellan 0 och 1.
        /// </summary>
        public decimal? Confidence { get; init; }

        /// <summary>
        /// Tidpunkt då evidensen observerades.
        /// </summary>
        public DateTimeOffset ObservedAtUtc { get; init; }

        /// <summary>
        /// Eventuell hash av evidensen.
        /// </summary>
        public string? EvidenceHash { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}