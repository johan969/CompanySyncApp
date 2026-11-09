using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class TradeExecutionCheckpoint : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public int TradeExecutionCheckpointTypeId { get; set; }
        public Guid? ParentCheckpointId { get; set; }
        public Guid? BuyerProposalAcceptanceId { get; set; }
        public Guid? BuyerSalesOrderId { get; set; }
        public Guid? SupplierPurchaseOrderId { get; set; }
        public Guid? SupplySolutionStepId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? CommercialInvoiceId { get; set; }
        public Guid? GoodsResponsibilityTransferId { get; set; }
        public int? ExternalSystemId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public ExecutionCheckpointStatus Status { get; set; }
        public ExecutionCheckpointCompletionMode CompletionMode { get; set; }
        public bool IsRequired { get; set; } = true;
        public int SortOrder { get; set; }
        public Guid? OwnerUserId { get; set; }
        public DateTimeOffset? DueAtUtc { get; set; }
        public DateTimeOffset? StartedAtUtc { get; set; }
        public DateTimeOffset? CompletedAtUtc { get; set; }
        public Guid? CompletedByUserId { get; set; }
        public DateTimeOffset? WaivedAtUtc { get; set; }
        public Guid? WaivedByUserId { get; set; }
        public string? AutomationHandlerKey { get; set; }
        public string? ExternalReference { get; set; }
        public string? CompletionEvidenceJson { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public TradeExecutionCheckpointType TradeExecutionCheckpointType { get; set; } = null!;
        public TradeExecutionCheckpoint? ParentCheckpoint { get; set; }
        public BuyerProposalAcceptance? BuyerProposalAcceptance { get; set; }
        public BuyerSalesOrder? BuyerSalesOrder { get; set; }
        public SupplierPurchaseOrder? SupplierPurchaseOrder { get; set; }
        public SupplySolutionStep? SupplySolutionStep { get; set; }
        public Shipment? Shipment { get; set; }
        public CommercialInvoice? CommercialInvoice { get; set; }
        public GoodsResponsibilityTransfer? GoodsResponsibilityTransfer { get; set; }
        public ExternalSystem? ExternalSystem { get; set; }
        public ICollection<TradeExecutionCheckpoint> ChildCheckpoints { get; set; } = [];
    }
}
