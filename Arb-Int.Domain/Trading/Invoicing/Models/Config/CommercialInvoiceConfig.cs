using Arb_Int.Domain.Economy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class CommercialInvoiceConfig : IEntityTypeConfiguration<CommercialInvoice>
    {
        public void Configure(EntityTypeBuilder<CommercialInvoice> builder)
        {
            builder.ToTable(
                "CommercialInvoices",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_CommercialInvoices_OrderSide",
                        """
                    num_nonnulls("BuyerSalesOrderId", "SupplierPurchaseOrderId") <= 1
                    """);

                    table.HasCheckConstraint(
                        "CK_CommercialInvoices_Amounts",
                        """
                    "NetAmount" >= 0
                    AND "TaxAmount" >= 0
                    AND "GrossAmount" >= 0
                    AND "PaidAmount" >= 0
                    AND "OutstandingAmount" >= 0
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Direction)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.SystemOwnership)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.InvoiceNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.SyncStatus)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.NetAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.TaxAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.GrossAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.PaidAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.OutstandingAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.ExternalInvoiceId)
                .HasMaxLength(255);

            builder.Property(x => x.ExternalInvoiceNumber)
                .HasMaxLength(255);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.Direction,
                x.InvoiceNumber
            })
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.CounterpartyBusinessRelationshipId,
                x.Status,
                x.DueDateUtc
            });

            builder.HasIndex(x => new
            {
                x.SyncStatus,
                x.ExternalSystemId
            });

            builder.HasOne(x => x.CounterpartyBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.CounterpartyBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BuyerSalesOrder)
                .WithMany()
                .HasForeignKey(x => x.BuyerSalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierPurchaseOrder)
                .WithMany()
                .HasForeignKey(x => x.SupplierPurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ExternalSystem)
                .WithMany()
                .HasForeignKey(x => x.ExternalSystemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.CommercialInvoice)
                .HasForeignKey(x => x.CommercialInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ExternalReferences)
                .WithOne(x => x.CommercialInvoice)
                .HasForeignKey(x => x.CommercialInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.SyncAttempts)
                .WithOne(x => x.CommercialInvoice)
                .HasForeignKey(x => x.CommercialInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.PaymentSnapshots)
                .WithOne(x => x.CommercialInvoice)
                .HasForeignKey(x => x.CommercialInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
