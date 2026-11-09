using Arb_Int.Domain.Economy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplierPurchaseOrderConfig : IEntityTypeConfiguration<SupplierPurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<SupplierPurchaseOrder> builder)
        {
            builder.ToTable(
                "SupplierPurchaseOrders",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplierPurchaseOrders_Amounts",
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
                x.SupplierBusinessRelationshipId
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

            builder.Property(x => x.SupplierReference)
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
                x.SupplierBusinessRelationshipId,
                x.Status
            });

            builder.HasOne(x => x.SupplierBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.SupplierBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierQuoteRevision)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.SupplierQuoteRevisionId,
                    x.SupplierBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplierBusinessRelationshipId
                })
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
                .WithOne(x => x.SupplierPurchaseOrder)
                .HasForeignKey(x => new
                {
                    x.SupplierPurchaseOrderId,
                    x.SupplierBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplierBusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.StatusHistory)
                .WithOne(x => x.SupplierPurchaseOrder)
                .HasForeignKey(x => x.SupplierPurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
