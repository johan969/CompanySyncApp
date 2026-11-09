namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att skapa en BuyerDemand.
    /// </summary>
    public sealed class CreateBuyerDemandRequest
    {
        /// <summary>
        /// Namnet på efterfrågan.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Beskriver efterfrågan.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Anger initial kunskapsnivå.
        /// </summary>
        public BuyerDemandKnowledgeLevel KnowledgeLevel { get; init; }

        /// <summary>
        /// Anger initial status.
        /// </summary>
        public BuyerDemandStatus Status { get; init; } = BuyerDemandStatus.Active;

        /// <summary>
        /// Initial confidence mellan 0 och 1.
        /// </summary>
        public decimal? Confidence { get; init; }

        /// <summary>
        /// Tidpunkt då efterfrågan börjar gälla.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Tidpunkt då efterfrågan slutar gälla.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Identifierar ansvarig användare.
        /// </summary>
        public Guid? OwnerUserId { get; init; }

        /// <summary>
        /// Interna anteckningar.
        /// </summary>
        public string? InternalNotes { get; init; }

        /// <summary>
        /// Identifierar användaren bakom initialt state.
        /// </summary>
        public Guid? InitialStateChangedByUserId { get; init; }

        /// <summary>
        /// Anteckningar om hur initialt state fastställdes.
        /// </summary>
        public string? InitialStateNotes { get; init; }
    }
}