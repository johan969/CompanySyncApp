namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller data för att skapa en provenance-länk mellan två inventory lots.
    /// </summary>
    public sealed class CreateInventoryLotLinkRequest
    {
        /// <summary>
        /// Identifierar input- eller parent-loten.
        /// </summary>
        public Guid ParentInventoryLotId { get; init; }

        /// <summary>
        /// Identifierar hur child-loten härstammar från parent-loten.
        /// </summary>
        public InventoryLotLinkKind Kind { get; init; }

        /// <summary>
        /// Anger använd input-kvantitet när den är känd.
        /// </summary>
        public decimal? InputQuantity { get; init; }

        /// <summary>
        /// Identifierar måttenheten för input-kvantiteten.
        /// </summary>
        public int? QuantityUnitId { get; init; }

        /// <summary>
        /// Anger yield i procent när den är känd.
        /// <para>Exempel:<br />
        /// 82.5 motsvarar 82,5 procent.</para>
        /// </summary>
        public decimal? YieldPercent { get; init; }

        /// <summary>
        /// Identifierar material transformation när länken kommer från en sådan.
        /// </summary>
        public Guid? MaterialTransformationId { get; init; }

        /// <summary>
        /// Innehåller anteckningar om provenance-länken.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token för parent-loten.
        /// </summary>
        public uint ParentInventoryLotRowVersion { get; init; }

        /// <summary>
        /// Concurrency-token för child-loten.
        /// </summary>
        public uint ChildInventoryLotRowVersion { get; init; }
    }
}
