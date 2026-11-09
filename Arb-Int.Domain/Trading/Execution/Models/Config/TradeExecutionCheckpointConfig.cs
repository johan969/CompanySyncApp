using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class TradeExecutionCheckpointConfig : IEntityTypeConfiguration<TradeExecutionCheckpoint>
    {
        public void Configure(EntityTypeBuilder<TradeExecutionCheckpoint> builder)
        {
            builder.ToTable(
                "TradeExecutionCheckpoints",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_TradeExecutionCheckpoints_Context",
                        """
                    num_nonnulls("ParentCheckpointId", "BuyerProposalAcceptanceId", "BuyerSalesOrderId",
                        "SupplierPurchaseOrderId", "SupplySolutionStepId", "ShipmentId", "CommercialInvoiceId",
                        "GoodsResponsibilityTransferId") >= 1
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.CompletionMode)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.AutomationHandlerKey)
                .HasMaxLength(255);

            builder.Property(x => x.ExternalReference)
                .HasMaxLength(1000);

            builder.Property(x => x.CompletionEvidenceJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.Status,
                x.DueAtUtc,
                x.OwnerUserId
            });

            builder.HasIndex(x => new
            {
                x.BuyerSalesOrderId,
                x.SupplierPurchaseOrderId,
                x.ShipmentId
            });

            builder.HasIndex(x => new
            {
                x.CommercialInvoiceId,
                x.Status
            });

            builder.HasOne(x => x.ParentCheckpoint)
                .WithMany(x => x.ChildCheckpoints)
                .HasForeignKey(x => x.ParentCheckpointId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BuyerProposalAcceptance)
                .WithMany()
                .HasForeignKey(x => x.BuyerProposalAcceptanceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BuyerSalesOrder)
                .WithMany()
                .HasForeignKey(x => x.BuyerSalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierPurchaseOrder)
                .WithMany()
                .HasForeignKey(x => x.SupplierPurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplySolutionStep)
                .WithMany()
                .HasForeignKey(x => x.SupplySolutionStepId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CommercialInvoice)
                .WithMany()
                .HasForeignKey(x => x.CommercialInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.GoodsResponsibilityTransfer)
                .WithMany()
                .HasForeignKey(x => x.GoodsResponsibilityTransferId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ExternalSystem)
                .WithMany()
                .HasForeignKey(x => x.ExternalSystemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
