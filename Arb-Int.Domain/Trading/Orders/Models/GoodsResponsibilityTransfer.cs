using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Registrerar ägande, risk och fysisk custody separat eftersom de inte alltid övergår samtidigt.
    /// </summary>
    public class GoodsResponsibilityTransfer : IEntity<Guid>, INamed, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public ResponsibilityTransferKind Kind { get; set; }
        public ResponsibilityTransferStatus Status { get; set; }
        public Guid FromPartyId { get; set; }
        public Guid ToPartyId { get; set; }
        public Guid? BuyerSalesOrderId { get; set; }
        public Guid? SupplierPurchaseOrderId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public Guid? InventoryLotId { get; set; }
        public int? IncotermRuleId { get; set; }
        public Guid? LocationId { get; set; }
        public DateTimeOffset? PlannedTransferAtUtc { get; set; }
        public DateTimeOffset? ActualTransferAtUtc { get; set; }
        public string? ContractualBasis { get; set; }
        public Guid? EvidenceSupplyDocumentId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public Party FromParty { get; set; } = null!;
        public Party ToParty { get; set; } = null!;
        public BuyerSalesOrder? BuyerSalesOrder { get; set; }
        public SupplierPurchaseOrder? SupplierPurchaseOrder { get; set; }
        public Shipment? Shipment { get; set; }
        public ShipmentLeg? ShipmentLeg { get; set; }
        public InventoryLot? InventoryLot { get; set; }
        public IncotermRule? IncotermRule { get; set; }
        public GeographicLocation? Location { get; set; }
        public SupplyDocument? EvidenceSupplyDocument { get; set; }
    }
}
