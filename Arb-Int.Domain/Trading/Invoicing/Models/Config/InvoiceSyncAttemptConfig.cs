using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class InvoiceSyncAttemptConfig : IEntityTypeConfiguration<InvoiceSyncAttempt>
    {
        public void Configure(EntityTypeBuilder<InvoiceSyncAttempt> builder)
        {
            builder.ToTable("InvoiceSyncAttempts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Direction)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.CorrelationId)
                .HasMaxLength(255);

            builder.Property(x => x.RequestPayloadHash)
                .HasMaxLength(128);

            builder.Property(x => x.ResponsePayloadHash)
                .HasMaxLength(128);

            builder.Property(x => x.ExternalEntityId)
                .HasMaxLength(255);

            builder.Property(x => x.ErrorCode)
                .HasMaxLength(255);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(8000);

            builder.HasIndex(x => new
            {
                x.CommercialInvoiceId,
                x.StartedAtUtc
            });

            builder.HasIndex(x => new
            {
                x.Status,
                x.StartedAtUtc
            });

            builder.HasOne(x => x.ExternalSystem)
                .WithMany()
                .HasForeignKey(x => x.ExternalSystemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
