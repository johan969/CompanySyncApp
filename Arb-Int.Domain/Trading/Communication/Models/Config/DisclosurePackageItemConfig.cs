using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class DisclosurePackageItemConfig : IEntityTypeConfiguration<DisclosurePackageItem>
    {
        public void Configure(EntityTypeBuilder<DisclosurePackageItem> builder)
        {
            builder.ToTable("DisclosurePackageItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.FieldKey)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Label)
                .HasMaxLength(255);

            builder.Property(x => x.Classification)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.SanitizedTextValue)
                .HasMaxLength(16000);

            builder.Property(x => x.SanitizedJsonValue)
                .HasColumnType("jsonb");

            builder.Property(x => x.SourceEntityType)
                .HasMaxLength(255);

            builder.Property(x => x.SourcePropertyName)
                .HasMaxLength(255);

            builder.HasIndex(x => new
            {
                x.DisclosurePackageRevisionId,
                x.FieldKey
            })
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.SourceEntityType,
                x.SourceEntityId
            });
        }
    }
}
