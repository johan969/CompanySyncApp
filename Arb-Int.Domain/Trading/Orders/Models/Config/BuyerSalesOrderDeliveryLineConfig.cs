using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerSalesOrderDeliveryLineConfig : IEntityTypeConfiguration<BuyerSalesOrderDeliveryLine>
    {
        public void Configure(EntityTypeBuilder<BuyerSalesOrderDeliveryLine> builder)
        {
            builder.ToTable(
                "BuyerSalesOrderDeliveryLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerSalesOrderDeliveryLines_Values",
                        """
                    "PlannedQuantity" > 0
                    AND "DeliveredQuantity" >= 0
                    AND "DeliveredQuantity" <= "PlannedQuantity"
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.PlannedQuantity)
                .HasPrecision(20, 6);
            builder.Property(x => x.DeliveredQuantity)
                .HasPrecision(20, 6);
            builder.HasIndex(x => new
            {
                x.BuyerSalesOrderDeliveryId,
                x.BuyerSalesOrderLineId
            })
                .IsUnique();
            builder.HasOne(x => x.BuyerSalesOrderLine)
                .WithMany(x => x.DeliveryLines)
                .HasForeignKey(x => new
                {
                    x.BuyerSalesOrderLineId,
                    x.BuyerSalesOrderId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerSalesOrderId
                })
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
