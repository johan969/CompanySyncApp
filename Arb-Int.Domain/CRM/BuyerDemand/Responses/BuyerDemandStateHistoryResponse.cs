namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller en historisk förändring av en BuyerDemands affärsmässiga state.
    /// </summary>
    public sealed class BuyerDemandStateHistoryResponse
    {
        /// <summary>
        /// Identifierar historikraden.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar efterfrågan.
        /// </summary>
        public Guid BuyerDemandId { get; init; }

        /// <summary>
        /// Föregående kunskapsnivå eller null för ursprungligt state.
        /// </summary>
        public BuyerDemandKnowledgeLevel? FromKnowledgeLevel { get; init; }

        /// <summary>
        /// Ny kunskapsnivå.
        /// </summary>
        public BuyerDemandKnowledgeLevel ToKnowledgeLevel { get; init; }

        /// <summary>
        /// Föregående status eller null för ursprungligt state.
        /// </summary>
        public BuyerDemandStatus? FromStatus { get; init; }

        /// <summary>
        /// Ny status.
        /// </summary>
        public BuyerDemandStatus ToStatus { get; init; }

        /// <summary>
        /// Identifierar eventuell evidens som motiverade förändringen.
        /// </summary>
        public Guid? BuyerDemandEvidenceId { get; init; }

        /// <summary>
        /// Confidence som gällde vid state-förändringen.
        /// </summary>
        public decimal? Confidence { get; init; }

        /// <summary>
        /// Tidpunkt då förändringen gjordes.
        /// </summary>
        public DateTimeOffset ChangedAtUtc { get; init; }

        /// <summary>
        /// Identifierar användaren som gjorde förändringen.
        /// </summary>
        public Guid? ChangedByUserId { get; init; }

        /// <summary>
        /// Anteckningar om state-förändringen.
        /// </summary>
        public string? Notes { get; init; }
    }
}