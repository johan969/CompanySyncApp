using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerProposalLineConfig : IEntityTypeConfiguration<BuyerProposalLine>
    {
        public void Configure(EntityTypeBuilder<BuyerProposalLine> builder)
        {
            builder.ToTable(
                "BuyerProposalLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerProposalLines_Values",
                        """
                    "Quantity" > 0
                    AND "UnitPrice" >= 0
                    AND "NetAmount" >= 0
                    AND "GrossAmount" >= 0
                    """);

                    table.HasCheckConstraint(
                        "CK_BuyerProposalLines_DeliveryWindow",
                        """
                        "EstimatedDeliveryToUtc" IS NULL
                        OR "EstimatedDeliveryFromUtc" IS NULL
                        OR "EstimatedDeliveryToUtc" >= "EstimatedDeliveryFromUtc"
                        """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerProposalOptionId
            });
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

            builder.Property(x => x.BuyerFacingNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.BuyerProposalOptionId,
                x.LineNumber
            })
                .IsUnique();

            builder.HasOne(x => x.BuyerRfqLine)
                .WithMany()
                .HasForeignKey(x => x.BuyerRfqLineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplySolutionLine)
                .WithMany()
                .HasForeignKey(x => x.SupplySolutionLineId)
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
        }
    }
}
