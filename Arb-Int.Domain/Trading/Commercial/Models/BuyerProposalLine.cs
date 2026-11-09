using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class BuyerProposalLine : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid BuyerProposalOptionId { get; set; }
        public Guid BuyerRfqLineId { get; set; }
        public Guid? SupplySolutionLineId { get; set; }
        public Guid? MaterialId { get; set; }
        public Guid? ProductId { get; set; }
        public int LineNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Quantity { get; set; }
        public int QuantityUnitId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NetAmount { get; set; }
        public decimal? TaxPercent { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public DateTimeOffset? EstimatedDeliveryFromUtc { get; set; }
        public DateTimeOffset? EstimatedDeliveryToUtc { get; set; }
        public string? BuyerFacingNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BuyerProposalOption BuyerProposalOption { get; set; } = null!;
        public BuyerRfqLine BuyerRfqLine { get; set; } = null!;
        public SupplySolutionLine? SupplySolutionLine { get; set; }
        public Material? Material { get; set; }
        public Product? Product { get; set; }
        public UnitOfMeasure QuantityUnit { get; set; } = null!;
    }
}
