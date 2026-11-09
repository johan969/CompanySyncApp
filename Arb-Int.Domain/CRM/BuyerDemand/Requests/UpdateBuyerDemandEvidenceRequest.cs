namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar information för BuyerDemand-evidens.
    /// </summary>
    public sealed class UpdateBuyerDemandEvidenceRequest
    {
        /// <summary>
        /// Anger evidensens källtyp.
        /// </summary>
        public EvidenceSourceKind SourceKind { get; init; }

        /// <summary>
        /// Anger evidensens position.
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
        /// Eventuell extern identifierare.
        /// </summary>
        public string? SourceExternalId { get; init; }

        /// <summary>
        /// Relevant utdrag från källan.
        /// </summary>
        public string? Excerpt { get; init; }

        /// <summary>
        /// Interna anteckningar.
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
        /// Concurrency-token från senast lästa version.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}