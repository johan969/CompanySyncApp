namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller nytt affärsmässigt state för en BuyerDemand.
    /// </summary>
    public sealed class ChangeBuyerDemandStateRequest
    {
        /// <summary>
        /// Ny kunskapsnivå.
        /// </summary>
        public BuyerDemandKnowledgeLevel KnowledgeLevel { get; init; }

        /// <summary>
        /// Ny status.
        /// </summary>
        public BuyerDemandStatus Status { get; init; }

        /// <summary>
        /// Identifierar eventuell evidens som motiverar förändringen.
        /// </summary>
        public Guid? BuyerDemandEvidenceId { get; init; }

        /// <summary>
        /// Confidence som ska gälla efter förändringen.
        /// </summary>
        public decimal? Confidence { get; init; }

        /// <summary>
        /// Tidpunkt då förändringen skedde. Aktuell UTC-tid används om värdet saknas.
        /// </summary>
        public DateTimeOffset? ChangedAtUtc { get; init; }

        /// <summary>
        /// Identifierar användaren som gjorde förändringen.
        /// </summary>
        public Guid? ChangedByUserId { get; init; }

        /// <summary>
        /// Anteckningar om state-förändringen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa version.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}