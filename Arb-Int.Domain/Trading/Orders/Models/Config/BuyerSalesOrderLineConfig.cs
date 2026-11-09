using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerSalesOrderLineConfig : IEntityTypeConfiguration<BuyerSalesOrderLine>
    {
        public void Configure(EntityTypeBuilder<BuyerSalesOrderLine> builder)
        {
            builder.ToTable(
                "BuyerSalesOrderLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerSalesOrderLines_Values",
                        """
                    "OrderedQuantity" > 0
                    AND "FulfilledQuantity" >= 0
                    AND "FulfilledQuantity" <= "OrderedQuantity"
                    AND "UnitPrice" >= 0
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerSalesOrderId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.OrderedQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.FulfilledQuantity)
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

            builder.Property(x => x.BuyerLineReference)
                .HasMaxLength(255);

            builder.HasIndex(x => new
            {
                x.BuyerSalesOrderId,
                x.LineNumber
            })
                .IsUnique();

            builder.HasOne(x => x.BuyerProposalAcceptanceLine)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.BuyerProposalAcceptanceLineId,
                    x.BuyerProposalAcceptanceId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerProposalAcceptanceId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Material)
                .WithMany()
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PurchaseAllocations)
                .WithOne(x => x.BuyerSalesOrderLine)
                .HasForeignKey(x => x.BuyerSalesOrderLineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
