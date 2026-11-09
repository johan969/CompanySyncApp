using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ShipmentOrderLineAllocationConfig : IEntityTypeConfiguration<ShipmentOrderLineAllocation>
    {
        public void Configure(EntityTypeBuilder<ShipmentOrderLineAllocation> builder)
        {
            builder.ToTable(
                "ShipmentOrderLineAllocations",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_ShipmentOrderLineAllocations_OrderSide",
                        """
                    num_nonnulls("BuyerSalesOrderLineId", "SupplierPurchaseOrderLineId") = 1
                    """);

                    table.HasCheckConstraint(
                        "CK_ShipmentOrderLineAllocations_Quantities",
                        """
                    "AllocatedQuantity" > 0
                    AND "DeliveredQuantity" >= 0
                    AND "DeliveredQuantity" <= "AllocatedQuantity"
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.AllocatedQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.DeliveredQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.ShipmentId,
                x.BuyerSalesOrderLineId,
                x.SupplierPurchaseOrderLineId
            });

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

            builder.HasOne(x => x.BuyerSalesOrderLine)
                .WithMany()
                .HasForeignKey(x => x.BuyerSalesOrderLineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierPurchaseOrderLine)
                .WithMany()
                .HasForeignKey(x => x.SupplierPurchaseOrderLineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
