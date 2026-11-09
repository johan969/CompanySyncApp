using Arb_Int.Domain.Sourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ExternalMessageAttachmentConfig : IEntityTypeConfiguration<ExternalMessageAttachment>
    {
        public void Configure(EntityTypeBuilder<ExternalMessageAttachment> builder)
        {
            builder.ToTable("ExternalMessageAttachments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.FileName)
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasMaxLength(255);

            builder.Property(x => x.StorageProvider)
                .HasMaxLength(100);

            builder.Property(x => x.StorageKey)
                .HasMaxLength(2048);

            builder.Property(x => x.ContentHash)
                .HasMaxLength(128);

            builder.HasIndex(x => x.ContentHash);

            builder.HasOne(x => x.SupplyDocument)
                .WithMany()
                .HasForeignKey(x => x.SupplyDocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
