using Arb_Int.Domain.Economy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerSalesOrderConfig : IEntityTypeConfiguration<BuyerSalesOrder>
    {
        public void Configure(EntityTypeBuilder<BuyerSalesOrder> builder)
        {
            builder.ToTable(
                "BuyerSalesOrders",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerSalesOrders_Amounts",
                        """
                    "NetAmount" >= 0
                    AND "TaxAmount" >= 0
                    AND "GrossAmount" >= 0
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerProposalAcceptanceId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.OrderNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.NetAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.TaxAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.GrossAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.BuyerPurchaseOrderReference)
                .HasMaxLength(255);

            builder.Property(x => x.NamedPlace)
                .HasMaxLength(512);

            builder.Property(x => x.PaymentTerms)
                .HasMaxLength(2048);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => x.OrderNumber)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.BuyerBusinessRelationshipId,
                x.Status
            });

            builder.HasOne(x => x.BuyerBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.BuyerBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BuyerProposalAcceptance)
                .WithMany(x => x.BuyerSalesOrders)
                .HasForeignKey(x => x.BuyerProposalAcceptanceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IncotermRule)
                .WithMany()
                .HasForeignKey(x => x.IncotermRuleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.BuyerSalesOrder)
                .HasForeignKey(x => new
                {
                    x.BuyerSalesOrderId,
                    x.BuyerProposalAcceptanceId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerProposalAcceptanceId
                })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Deliveries)
                .WithOne(x => x.BuyerSalesOrder)
                .HasForeignKey(x => x.BuyerSalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.StatusHistory)
                .WithOne(x => x.BuyerSalesOrder)
                .HasForeignKey(x => x.BuyerSalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
