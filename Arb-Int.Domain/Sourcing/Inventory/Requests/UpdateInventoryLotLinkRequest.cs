namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar metadata för en provenance-länk mellan inventory lots.
    /// </summary>
    public sealed class UpdateInventoryLotLinkRequest
    {
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
        /// Concurrency-token för provenance-länken.
        /// </summary>
        public uint RowVersion { get; init; }

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
