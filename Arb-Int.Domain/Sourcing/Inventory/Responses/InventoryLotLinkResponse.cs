namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en provenance-länk mellan två inventory lots.
    /// </summary>
    public sealed class InventoryLotLinkResponse
    {
        /// <summary>
        /// Identifierar länken.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar input- eller parent-loten.
        /// </summary>
        public Guid ParentInventoryLotId { get; init; }

        /// <summary>
        /// Identifierar output- eller child-loten.
        /// </summary>
        public Guid ChildInventoryLotId { get; init; }

        /// <summary>
        /// Anger hur child-loten härstammar från parent-loten.
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
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
