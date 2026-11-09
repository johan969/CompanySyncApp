namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar metadata för en BuyerDemand.
    /// </summary>
    public sealed class UpdateBuyerDemandRequest
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
        /// Confidence mellan 0 och 1.
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
        /// Concurrency-token från senast lästa version.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}