using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SalesPurchaseAllocationConfig : IEntityTypeConfiguration<SalesPurchaseAllocation>
    {
        public void Configure(EntityTypeBuilder<SalesPurchaseAllocation> builder)
        {
            builder.ToTable(
                "SalesPurchaseAllocations",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SalesPurchaseAllocations_Quantity",
                        """
                    "AllocatedQuantity" > 0
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.AllocatedQuantity)
                .HasPrecision(20, 6);
            builder.Property(x => x.AllocatedPurchaseCost)
                .HasPrecision(20, 6);
            builder.Property(x => x.Notes)
                .HasMaxLength(4096);
            builder.HasIndex(x => new
            {
                x.BuyerSalesOrderLineId,
                x.SupplierPurchaseOrderLineId
            })
                .IsUnique();
            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.PurchaseCostCurrency)
                .WithMany()
                .HasForeignKey(x => x.PurchaseCostCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
