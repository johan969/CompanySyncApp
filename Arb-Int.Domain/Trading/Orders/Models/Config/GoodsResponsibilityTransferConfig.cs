using Arb_Int.Domain.Sourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class GoodsResponsibilityTransferConfig : IEntityTypeConfiguration<GoodsResponsibilityTransfer>
    {
        public void Configure(EntityTypeBuilder<GoodsResponsibilityTransfer> builder)
        {
            builder.ToTable(
                "GoodsResponsibilityTransfers",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_GoodsResponsibilityTransfers_DifferentParties",
                        """
                    "FromPartyId" <> "ToPartyId"
                    """);

                    table.HasCheckConstraint(
                        "CK_GoodsResponsibilityTransfers_Context",
                        """
                    num_nonnulls("BuyerSalesOrderId", "SupplierPurchaseOrderId", "ShipmentId", "ShipmentLegId",
                        "InventoryLotId") >= 1
                    """);

                    table.HasCheckConstraint(
                        "CK_GoodsResponsibilityTransfers_ShipmentLeg",
                        """
                    "ShipmentLegId" IS NULL
                    OR "ShipmentId" IS NOT NULL
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ContractualBasis)
                .HasMaxLength(4096);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.Kind,
                x.Status,
                x.PlannedTransferAtUtc
            });

            builder.HasOne(x => x.FromParty)
                .WithMany()
                .HasForeignKey(x => x.FromPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ToParty)
                .WithMany()
                .HasForeignKey(x => x.ToPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BuyerSalesOrder)
                .WithMany()
                .HasForeignKey(x => x.BuyerSalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierPurchaseOrder)
                .WithMany()
                .HasForeignKey(x => x.SupplierPurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.ShipmentLegId,
                    x.ShipmentId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.ShipmentId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InventoryLot)
                .WithMany()
                .HasForeignKey(x => x.InventoryLotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IncotermRule)
                .WithMany()
                .HasForeignKey(x => x.IncotermRuleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.EvidenceSupplyDocument)
                .WithMany()
                .HasForeignKey(x => x.EvidenceSupplyDocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
