namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller en historisk förändring av en affärsrelations steg.
    /// </summary>
    public sealed class BusinessRelationshipStageHistoryResponse
    {
        /// <summary>
        /// Identifierar historikraden.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar affärsrelationen.
        /// </summary>
        public Guid BusinessRelationshipId { get; init; }

        /// <summary>
        /// Föregående steg eller null för relationens ursprungliga steg.
        /// </summary>
        public BusinessRelationshipStageResponse? FromStage { get; init; }

        /// <summary>
        /// Det nya steget.
        /// </summary>
        public required BusinessRelationshipStageResponse ToStage { get; init; }

        /// <summary>
        /// Tidpunkt då förändringen gjordes.
        /// </summary>
        public DateTimeOffset ChangedUtc { get; init; }

        /// <summary>
        /// Identifierar användaren som gjorde förändringen.
        /// </summary>
        public Guid? ChangedByUserId { get; init; }

        /// <summary>
        /// Anteckningar om varför steget ändrades.
        /// </summary>
        public string? Notes { get; init; }
    }
}