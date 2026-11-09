using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ReconciliationTaskConfig : IEntityTypeConfiguration<ReconciliationTask>
    {
        public void Configure(EntityTypeBuilder<ReconciliationTask> builder)
        {
            builder.ToTable(
                "ReconciliationTasks",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_ReconciliationTasks_Context",
                        """
                    num_nonnulls("CommercialInvoiceId", "BuyerSalesOrderId", "SupplierPurchaseOrderId",
                        "ShipmentId") >= 1
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ResolutionNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.Status,
                x.AssignedUserId,
                x.DueAtUtc
            });

            builder.HasOne(x => x.CommercialInvoice)
                .WithMany(x => x.ReconciliationTasks)
                .HasForeignKey(x => x.CommercialInvoiceId)
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
        }
    }
}
