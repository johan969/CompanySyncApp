using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class CommercialInvoiceLineConfig : IEntityTypeConfiguration<CommercialInvoiceLine>
    {
        public void Configure(EntityTypeBuilder<CommercialInvoiceLine> builder)
        {
            builder.ToTable(
                "CommercialInvoiceLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_CommercialInvoiceLines_Values",
                        """
                    "Quantity" >= 0
                    AND "UnitPrice" >= 0
                    AND "NetAmount" >= 0
                    AND "TaxAmount" >= 0
                    AND "GrossAmount" >= 0
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Quantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.UnitPrice)
                .HasPrecision(20, 6);

            builder.Property(x => x.NetAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.TaxPercent)
                .HasPrecision(9, 4);

            builder.Property(x => x.TaxAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.GrossAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.ExternalLineId)
                .HasMaxLength(255);

            builder.HasIndex(x => new
            {
                x.CommercialInvoiceId,
                x.LineNumber
            })
                .IsUnique();

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
