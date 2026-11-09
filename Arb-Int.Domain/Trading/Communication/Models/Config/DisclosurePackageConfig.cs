using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class DisclosurePackageConfig : IEntityTypeConfiguration<DisclosurePackage>
    {
        public void Configure(EntityTypeBuilder<DisclosurePackage> builder)
        {
            builder.ToTable("DisclosurePackages");

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

            builder.Property(x => x.SourceEntityType)
                .HasMaxLength(255);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.AudienceBusinessRelationshipId,
                x.Status
            });

            builder.HasIndex(x => new
            {
                x.SourceEntityType,
                x.SourceEntityId
            });

            builder.HasOne(x => x.AudienceBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.AudienceBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Revisions)
                .WithOne(x => x.DisclosurePackage)
                .HasForeignKey(x => x.DisclosurePackageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
