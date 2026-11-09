using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class DisclosurePackageRevisionConfig : IEntityTypeConfiguration<DisclosurePackageRevision>
    {
        public void Configure(EntityTypeBuilder<DisclosurePackageRevision> builder)
        {
            builder.ToTable("DisclosurePackageRevisions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Subject)
                .HasMaxLength(500);

            builder.Property(x => x.ModeratedSummary)
                .HasMaxLength(16000);

            builder.Property(x => x.SnapshotJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.ContentHash)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.DisclosurePackageId,
                x.RevisionNumber
            })
                .IsUnique();

            builder.HasIndex(x => x.ContentHash);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.DisclosurePackageRevision)
                .HasForeignKey(x => x.DisclosurePackageRevisionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
