namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller data för att lägga till ett geografiskt ursprung på en inventory lot.
    /// </summary>
    public sealed class CreateInventoryLotOriginRequest
    {
        /// <summary>
        /// Identifierar den geografiska plats som ska kopplas till loten.
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
        /// Anger om platsen är primär inom loten och den valda rollen.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Anger proportion som decimal mellan 0 och 1 när den är känd.
        /// <para>Exempel:<br />
        /// 0.75 motsvarar 75 procent.</para>
        /// </summary>
        public decimal? Proportion { get; init; }

        /// <summary>
        /// Innehåller anteckningar om origin-posten.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token för inventory lot-aggregatet.
        /// </summary>
        public uint InventoryLotRowVersion { get; init; }
    }
}
