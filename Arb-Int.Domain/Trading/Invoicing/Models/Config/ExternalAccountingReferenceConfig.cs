using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ExternalAccountingReferenceConfig : IEntityTypeConfiguration<ExternalAccountingReference>
    {
        public void Configure(EntityTypeBuilder<ExternalAccountingReference> builder)
        {
            builder.ToTable("ExternalAccountingReferences");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.ExternalEntityType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ExternalEntityId)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.ExternalEntityNumber)
                .HasMaxLength(255);

            builder.Property(x => x.ExternalUrl)
                .HasMaxLength(2048);

            builder.Property(x => x.ExternalVersionToken)
                .HasMaxLength(1000);

            builder.Property(x => x.SyncStatus)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.HasIndex(x => new
            {
                x.ExternalSystemId,
                x.ExternalEntityType,
                x.ExternalEntityId
            })
                .IsUnique();

            builder.HasOne(x => x.ExternalSystem)
                .WithMany()
                .HasForeignKey(x => x.ExternalSystemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
