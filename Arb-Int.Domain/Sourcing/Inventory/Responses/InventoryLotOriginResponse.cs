namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en geografisk origin-post för en inventory lot.
    /// </summary>
    public sealed class InventoryLotOriginResponse
    {
        /// <summary>
        /// Identifierar origin-posten.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar loten.
        /// </summary>
        public Guid InventoryLotId { get; init; }

        /// <summary>
        /// Identifierar den geografiska platsen.
        /// </summary>
        public Guid GeographicLocationId { get; init; }

        /// <summary>
        /// Anger vilken origin-roll platsen representerar.
        /// </summary>
        public OriginRole Role { get; init; }

        /// <summary>
        /// Anger hur origin-uppgiften har verifierats.
        /// </summary>
        public OriginVerificationStatus VerificationStatus { get; init; }

        /// <summary>
        /// Anger om origin-posten är primär inom sin roll.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Anger proportion som decimal mellan 0 och 1 när den är känd.
        /// </summary>
        public decimal? Proportion { get; init; }

        /// <summary>
        /// Innehåller anteckningar om origin-posten.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
