using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class PaymentStatusSnapshotConfig : IEntityTypeConfiguration<PaymentStatusSnapshot>
    {
        public void Configure(EntityTypeBuilder<PaymentStatusSnapshot> builder)
        {
            builder.ToTable(
                "PaymentStatusSnapshots",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_PaymentStatusSnapshots_Amounts",
                        """
                    "PaidAmount" >= 0
                    AND "OutstandingAmount" >= 0
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);
            builder.Property(x => x.PaidAmount)
                .HasPrecision(20, 6);
            builder.Property(x => x.OutstandingAmount)
                .HasPrecision(20, 6);
            builder.Property(x => x.ExternalPaymentReference)
                .HasMaxLength(255);
            builder.Property(x => x.SnapshotJson)
                .HasColumnType("jsonb");
            builder.HasIndex(x => new
            {
                x.CommercialInvoiceId,
                x.ObservedAtUtc
            });

            builder.HasOne(x => x.ExternalSystem)
                .WithMany()
                .HasForeignKey(x => x.ExternalSystemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
